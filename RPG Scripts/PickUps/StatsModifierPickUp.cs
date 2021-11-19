using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.PickUp
{
    public class StatsModifierPickUp : PickUp, IActivate
    {
        [System.Serializable]
        public class Mod
        {
            public Stats.Stats stat;
            public float add;
            public float percentIncrease;
            public float time;
        }
        [SerializeField] bool destroyOnDeactivate = false;
        [SerializeField] float deactivateForSeconds = 60f;
        [SerializeField] GameObject effect;
        [SerializeField] Mod[] mods;
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
            //Player.GetComponent<Health>().Heal(health);
            foreach (Mod mod in mods)
            {
                if (mod.add != 0)
                {
                    AddativeStat(mod.stat, mod.add, mod.time);
                }
                if (mod.percentIncrease != 0)
                {
                    PercentageStat(mod.stat, mod.percentIncrease, mod.time);
                }
            }
            if (effect != null)
            {
                GameObject obj = Instantiate(effect, Player.transform);                
                Destroy(obj, 5f);
            }
        }

        public void Deactivate()
        {
            if (destroyOnDeactivate)
            {
                Destroy(gameObject);
            }
            StartCoroutine(DeactivateForSeconds(deactivateForSeconds));
        }

    }
}
