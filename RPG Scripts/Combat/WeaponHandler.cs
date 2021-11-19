using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace RPG.Combat
{
    [Serializable]
    public class WeaponHandler 
    {
        private GameObject activeWeaponObject = null;
        private Weapon activeWeapon = null;
        private Transform weaponTransform;
        //private Transform aimTarget;
        private Animator playerAnimator;
        private GameObject instigator;
        private Transform rightHand;
        private Transform leftHand;
        private CombatTarget combatTarget;
        private Transform target;
        private WeaponHit hit;
        [Range(0, 1)]
        private float damageModifior = 1f;
        public float DamageModifior { get => damageModifior; set => SetDamageModifior(value); }
        private float damageAdditional = 0f;
        public float DamageAdditional { get => damageAdditional; set => SetDamageAdditional(value); }
        public float WeaponDamage { get => GetWeaponDamage(); }

        private float GetWeaponDamage()
        {
            if (activeWeapon != null)
            {
                return activeWeapon.Damage;
            }
            return 0f;
        }

        private void SetDamageModifior(float modifior)
        {
            if (modifior <=1 && modifior >= 0)
            {
                damageModifior = modifior;                
            }
            hit.HitDammage = GetDamageHit();
        }

        private void SetDamageAdditional(float additional)
        {
            damageAdditional = additional;
            try
            {
                hit.HitDammage = GetDamageHit();
            }
            catch { }
        }

        public WeaponHandler(Transform rightHandTransform, Transform leftHandTransform, Animator animator, GameObject caller)
        {
            instigator = caller;
            playerAnimator = animator;
            leftHand = leftHandTransform;
            rightHand = rightHandTransform;
        }

        public void ResetHandler()
        {
            if (activeWeapon != null) activeWeapon.DestroyObject(activeWeaponObject);
            activeWeapon = null;            
        }

        public float GetDamageHit()
        {
            return (damageAdditional) * DamageModifior;
        }

        public IEnumerator NewWeapon(Weapon weapon, string Caller)
        {
            //Debug.Log($"New Weapon Called by {Caller}");
            Weapon origionalWeapon = null;
            Vector3 curentPosition = new Vector3();
            bool weaponShouldDrop = false;
            if (activeWeapon != null)
            {
                origionalWeapon = activeWeapon;
                curentPosition = activeWeaponObject.transform.position;
                weaponShouldDrop = DropWeapon();                
            }
            activeWeapon = weapon;
            Spawn();
            //Debug.Log("Weapon Spawned");
            if (weaponShouldDrop)
            {
                yield return CreatePickUp(5, origionalWeapon, curentPosition);
            }
            yield return null;
        }
        
        public void CancelAttack()
        {
            if (hit != null) hit.Activate(false);
        }

        public bool DropWeapon()
        {
            if (activeWeaponObject == null) return false;
            activeWeapon.DestroyObject(activeWeaponObject);
            return true;
            //Debug.Log(activeWeapon.name);
            //if (activeWeapon.DropPickUp != null)
            //{
            //    try
            //    {
            //        activeWeapon.CreatePickUp(activeWeaponObject.transform);
            //    }
            //    catch (Exception E)
            //    {
            //        Debug.Log($"{E.Message} : {activeWeapon}");
            //    }                
            //}
             
        }
        
        public IEnumerator CreatePickUp(float waitSeconds, Weapon weapon, Vector3 position)
        {
            Debug.Log("Create Pickup Called");
            yield return new WaitForSeconds(waitSeconds);
            try
            {
                weapon.CreatePickUp(position);
                Debug.Log("Create Pickup complete");
            }
            catch (Exception E)
            {
                Debug.Log($"{E.Message} : {activeWeapon}");
            }
        }

        public void Activate(CombatTarget targetEntity)
        {
            combatTarget = targetEntity;
            
            if (!targetEntity.Colliders.isActive) return;
            
            target = targetEntity.Colliders.GetRandomTarget();            
            if (hit != null) hit.Activate(true);
            if (activeWeapon.Projectile == null)
            {
                return;
            }
            CreateProjectile();
        }

        public void CreateProjectile()
        {
            Transform desiredTransform;
            if (activeWeaponObject != null) desiredTransform = activeWeaponObject.transform;
            else desiredTransform = weaponTransform;
            Projectile newProjectile = activeWeapon.CreateProjectile(desiredTransform);
            newProjectile.enabled = true;
            newProjectile.GoToTarget(activeWeapon.ProjectileSpeed, GetDamageHit(), instigator, this);
        }        

        public GameObject Spawn()
        {
            weaponTransform = activeWeapon.WeaponTransform(rightHand, leftHand);
            if (playerAnimator != null) playerAnimator.runtimeAnimatorController = activeWeapon.WeaponOverRideAnimator;
            else
            {
                var overideController = playerAnimator.runtimeAnimatorController as AnimatorOverrideController;
                if (overideController != null)
                {
                    playerAnimator.runtimeAnimatorController = overideController.runtimeAnimatorController;
                }
            }
            if (activeWeapon.WeaponPrefab != null)
            {
                activeWeaponObject = activeWeapon.CreateWeapon(weaponTransform);
                try
                {
                    hit = activeWeaponObject.GetComponent<WeaponHit>();
                    if (hit != null) hit.SetDamage(GetDamageHit(), instigator, activeWeapon.TimeBetweenAttack);
                }
                catch (Exception E)
                {
                    Debug.Log(E.Message);
                }
                return activeWeaponObject;
            }
            return null;
        }

        public Vector3 GetAimLocation()
        {
            //Debug.Log(combatTarget.name);
            if (!combatTarget.Colliders.isActive) return combatTarget.GetComponent<Rigidbody>().centerOfMass;
            Collider targetCollider = target.GetComponent<Collider>();
            return targetCollider.bounds.center;
        }

    }
}
