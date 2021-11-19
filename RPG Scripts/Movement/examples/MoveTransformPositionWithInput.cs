using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    public class MoveTransformPositionWithInput : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
                transform.position += Vector3.right;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                transform.position += Vector3.left;

            if (Input.GetKeyDown(KeyCode.UpArrow))
                transform.position += Vector3.forward;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                transform.position += Vector3.back;

            if (Input.GetKeyDown(KeyCode.Space))
                transform.position += new Vector3(0, 0.5f, 0);

            if (Input.GetKeyDown(KeyCode.LeftShift))
                transform.position += new Vector3(0, -0.5f, 0);

            if (Input.GetKeyDown(KeyCode.Delete))
                transform.position = new Vector3(0, 0, 0);
        }
    }
}
