using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        private void Awake()
        {
            int numMusicPlayer = FindObjectsOfType<MusicPlayer>().Length;

            if (numMusicPlayer > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                // DontDestroyOnLoad (Unity)
                // https://docs.unity3d.com/ScriptReference/Object.DontDestroyOnLoad.html

                DontDestroyOnLoad(gameObject);
            }


        }


    }
}
