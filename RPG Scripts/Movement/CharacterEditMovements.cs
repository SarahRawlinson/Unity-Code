using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    public class CharacterEditMovements : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 50f;
        [SerializeField] AnimatorOverrideController[] controllers;
        // Start is called before the first frame update
        void Start()
        {

        }

        public void ActivateRandomDance()
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = controllers[UnityEngine.Random.Range((int)0, controllers.Length)];
        }
        public void StopDance()
        {
            var overideController = transform.GetComponent<Animator>().runtimeAnimatorController as AnimatorOverrideController;
            transform.GetComponent<Animator>().runtimeAnimatorController = overideController.runtimeAnimatorController;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Q))
                transform.Rotate(Time.deltaTime * rotationSpeed * Vector3.down);

            if (Input.GetKey(KeyCode.E))
                transform.Rotate(Time.deltaTime * rotationSpeed * Vector3.up);
        }
    }
}
