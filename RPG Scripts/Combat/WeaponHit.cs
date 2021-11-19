using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RPG.Stats;
using System;
using RPG.FX;

namespace RPG.Combat
{
    public class WeaponHit : MonoBehaviour
    {
        //[SerializeField] GameObject CollideFX;
        [SerializeField] FXHandler fXHandler;
        private float coolDownBeforeNextHit = 10f;
        private GameObject instigator;
        private float hitDammage;
        private float timeSinceLastHit;
        private bool activeted = false;
        public UnityEvent OnHit;

        public float HitDammage { get => hitDammage; set => hitDammage = value; }
        public GameObject Instigator { get => instigator;}

        private void Update()
        {
            timeSinceLastHit += Time.deltaTime;
        }

        public void SetDamage(float damage, GameObject attacker, float timeBetweenAttack)
        {
            hitDammage = damage;
            instigator = attacker;
            coolDownBeforeNextHit = timeBetweenAttack;
            timeSinceLastHit = timeBetweenAttack;
            //Debug.Log($"{gameObject.name} has been created for {instigator.name}");
        }
        public void Activate(bool on)
        {
            activeted = on;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!activeted) return;
            if (timeSinceLastHit < coolDownBeforeNextHit) return;
            bool activateFX = false;
            Rigidbody rigidbody = null;
            try
            {
                rigidbody = other.attachedRigidbody;
                //Debug.Log($"Hit! {other.attachedRigidbody.name}");
            }
            catch (Exception E)
            {
                Debug.LogError("Error caught : " + E.Message);
                return;
            }
            if (rigidbody == null) return;                      
            try
            {
                Health targetHealth = rigidbody.gameObject.GetComponent<Health>();
                if (targetHealth == instigator.GetComponent<Health>()) return;
                if (targetHealth != null)
                {
                    activateFX = true;
                    targetHealth.TakeDammage(hitDammage, instigator);
                    timeSinceLastHit = 0;
                    //Debug.Log($"{other.name} has taken a hit of {hitDammage} from {instigator.name}");
                }
            }
            catch (Exception e)
            {
                Debug.Log($"{rigidbody.gameObject.name} could not load health : {e.StackTrace}");
                return;
            }
            try
            {
                if (fXHandler != null && activateFX)
                {
                    //GameObject fx = Instantiate(CollideFX, other.transform.position, Quaternion.identity);
                    //fx.transform.parent = gameObject.transform;
                    fXHandler.CreateFX(gameObject.transform);
                    OnHit.Invoke();
                }
            }
            catch (Exception E)
            {
                Debug.LogError(E.Message);
            }
        }
    }
}
