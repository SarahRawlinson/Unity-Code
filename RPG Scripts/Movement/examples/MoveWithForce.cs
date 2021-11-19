using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    public class MoveWithForce : MonoBehaviour
    {
        private Rigidbody rigidbodyComponent;
        [SerializeField] private float movementForce = 10f;

        private void Awake() => rigidbodyComponent = GetComponent<Rigidbody>();
        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.W)) rigidbodyComponent.AddForce(movementForce * Vector3.forward);
            if (Input.GetKey(KeyCode.S)) rigidbodyComponent.AddForce(movementForce * Vector3.back);
            if (Input.GetKey(KeyCode.D)) rigidbodyComponent.AddForce(movementForce * Vector3.right);
            if (Input.GetKey(KeyCode.A)) rigidbodyComponent.AddForce(movementForce * Vector3.left);
        }
    }
}
