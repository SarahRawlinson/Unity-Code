using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

namespace MultiplayerRTS.Combat
{
    public class CombatFiring : NetworkBehaviour
    {
        [SerializeField] private CombatFighter_RTS fighter = null;
        [SerializeField] private Weapon[] weapons;
        [SerializeField] float rotaionSpeed = 20f;
        [SerializeField] private LayerMask viewLayerMask = new LayerMask();
        [SerializeField] private bool rotateTowardsTarget = true;
        public Weapon[] Weapons { get => weapons; set => weapons = value; }
        [SyncVar] private Vector3 TargetPosition = new Vector3();
        [SyncVar] private bool _hasTarget = false;


        
        private void Update()
        {
            if (isServer)
            {
                TargetCombatFighterTarget();
                foreach (Weapon weapon in weapons)
                {
                    weapon.WorkOutTime(Time.deltaTime);
                }
            }

            if (isClient)
            {
                WeaponTargeting();
            }
            
        }

        [Client]
        private void WeaponTargeting()
        {
            if (!_hasTarget) return;
            foreach (Weapon weapon in weapons)
            {
                weapon.RotateTowardsTarget(TargetPosition);
            }
        }

        public CombatTarget_RTS TargetInRange()
        {
            CombatTarget_RTS target = null;
            foreach (Weapon weapon in weapons)
            {
                Collider[] colliders;
                try
                {
                    colliders =
                        Physics.OverlapSphere(weapon.transform.position, weapon.FireRange, viewLayerMask);
                }
                catch (Exception e)
                {
                    Debug.Log($"{e.Message} : {e.StackTrace}");
                    return target;
                }
                foreach (Collider collider in colliders)
                {
                    if (collider.attachedRigidbody == null) continue;
                    if (!collider.attachedRigidbody.TryGetComponent(out CombatTarget_RTS colliderTarget)) continue;
                    if (colliderTarget.connectionToClient.connectionId == connectionToClient.connectionId) continue;
                    target = colliderTarget;
                    break;
                }
                if (target == null) return target;
                if (!weapon.TryTarget(target)) continue;
                try
                {
                    weapon.FireWeapon(target, connectionToClient);
                }
                catch (Exception e)
                {
                    Debug.Log($"could not fire weapon: {e.StackTrace}", gameObject);
                }
            }
            return target;
        }

        private void TargetCombatFighterTarget()
        {
            if (fighter == null) return;
            CombatTarget_RTS target = fighter.Target;
            if (target == null)
            {
                _hasTarget = false;
                return;
            }
            else
            {
                _hasTarget = true;
                TargetPosition = fighter.Target.transform.position;
            }
            if (rotateTowardsTarget)
            {
                Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 
                    rotaionSpeed * Time.deltaTime);
            }
            //Debug.Log("Checking Weapons");
            Target(target);
        }

        private void Target(CombatTarget_RTS target)
        {
            foreach (Weapon weapon in weapons)
            {
                if (!CanFireAtTarget(weapon))
                {
                    //Debug.Log("Not Close Enough To Fire");
                    continue;
                }

                if (!weapon.TryTarget(target))
                {
                    continue;
                }

                try
                {
                    weapon.FireWeapon(target, connectionToClient);
                }
                catch (Exception e)
                {
                    Debug.Log($"could not fire weapon: {e.StackTrace}", gameObject);
                }
            }
        }


        [Server]
        private bool CanFireAtTarget(Weapon weapon)
        {
            return (fighter.Target.transform.position - transform.position).sqrMagnitude <= weapon.FireRange * weapon.FireRange;
        }
    }
}
