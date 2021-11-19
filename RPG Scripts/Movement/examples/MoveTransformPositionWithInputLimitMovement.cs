using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    public class MoveTransformPositionWithInputLimitMovement : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.D) && transform.position.x < 5)
                transform.position += Vector3.right;

            if (Input.GetKeyDown(KeyCode.A) && transform.position.x > -5)
                transform.position += Vector3.left;

            if (Input.GetKeyDown(KeyCode.W) && transform.position.z < 5)
                transform.position += Vector3.forward;

            if (Input.GetKeyDown(KeyCode.S) && transform.position.z > -5)
                transform.position += Vector3.back;

            if (Input.GetKeyDown(KeyCode.Space) && transform.position.y < 5)
                transform.position += new Vector3(0, 0.5f, 0);

            if (Input.GetKeyDown(KeyCode.C) && transform.position.y > -5)
                transform.position += new Vector3(0, -0.5f, 0);

            if (Input.GetKeyDown(KeyCode.Delete))
                transform.position = new Vector3(0, 0, 0);
        }
    }
}