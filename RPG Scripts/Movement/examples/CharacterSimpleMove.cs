using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    public class CharacterSimpleMove : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 10f;
        CharacterController characterController;
        public bool isGrounded;

        private void Awake() => characterController = GetComponent<CharacterController>();

        private void FixedUpdate()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 direction = new Vector3(horizontal, 0, vertical);
            Vector3 movement = transform.TransformDirection(direction) * movementSpeed;
            isGrounded = characterController.SimpleMove(movement);
        }
    }
}
