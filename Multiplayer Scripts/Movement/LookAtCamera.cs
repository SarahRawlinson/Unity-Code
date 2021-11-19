using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform mainCameraTransform;
    // Start is called before the first frame update
    void Awake()
    {
        mainCameraTransform = Camera.main.transform;
    }

    // Update is called once per frame after update
    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward, mainCameraTransform.rotation * Vector3.up);
    }
}
