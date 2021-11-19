using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    public class CharacterMove : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 10f;
        [SerializeField] private float jumpSpeed = .5f;
        [SerializeField] private float gravity = 2f;
        CharacterController characterController;
        Vector3 movementDirection;

        private void Awake() => characterController = GetComponent<CharacterController>();

        private void FixedUpdate()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 inputDirection = new Vector3(horizontal, 0, vertical);
            Vector3 transformDirection = transform.TransformDirection(inputDirection);
            Vector3 flatMovement = movementSpeed * Time.deltaTime * transformDirection;
            movementDirection = new Vector3(flatMovement.x, movementDirection.y, flatMovement.z);

            if (PlayerJumped) movementDirection.y = jumpSpeed;
            else if (characterController.isGrounded) movementDirection.y = 0f;
            else movementDirection.y -= gravity * Time.deltaTime;
            characterController.Move(movementDirection);
        }
        private bool PlayerJumped => characterController.isGrounded && Input.GetKey(KeyCode.Space);
    }
}
