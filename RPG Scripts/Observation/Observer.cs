using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;
using System;

namespace RPG.Observation
{
    [DisallowMultipleComponent]
    public class Observer : MonoBehaviour, ITestForTarget
    {
        private FieldOfView[] views;
        public List<GameObject> VisibleTargets { get => Targets(FieldOfView.Visability.Visable); }
        public List<GameObject> AwareTargets { get => Targets(FieldOfView.Visability.Aware); }
        public List<GameObject> CloseRangeTargets { get => Targets(FieldOfView.Visability.CloseRange); }
        public List<GameObject> HearRangeTargets { get => Targets(FieldOfView.Visability.Hear); }
        public List<GameObject> SoundRangeTargets { get => Targets(FieldOfView.Visability.Hear, true); }
        public string TagToLookFor { get => tagToLookFor; }
        [SerializeField] GameObject HeadAimTarget;

        [SerializeField] string tagToLookFor = "BodyPart";
        //public Action onVisible;
        //public Action onVisabilityLost;
        //private bool visible = false;
        // Start is called before the first frame update
        void Start()
        {
            views = GetComponents<FieldOfView>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public (bool, GameObject) TestForTarget(Collider collider, List<GameObject> gameObjects)
        {
            if (collider == null)
            {
                return (false, null);
            }
            if (collider.gameObject.tag != tagToLookFor)
            {
                return (false, null);
            }
            Rigidbody rigidbody;
            try
            {
                rigidbody = collider.attachedRigidbody;
                if (rigidbody == null)
                {
                    return (false, null);
                }
            }
            catch (Exception E)
            {
                Debug.LogWarning(E.Message);
                return (false, null);
            }
            RPGCharactorController controller = rigidbody.gameObject.GetComponent<RPGCharactorController>();
            if (controller != null)
            {
                if (!controller.IsDead())
                {
                    if (!gameObjects.Contains(controller.gameObject)) return (true, controller.gameObject);
                }
            }
            return (false, null);
        }

        public List<GameObject> Targets(FieldOfView.Visability visability, bool overrideHear = false)
        {
            List<GameObject> targetList = new List<GameObject>();
            switch (visability)
            {
                case FieldOfView.Visability.Hear:
                    if (overrideHear) CheckViewsForVisability(visability, targetList, false);
                    else CheckViewsForVisability(visability, targetList, true);
                    break;
                default:
                    CheckViewsForVisability(visability, targetList, false);
                    break;
            }           
            return targetList;
        }

        static public List<GameObject> ObjectsWithinRange(Transform TargetPoint, float viewRadius, ITestForTarget testForTarget, ColliderHandler colliderHandler, string tagToLookFor)
        {
            List<GameObject> objects = new List<GameObject>();
            Collider[] targetsInViewRadius = Physics.OverlapSphere(TargetPoint.position, viewRadius);
            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                if (colliderHandler.Colliders.Contains(targetsInViewRadius[i]))
                {
                    continue;
                }
                if (targetsInViewRadius[i].gameObject.tag != tagToLookFor) continue;
                (bool on, GameObject obj) tple;
                tple = (testForTarget.TestForTarget(targetsInViewRadius[i], objects));
                if (tple.on) objects.Add(tple.obj);
            }
            return objects;
        }

        private void CheckViewsForVisability(FieldOfView.Visability visability, List<GameObject> targetList, bool checkForSound)
        {
            foreach (FieldOfView view in views)
            {
                if (view.VisabilityType != visability) continue;
                foreach (GameObject obj in view.FindVisableTargets(this))
                {
                    if (obj == null) continue;
                    bool check = (checkForSound == false || obj.GetComponent<SoundAlerts>().NoiseActive);
                    if (!targetList.Contains(obj) && check)
                    {
                        //Debug.Log($"target object {obj.name}");
                        targetList.Add(obj);
                        //if (visability == FieldOfView.Visability.Hear)
                        //{
                        //    string sound;
                        //    if (checkForSound) sound = "heard"; else sound = "Called to";
                        //    Debug.Log($"{obj.name} has been {sound}");
                        //    Debug.DrawRay(view.Eye.position, (obj.transform.position - view.Eye.position).normalized * view.ViewRadius, Color.magenta);
                        //}
                        //view.LookAtTarget(obj.transform);
                    }
                }
            }
        }
        public void SetFocus(Vector3 target, float lookSpeed)
        {
            Vector3 targetDirection = target - transform.position;
            float singleStep = lookSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            //Debug.Log("Moving to target");
            HeadAimTarget.transform.position = new Vector3(target.x, target.y, target.z);
        }


        public static bool WithinDistance(float distance, GameObject objectToFind, Transform transform)
        {
            
            bool spotted = BetweenDistance(objectToFind.transform, transform) < distance;            
            return spotted;
        }
        
        public static float BetweenDistance(Transform A, Transform B)
        {
            return Vector3.Distance(A.position, B.position);
        }

        public static (bool hasHit, RaycastHit hit) HasHit(Ray ray)
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit);
            if (hasHit)
            {
                return (true, hit);
            }
            return (false, hit);
        }

        public static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        public static Vector3 MousePosition(Camera camera)
        {
            return camera.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.y));
        }

        public static bool OnScreen(Vector3 position, Camera cam)
        {
            bool onScreen = false;
            Vector3 screenPoint = cam.WorldToViewportPoint(position);
            if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1) onScreen = true;
            return onScreen;
        }

        public static bool CheckCamerasForOnScreen(Vector3 position, Camera[] cameras)
        {
            bool onScreen = false;
            foreach (Camera cam in cameras)
            {
                onScreen = OnScreen(position, cam);
                if (onScreen) break;
            }
            return onScreen;
        }

        //public static void LookAtTarget(Vector3 target, Transform objectToLook)
        //{
        //    objectToLook.LookAt(target + Vector3.up * objectToLook.position.y);
        //}
    }
}
