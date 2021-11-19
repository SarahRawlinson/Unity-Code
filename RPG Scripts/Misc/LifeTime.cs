using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    [SerializeField] float lifeTime = 50f;

    public float AliveTime { get => lifeTime; set => lifeTime = value; }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("lifetime set");
        Invoke("DestroyThis", lifeTime);
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
    }
}
