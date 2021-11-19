using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    [DisallowMultipleComponent]
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] [Range(0.01f, 1.0f)] float smoothSpeed = 0.125f;
        //[SerializeField] float turnSpeed = 0.1f;
        //[SerializeField] Vector3 offset;
        //[SerializeField] Vector3 rotation = new Vector3(0.0f, 1.0f, 0.0f);
        //[SerializeField] bool lookAtPlayer = true;
        // Update is called once per frame
        public void Start()
        {
            
        }
        public void LateUpdate()
        {

            Vector3 newPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
            Vector3 desiredPosition = newPosition;
            Vector3 smoothedPosition = Vector3.Slerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            //if (Input.GetKey(KeyCode.RightArrow))
            //{
            //    transform.RotateAround(target.position, Vector3.up, 20 * turnSpeed);
            //}
            //else if (Input.GetKey(KeyCode.LeftArrow))
            //{
            //    transform.RotateAround(target.position, Vector3.up, -20 * turnSpeed);
            //}
            //else if (lookAtPlayer) transform.LookAt(target);

        }
    }
}
