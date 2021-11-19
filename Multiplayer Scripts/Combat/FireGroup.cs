using Mirror;
using UnityEngine;

namespace MultiplayerRTS.Combat
{
    public partial class Weapon
    {
        [System.Serializable]
        public class FireGroup
        {
            [SerializeField] Transform[] firePoints = null;

            public Transform[] FirePoints
            {
                get => firePoints;
            }

            private float lastFireTime;
            private int lastFire = -1;
            private bool isAbleToFire = true;

            public bool GetNextFirePosition(float fireRate, CombatTarget_RTS target,
                NetworkConnection connectionToClient, GameObject projectilePrefab, int damage)
            {
                if (Time.time > (1 / fireRate) + lastFireTime)
                {
                    isAbleToFire = true;
                }

                if (!isAbleToFire) return false;
                if (lastFire < 0 || lastFire >= (FirePoints.Length - 1))
                {
                    lastFire = 0;
                }
                else
                {
                    lastFire += 1;
                }

                lastFireTime = Time.time;
                isAbleToFire = false;

                FirePointFire(target, connectionToClient, firePoints[lastFire].position, projectilePrefab, damage);
                return true;
            }

            private void FirePointFire(CombatTarget_RTS target, NetworkConnection connectionToClient,
                Vector3 nextFirePos, GameObject projectilePrefab, int damage)
            {
                Quaternion projectileRotation =
                    Quaternion.LookRotation(target.AimAtPoint(nextFirePos).position - nextFirePos);
                GameObject projectileInstance = Instantiate(projectilePrefab, nextFirePos, projectileRotation);
                if (projectileInstance.TryGetComponent(out CombatProjectile projectile))
                {
                    projectile.HitDamage = damage;
                }

                NetworkServer.Spawn(projectileInstance, connectionToClient);
            }
        }
    }
}