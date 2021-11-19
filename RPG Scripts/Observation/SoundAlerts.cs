using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;

namespace RPG.Observation
{
    [DisallowMultipleComponent]
    public class SoundAlerts : MonoBehaviour
    {
        private bool noiseActive = false;
        private int activeNoises = 0;
        [SerializeField] float attckTime = 30f;
        [SerializeField] float deathTime = 60f;
        [SerializeField] float MoveTime = 20f;
        //[SerializeField] float alertTime = 10f;

        public bool NoiseActive { get => noiseActive; set => noiseActive = value; }

        // Start is called before the first frame update
        void Start()
        {
            RPGCharactorController charactorController = null;
            charactorController = GetComponent<RPGCharactorController>();
            if (charactorController != null)
            {
                charactorController.onFighting += Attack;
                charactorController.onDeath += Death;
                charactorController.onMoving += Move;
                charactorController.onIdle += Idle;
                charactorController.onAlert += Alert;
            }
        }

        public void Alert()
        {

        }

        public void Move()
        {
            MakeSound(MoveTime);
        }

        public void Idle()
        {

        }

        public void Attack()
        {
            MakeSound(attckTime);
        }

        public void Death()
        {
            MakeSound(deathTime);
        }

        public void MakeSound(float time)
        {
            activeNoises += 1;
            noiseActive = true;
            StartCoroutine(EndSound(time));
        }

        IEnumerator EndSound(float EndTime)
        {
            yield return new WaitForSeconds(EndTime);
            TurnOffSound();
        }

        public void TurnOffSound()
        {
            if (activeNoises > 0) activeNoises -= 1;
            if (activeNoises == 0)
            {
                noiseActive = false;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
