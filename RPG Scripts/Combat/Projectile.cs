using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;
using RPG.FX;
using RPG.Control;
using UnityEngine.Events;
using RPG.Sound;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] bool isHoming = false;
        [SerializeField] float liftime = 50f;
        //[SerializeField] float timeAllowForStationary = .5f;
        //[SerializeField] GameObject CollideFX;
        [SerializeField] FXHandler fXHandler;
        [SerializeField] SoundFXHandler soundFXHandler;
        //public Transform targetTransform;
        public float travelspeed;
        private float hitDammage;
        //private Vector3 lastPosition;
        //private float stationaryTime = 0f;
        private GameObject instigator;
        //private ColliderHandler colliders;
        private WeaponHandler weaponHandler;
        //private GameObject enemy;

        private void Start()
        {
            Invoke("DestroyThis", liftime);
            
        }

        // Update is called once per frame
        void Update()
        {

            //if (targetTransform == null) Destroy(gameObject);
            //lastPosition = transform.position;
            //if (enemy == null) return;
            if (isHoming)
            {                
                //if (targetTransform.GetComponent<Health>().IsDead()) Destroy(gameObject);
                try
                {
                    transform.LookAt(weaponHandler.GetAimLocation());
                }
                catch (Exception e) { Debug.Log($"Proplem with Homing Projectile {e.Message} : {e.StackTrace}"); }
            }
            transform.Translate(Vector3.forward * travelspeed * Time.deltaTime);
            //if (lastPosition == transform.position)
            //{
            //    stationaryTime += Time.deltaTime;
            //    if (stationaryTime > timeAllowForStationary) Destroy(gameObject);
            //}
            //else
            //{
            //    stationaryTime = 0f;
            //}
        }

        //internal void GoToTarget(Transform target, float speed, float dammage, GameObject fighter, ColliderHandler colliders)
        //{
        //    //Debug.Log("Weapon projectile has been sent");
        //    hitDammage = dammage;
        //    targetTransform = target;
        //    travelspeed = speed;
        //    instigator = fighter;
        //}
        internal void GoToTarget(float speed, float dammage, GameObject fighter, WeaponHandler weapon)
        {
            transform.LookAt(weapon.GetAimLocation());
            weaponHandler = weapon;
            //Debug.Log("Weapon projectile has been sent");
            hitDammage = dammage;
            //targetTransform = target;
            travelspeed = speed;
            instigator = fighter;
        }

        //private Vector3 GetAimLocation()
        //{
        //    bool target = false;
        //    try
        //    {
        //        var component = targetTransform.gameObject.GetComponents<Collider>();
        //        target = component.Length > 0;
        //        //if (component == null) return transform.position;
        //    }
        //    catch (Exception E)
        //    {
        //        Debug.LogWarning($"Caught Error: {E.Message}");
        //        gameObject.name = "not good!!";
        //        //Destroy(gameObject);
        //        return transform.position;
        //    }
        //    if (target)
        //    {
        //        CapsuleCollider targetCapsule = targetTransform.GetComponent<CapsuleCollider>();
        //        GameObject targetController = targetCapsule.attachedRigidbody.gameObject;
        //        Debug.Log($"{instigator.name} has targeted {targetController.name} {targetCapsule.name}");
        //        gameObject.name = "ok!!";
        //        return targetTransform.position + (Vector3.up * targetCapsule.center.y);
        //    }
        //    gameObject.name = "look!!";
        //    return targetTransform.position + (Vector3.up * transform.position.y / 2);
        //}

        private void OnTriggerEnter(Collider other)
        {
            //if (other.tag == "Weapon") return;
            //Debug.Log("Projectile Hit");
            Rigidbody rigidbody = null;
            try
            {
                rigidbody = other.attachedRigidbody;
            }
            catch (Exception E)
            {
                Debug.LogError("Error caught : " + E.Message);
            }
            if (rigidbody == null)
            {
                Debug.Log("hit other");
                DestroyThis();
                return;
            }
            try
            {
                Health targetHealth = rigidbody.gameObject.GetComponent<Health>();
                if (targetHealth == instigator.GetComponent<Health>()) return;
                if (targetHealth != null)
                {
                    targetHealth.TakeDammage(hitDammage, instigator);
                    fXHandler.CreateFX(other.transform);
                    soundFXHandler.CreateFX(other.transform);
                    DestroyThis();                    
                }
            }
            catch (Exception E)
            {
                Debug.LogError($"Error Caught : {other.attachedArticulationBody.gameObject.name} - {E.Message}");
            }
            //Debug.Log("hit something else");
            DestroyThis();            
        }

        private void DestroyThis()
        {
            Destroy(gameObject);
        }
        private void OnDestroy()
        {
            //Debug.Log("Bullet Destroyed");
        }
    }
}
