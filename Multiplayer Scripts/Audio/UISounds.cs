using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace MultiplayerRTS
{
    public class UISounds : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip connected;
        [SerializeField] private AudioClip join;
        [SerializeField] private AudioClip exit;
        [SerializeField] private AudioClip startMatch;

        public void PlayConnected()
        {
            PlayClip(connected);
        }
        
        public void PlayJoined()
        {
            PlayClip(join);
        }
        
        public void PlayExit()
        {
            PlayClip(exit);
        }
        
        public void PlayStartMatch()
        {
            PlayClip(startMatch);
        }
        
        private void PlayClip(AudioClip audio)
        {
            audioSource.clip = audio;
            audioSource.Play();
        }
    }
}
