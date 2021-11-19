using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;


namespace RPG.PickUp
{
    public class WeaponPickUp : PickUp, IActivate
    {
        //public new void Activate()
        //{
        //    Debug.Log(gameObject.name + " Activated");
        //}
        [SerializeField] Weapon weapon;
        public SetActive activate;
        private void Update()
        {
            if (Active && !PickUpComplete)
            {
                //// dont think this is needed just trying it out
                //activate = new SetActive(Activated);
                //activate.Invoke(this);
                //activate = new SetActive(Deactivated);
                //activate.Invoke(this);
                Activated(this);
                Deactivated(this);
            }
        }

        public void Activate()
        {
            //Debug.Log($"{Player.GetComponent<Fighter>().gameObject.name} now has {weapon.name}");
            Player.GetComponent<Fighter>().EquipWeapon(weapon, "Weapon PickUp");
            //Debug.Log("Activated");
        }

        public void Deactivate()
        {
            //Debug.Log("Deactivated");
            Destroy(gameObject);
        }
    }
}
