using UnityEngine;
using RPG.PickUp;
using System.Collections;


namespace RPG.Combat
{
    
    [CreateAssetMenu(fileName = "Weapon", menuName = "RPG Project/Weapon/New Weapon", order = 0)]
    
    public class Weapon : ScriptableObject
    {
        
        [Range(0, 1)]
        [SerializeField] float attackSpeedFraction = 0f;        
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttack = 1f;
        [SerializeField] float damage = 2f;
        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimatorOverrideController weaponOverRideAnimator;
        public enum Hand { Right, Left, Both }
        [SerializeField] Hand weaponHand;
        [SerializeField] Projectile projectile;
        [SerializeField] float projectileSpeed = .5f;        
        [SerializeField] WeaponPickUp dropPickUp;
        [SerializeField] float waitToSpawnParticle = 0f;       

        public Hand WeaponHand { get => weaponHand;}
        public float WaitToSpawnParticle { get => waitToSpawnParticle;}        
        public GameObject WeaponPrefab { get => weaponPrefab; }
        public WeaponPickUp DropPickUp { get => dropPickUp;}
        public Projectile Projectile { get => projectile;}
        public float ProjectileSpeed { get => projectileSpeed;}
        public float Damage { get => damage;}
        public AnimatorOverrideController WeaponOverRideAnimator { get => weaponOverRideAnimator;}
        public float TimeBetweenAttack { get => timeBetweenAttack; }

        public Transform WeaponTransform(Transform rightHand, Transform leftHand)
        {
            switch (weaponHand)
            {
                case Hand.Right: return rightHand;
                case Hand.Left: return leftHand;
                default: return rightHand;
            }
        }

        public float GetRange()
        {
            return weaponRange;
        }

        public float GetTimeBetweenAttack()
        {
            return timeBetweenAttack;
        }

        public float GetDammage()
        {
            return damage;
        }

        public float GetAttackSpeedFraction()
        {
            return attackSpeedFraction;
        } 
        
        public WeaponPickUp CreatePickUp(Vector3 position)
        {
            return Instantiate(
                    dropPickUp,
                    position + Vector3.forward * position.z / 40,
                    Quaternion.identity);
        }

        public void DestroyObject(GameObject activeWeaponObject)
        {
            Destroy(activeWeaponObject);
        }
        
        public Projectile CreateProjectile(Transform desiredTransform)
        {
            return Instantiate(
                projectile,
                desiredTransform.position,
                Quaternion.identity);            
        }

        public GameObject CreateWeapon(Transform weaponTransform)
        {
            return Instantiate(weaponPrefab, weaponTransform);
        }

    }
}
