using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RPG.Core;
using RPG.States;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BaseStats))]
    public class Health : MonoBehaviour, ISaveable, IStat
    {
        [Range(0f, 1f)]
        [SerializeField] float RegenPercent = 1f;
        private float startHealth = -5;

        public float healthStat;
        private BaseStats baseStats;
        private bool dead { get => IsDead(); }
        private bool alreadyDead = false;
        public event Action<float> OnDamageTaken;
        public float damageRecieved = 0f;
        public UnityEvent OnDeath;
        public UnityEvent OnDamage;
        public UnityEvent OnHeal;
        public event Action onStatChange;
        public event Action<Transform> onBeingAttacked;

        private float health { get => healthStat;/* set => healthStat = value; */}
        public float StartHealth { get => startHealth;/* set => startHealth = value; */}

        //public void Awake()
        //{
        //    GetStartHealth();
            
        //}

        public void Start()
        {
            GetStartHealth();
            if (!dead) FullHealth();
            baseStats = GetComponent<BaseStats>();
            if (baseStats != null)
            {
                baseStats.onLevelUp += GetStartHealth;
                baseStats.onModifierUpdate += GetStartHealth;
                baseStats.onLevelUp += RegenarateHealth;
                baseStats.onLoad += GetStartHealth;
            }
            onStatChange();
        }

        public void Heal(float increaseInHealth)
        {            
            GiveHealth(increaseInHealth);
            onStatChange();
            OnHeal.Invoke();
        }

        private void GiveHealth(float increaseInHealth)
        {
            GetStartHealth();
            if (healthStat + increaseInHealth >= startHealth)
            {
                FullHealth();
            }
            else
            {
                healthStat += increaseInHealth;
            }
            onStatChange();
        }

        private void RegenarateHealth()
        {
            float healthTarget = startHealth * RegenPercent;
            GiveHealth(healthTarget);
            onStatChange();
        }

        private void FullHealth()
        {
            healthStat = startHealth;
            onStatChange();
        }

        private void GetStartHealth()
        {
            startHealth = GetComponent<BaseStats>().GetStatFloat(Stats.Health);
            //Debug.Log($"{gameObject.name} {startHealth}");
        }

        private void Update()
        {

        }

        public void TakeDammage(float loss, GameObject attacker)
        {            
            if (loss < healthStat)
            {
                healthStat -= loss;
                damageRecieved = loss;
                OnDamage.Invoke();
                onBeingAttacked(attacker.transform);
            }
            else
            {
                damageRecieved = healthStat;
                healthStat = 0;
                GiveXP(attacker);
            }
            OnDamageTaken(damageRecieved);
            onStatChange();
            //if (gameObject.tag != "Player" ) Debug.Log(gameObject.name + " has taken " + loss + " dammage and has " + healthStat + " left of health" );
        }

        private void GiveXP(GameObject attacker)
        {
            Experience experience = attacker.GetComponent<Experience>();
            if (experience != null)
            {
                experience.AwardFightXP(10, true);
            }
        }

        public bool IsDead()
        {
            //Debug.Log($"{gameObject.name} health of {healthStat} and dead = {dead || healthStat <= 0}");
            if (healthStat <= 0)
            {
                if (!alreadyDead)
                {
                    alreadyDead = true;
                    GetComponent<Rigidbody>().isKinematic = true;
                    GetComponent<Rigidbody>().useGravity = false;
                    OnDeath.Invoke();
                    onStatChange();
                }
                return true;
            }
            return false;
        }

        public object CaptureState()
        {
            //Debug.Log($"{gameObject.name} Health Saved at {healthStat}");
            return healthStat;
        }

        public void RestoreState(object state)
        {
            healthStat = (float)state;
            //Debug.Log($"{gameObject.name} health has been restored {healthStat} = {(float)state}");
            if (!dead) GetComponent<Death>().OnDeath(dead);
        }

        float IStat.PercentLeft()
        {
            GetStartHealth();
            //Debug.Log($"{string.Format("{0:0.0}%", (healthStat / startHealth) * 100)} current health({healthStat}) / start health({startHealth})");
            return healthStat / startHealth;
        }

        float IStat.CurrentValue()
        {
            return healthStat;
        }

        float IStat.StartValue()
        {
            return startHealth;
        }
    }
}

