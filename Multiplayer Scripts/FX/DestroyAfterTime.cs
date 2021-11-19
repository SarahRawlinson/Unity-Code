using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using MultiplayerRTS.Networking;
using UnityEngine;

namespace MultiplayerRTS
{
    public class DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] private float destroyTime = 5;

        private void Start()
        {
            DestroyMe(destroyTime);
        }

        public void DestroyMe(float time)
        {
            Destroy(gameObject, time);
        }
        
    }
}
