using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundObject : MonoBehaviour
{
    [SerializeField] Transform trarget;
    [SerializeField] float distance = 1f;
    public enum RotationLR { Left, Right}
    [SerializeField] RotationLR rotationDirection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relitivePos = (trarget.position + new Vector3(0, .5f, 0)) - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relitivePos);

        Quaternion current = transform.localRotation;

        transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime);
        transform.Translate(0, 0, 3 * (Time.deltaTime * distance));
    }
}
