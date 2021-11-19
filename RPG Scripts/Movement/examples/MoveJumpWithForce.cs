using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    public class MoveJumpWithForce : MonoBehaviour
    {
        private Rigidbody rigidbodyComponent;
        [SerializeField] private float jumpForce = 300f;
        private bool shouldJump;

        private void Awake() => rigidbodyComponent = GetComponent<Rigidbody>();

        private void Update()
        {
            if (shouldJump == false) shouldJump = Input.GetKeyDown(KeyCode.Space);
        }

        private void FixedUpdate()
        {
            if (shouldJump)
            {
                rigidbodyComponent.AddForce(jumpForce * Vector3.up);
                shouldJump = false;
            }
        }
    }
}
