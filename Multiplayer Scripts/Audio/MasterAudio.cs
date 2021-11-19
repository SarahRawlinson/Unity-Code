using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MultiplayerRTS.Audio
{
    public class MasterAudio : MonoBehaviour
    {
        private void Start()
        {
            MasterAudio[] objs = FindObjectsOfType<MasterAudio>();

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
            if (FindObjectsOfType<MasterAudio>().Length > 1)
            {
                Debug.Log("More Than One Audio Master");
                Destroy(gameObject);
            }
        }

    }
}
