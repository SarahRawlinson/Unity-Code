using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;

namespace RPG.Control
{
    [DisallowMultipleComponent]
    public class ColliderHandler : MonoBehaviour
    {
        [Serializable] 
        public class TargetCollider
        {
            [SerializeField] BodyTarget targetType;
            [SerializeField] Collider collider;
            [SerializeField] BodyPartType bodyPartType;

            public BodyTarget TargetType { get => targetType; }
            public Collider Collider { get => collider; }
            public BodyPartType BodyPart { get => bodyPartType; }
            
        }


        public enum BodyPartType
        {
            Head,
            Chest,
            Arm,
            Leg,            
            Hand,
            Foot
        }

        public enum BodyTarget
        {
            headCollider,
            chestCollider,
            hipCollider,
            rightSholderCollider,
            rightElbowCollider,
            rightHandCollider,
            rightUpperLegCollider,
            rightLowerLegCollider,
            rightFootCollider,
            leftSholderCollider,
            leftElbowCollider,
            leftHandCollider,
            leftUpperLegCollider,
            leftLowerLegCollider,
            leftFootCollider
        }

        [SerializeField] Collider triggerCollider;
        [SerializeField] List<TargetCollider> colliders;
        public bool isActive;

        public List<Collider> Colliders { get => ColliderList();}
        public GameObject CharactorBumped { get => charactorBumped;  }
        public GameObject CharactorDamaging { get => charactorDamaging;  }
        public event Action OnTakingDamage;
        public event Action OnBumping;

        private GameObject charactorBumped;
        private GameObject charactorDamaging;
        [SerializeField] float alertCoolDownHit = 5f;
        private float timeSinceAlertHit;
        [SerializeField] float alertCoolDownBump = 5f;
        private float timeSinceAlertBump;

        private void Update()
        {
            timeSinceAlertBump += Time.deltaTime;
            timeSinceAlertHit += Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            try
            {
                GameObject colliderObject = other.attachedRigidbody.gameObject;
                if (gameObject == colliderObject) return;
                WeaponHit hit = colliderObject.GetComponent<WeaponHit>();
                if (hit != null && timeSinceAlertHit > alertCoolDownHit)
                {
                    if (hit.Instigator == gameObject) return;
                    //Debug.Log($"Hit other RPGCharactorController {hit.Instigator}");
                    charactorDamaging = hit.Instigator;
                    OnTakingDamage();

                }
                if (colliderObject.GetComponent<RPGCharactorController>() != null && timeSinceAlertBump > alertCoolDownBump)
                {
                    if (colliderObject.GetComponent<RPGCharactorController>().IsDead()) return;
                    //Debug.Log($"bumped other RPGCharactorController {colliderObject.name}");
                    charactorBumped = colliderObject;
                    OnBumping();
                }
            }
            catch
            {
            }
        }

        private List<Collider> ColliderList()
        {
            List<Collider> collidersList = new List<Collider>();
            foreach (TargetCollider target in colliders)
            {                
                collidersList.Add(target.Collider);
            }
            return collidersList;
        }

        private TargetCollider RandomTarget(List<TargetCollider> targets)
        {
            System.Random rand = new System.Random();
            return targets[rand.Next(targets.Count)];
        }

        public Transform GetRandomTarget()
        {
            return RandomTarget(colliders).Collider.transform;
        }

        public Transform GetTargetOfType(BodyPartType bodyPart)
        {
            List<TargetCollider> targetColliders = new List<TargetCollider>();
            foreach (TargetCollider target in colliders)
            {
                if (target.BodyPart != bodyPart) continue;
                targetColliders.Add(target);
            }
            return RandomTarget(targetColliders).Collider.transform;
        }

        public void CollidersEnabled(bool on)
        {
            isActive = on;
            triggerCollider.enabled = on;
            foreach (TargetCollider target in colliders)
            {
                try
                {
                    target.Collider.enabled = on;
                }
                catch
                {
                    Debug.Log($"Colider could not be turned {on}");
                }                
            }
        }

    }
}
