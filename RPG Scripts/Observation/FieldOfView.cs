using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Control;


namespace RPG.Observation
{
    public class FieldOfView : MonoBehaviour
    {
        public enum Visability {Visable, CloseRange, Aware, NotVisable, Hear, ActiveRange}
        [SerializeField] Visability visability;
        [SerializeField] Color colour;
        [SerializeField] float viewRadius;
        [SerializeField] string tagToLookFor = "BodyPart";
        [Range(0,360)]
        [SerializeField] float viewAngle;
        [SerializeField] Transform eye = null;
        //[SerializeField] float eyeMovement = 50f;
        private List<GameObject> visibleTargets = new List<GameObject>();
        public float ViewRadius { get => viewRadius; set => viewRadius = value; }
        public float ViewAngle { get => viewAngle; set => viewAngle = value; }
        public List<GameObject> VisibleTargets { get => visibleTargets;}
        public Transform Eye { get => eye;}
        public Visability VisabilityType { get => visability; set => visability = value; }
        public Color Colour { get => colour; }

        public IEnumerator FindTargetsWithDelay(float deley, ITestForTarget testForTarget)
        {
            while (true)
            {
                yield return new WaitForSeconds(deley);
                FindVisableTargets(testForTarget);
            }
        }

        public void LookAtTarget(Transform target)
        {
            if (eye == null) return;
            eye.transform.LookAt(target);
            //transform.Rotate(Vector3.up, eyeMovement * Time.deltaTime);
        }

        public List<GameObject> FindVisableTargets(ITestForTarget testForTarget)
        {
            Transform hasHit = null;
            visibleTargets.Clear();
            try
            {
                Collider[] targetsInViewRadius = Physics.OverlapSphere(eye.position, viewRadius);
                for (int i = 0; i < targetsInViewRadius.Length; i++)
                {
                    if (GetComponent<ColliderHandler>().Colliders.Contains(targetsInViewRadius[i]))
                    {
                        //Debug.Log($"{gameObject.name} has seen own {targetsInViewRadius[i].gameObject.name}");
                        continue;
                    }
                    if (targetsInViewRadius[i].gameObject.tag != tagToLookFor) continue;
                    //if (targetsInViewRadius[i].gameObject.tag == "BodyPart") Debug.LogWarning($"BodyPart Found!!!!! {targetsInViewRadius[i].gameObject.name}");
                    Transform target = targetsInViewRadius[i].transform;
                    Vector3 directionToTarget = target.position;
                    if (Vector3.Angle(eye.forward, (directionToTarget - eye.position).normalized) < viewAngle / 2)
                    {
                        //Debug.Log("collider is view");
                        //if (targetsInViewRadius[i].gameObject.tag == "BodyPart") Debug.LogWarning($"{targetsInViewRadius[i].gameObject.tag} Found!!!!!");
                        hasHit = TestRayTransformForTarget(testForTarget, target, directionToTarget, targetsInViewRadius[i]);
                    }
                    //else Debug.Log("collider not in view");
                }
            }
            catch (System.Exception e)
            {
                Debug.Log($"No Targets - {e.Message} - {e.TargetSite}");
            }
            return visibleTargets;
        }

        //public List<GameObject> FindAudableTargets(ITestForTarget testForTarget)
        //{
        //    Transform hasHit = null;
        //    visibleTargets.Clear();
        //    try
        //    {
        //        Collider[] targetsInViewRadius = Physics.OverlapSphere(eye.position, viewRadius);
        //        for (int i = 0; i < targetsInViewRadius.Length; i++)
        //        {
        //            if (GetComponent<ColliderHandler>().Colliders.Contains(targetsInViewRadius[i]))
        //            {
        //                //Debug.Log($"{gameObject.name} has seen own {targetsInViewRadius[i].gameObject.name}");
        //                continue;
        //            }
        //            if (targetsInViewRadius[i].gameObject.tag != tagToLookFor) continue;
        //            //if (targetsInViewRadius[i].gameObject.tag == "BodyPart") Debug.LogWarning($"BodyPart Found!!!!! {targetsInViewRadius[i].gameObject.name}");
        //            Transform target = targetsInViewRadius[i].transform;
        //            Vector3 directionToTarget = target.position;
        //            if (Vector3.Angle(eye.forward, (directionToTarget - eye.position).normalized) < viewAngle / 2)
        //            {
        //                //Debug.Log("collider is view");
        //                //if (targetsInViewRadius[i].gameObject.tag == "BodyPart") Debug.LogWarning($"{targetsInViewRadius[i].gameObject.tag} Found!!!!!");
        //                hasHit = TestRayTransformForTarget(testForTarget, target, directionToTarget, targetsInViewRadius[i]);
        //            }
        //            //else Debug.Log("collider not in view");
        //        }
        //    }
        //    catch (System.Exception e)
        //    {
        //        Debug.Log($"No Targets - {e.Message} - {e.TargetSite}");
        //    }
        //    return visibleTargets;
        //}

        private Transform TestRayTransformForTarget(ITestForTarget testForTarget, Transform target, Vector3 directionToTarget, Collider collider)
        {
            //float distanceToTarget = Vector3.Distance(transform.position, target.position);
            Ray ray = new Ray(eye.position, (directionToTarget - eye.position).normalized * viewRadius);
            RaycastHit hit;
            Transform hitTransform = null;
            bool hasHit = Physics.Raycast(ray, out hit);
            if (hasHit)
            {
                //Debug.Log("test for ray");
                if (hit.collider != collider)
                {
                    //Debug.LogWarning($"{hit.collider.ToString()} not = to {collider.ToString()}");                    
                    //Debug.DrawRay(eye.position, (directionToTarget - eye.position).normalized * viewRadius, Color.red);
                    return null;
                }
                else
                {
                    //Debug.DrawRay(eye.position, (directionToTarget - eye.position).normalized * viewRadius, Color.green);
                    //Debug.LogWarning("Colliders the same!!!");
                }
                (bool on, GameObject obj) tple;
                tple = (testForTarget.TestForTarget(collider, visibleTargets));
                hasHit = tple.on;
                if (hasHit)
                {
                    //Debug.LogWarning("Has Hit! Yay!!!!!");
                    visibleTargets.Add(tple.obj);
                }
            }
            return hitTransform;
        }

        public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += eye.eulerAngles.y;
            }
            return eye.rotation * new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }


    }
}
