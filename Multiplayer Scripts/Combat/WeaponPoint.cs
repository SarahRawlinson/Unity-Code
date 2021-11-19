using System;
using Mirror;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MultiplayerRTS.Combat
{
    public class WeaponPoint : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;
        private bool active = false;
        public Weapon activeWeapon;

        public bool IsActive()
        {
            return active;
        }
        public Weapon GetWeapon()
        {
            return _weapon;
        }
        
        [Server]
        public void SetWeapon(Weapon weapon)
        {
            _weapon = weapon;
        }
        
        [Server]
        public GameObject CreateWeapon()
        {
            active = true;
            return _weapon.gameObject;
        }
    }
}