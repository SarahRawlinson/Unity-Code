using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.FX
{

    [CreateAssetMenu(fileName = "FXHandler", menuName = "RPG Project/FX/New FX", order = 0)]
    public class FXHandler : ScriptableObject
    {
        [SerializeField] GameObject fx;
        [SerializeField] float aliveTime;

        public void CreateFX(Transform pos)
        {
            GameObject fxObject = Instantiate(fx, pos.position, Quaternion.identity);
            
            fxObject.AddComponent(typeof(LifeTime));
            //Debug.Log("set lifetime");
            fxObject.GetComponent<LifeTime>().AliveTime = aliveTime;
            // not sure this will work, if start has been called before set
            //return fxObject;
        }
    }
}
