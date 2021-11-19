using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    public class MoveTransformPositionWithInputSmoothly : MonoBehaviour
    {
        [SerializeField] private float speed = 2f;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.D) && transform.position.x < 5)
                transform.position += Time.deltaTime * speed * Vector3.right;

            if (Input.GetKey(KeyCode.A) && transform.position.x > -5)
                transform.position += Time.deltaTime * speed * Vector3.left;

            if (Input.GetKey(KeyCode.W) && transform.position.z < 5)
                transform.position += Time.deltaTime * speed * Vector3.forward;

            if (Input.GetKey(KeyCode.S) && transform.position.z > -5)
                transform.position += Time.deltaTime * speed * Vector3.back;

            if (Input.GetKey(KeyCode.Space) && transform.position.y < 5)
                transform.position += Time.deltaTime * speed * new Vector3(0, 0.5f, 0);

            if (Input.GetKey(KeyCode.C) && transform.position.y > -5)
                transform.position += Time.deltaTime * speed * new Vector3(0, -0.5f, 0);

            if (Input.GetKeyDown(KeyCode.Delete))
                transform.position = new Vector3(0, 0, 0);
        }
    }
}