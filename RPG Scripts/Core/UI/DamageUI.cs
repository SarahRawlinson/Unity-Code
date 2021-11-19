using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;
using UnityEngine.UI;

namespace RPG.Core.UI
{
    public class DamageUI : MonoBehaviour
    {
        [SerializeField] Text damageText;
        [SerializeField] Health entityHealth;
        private Animation animationComponant;
        void Start()
        {
            animationComponant = GetComponent<Animation>();
            entityHealth.OnDamageTaken += TakeDamage;
        }
        public void TakeDamage(float damage)
        {
            animationComponant.Stop();
            //float damage = entityHealth.damageRecieved;
            damageText.text = Mathf.Round(damage).ToString();
            animationComponant.Play();            
        }
    }
}
