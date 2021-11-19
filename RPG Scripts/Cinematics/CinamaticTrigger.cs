using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinamaticTrigger : MonoBehaviour
    {
        bool Triggered = false;
        private void OnTriggerEnter(Collider other)
        {   
            if (!Triggered && other.gameObject.tag == "Player")
            {
                Triggered = true;
                GetComponent<PlayableDirector>().Play();
            }
            
        }
    }
}
