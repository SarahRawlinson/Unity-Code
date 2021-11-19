using UnityEngine;
using System.Collections;

public class MoveForwardBackwards : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float timeInEachState;
    public float timer = 0f;
    public bool forwardDirection = true;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {        
        if (forwardDirection)
        {
            transform.position += Vector3.forward * Time.deltaTime * movementSpeed;
            if (timer > timeInEachState)
            {
                forwardDirection = false;
                timer = 0f;
            }
        }
        else
        {
            transform.position -= Vector3.forward * Time.deltaTime * movementSpeed;
            if (timer > timeInEachState)
            {
                forwardDirection = true;
                timer = 0f;
            }
        }
        timer += Time.deltaTime;
    }
}
