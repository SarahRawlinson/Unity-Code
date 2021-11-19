using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Stats;

namespace RPG.PickUp
{
    public class HealthPickUp : PickUp, IActivate
    {
        [SerializeField] float health;
        [SerializeField] float deactivateForSecons = 60f;
        public SetActive activate;
        
        private void Update()
        {
            if (Active && !PickUpComplete)
            {
                Activated(this);
                Deactivated(this);
            }
        }
        public void Activate()
        {
            Player.GetComponent<Health>().Heal(health);
            Player.GetComponent<Experience>().HealthFX();
        }

        public void Deactivate()
        {
            StartCoroutine(DeactivateForSeconds(deactivateForSecons));
        }

    }
}

