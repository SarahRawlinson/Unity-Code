using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    public class MoveWithForceUseTransformRotationClampVelocity : MonoBehaviour
    {
        private Rigidbody rigidbodyComponent;
        [SerializeField] private float movementForce = 10f;
        [SerializeField] private float maxVelocity = 10f;
        private bool shouldJump;

        private void Awake() => rigidbodyComponent = GetComponent<Rigidbody>();

        private void FixedUpdate()
        {
            if (rigidbodyComponent.velocity.magnitude >= maxVelocity) return;
            if (Input.GetKey(KeyCode.W)) rigidbodyComponent.AddForce(movementForce * transform.forward);
            if (Input.GetKey(KeyCode.S)) rigidbodyComponent.AddForce(movementForce * -transform.forward);
            if (Input.GetKey(KeyCode.D)) rigidbodyComponent.AddForce(movementForce * transform.right);
            if (Input.GetKey(KeyCode.A)) rigidbodyComponent.AddForce(movementForce * -transform.right);
        }
    }

}