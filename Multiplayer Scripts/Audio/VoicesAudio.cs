using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace MultiplayerRTS.Audio
{
    public class VoicesAudio : MonoBehaviour, ActiveSound
    {
        [SerializeField] private Voice[] voices;
        [SerializeField] private GameObject[] playSoundEvents;
        private List<PlaySoundEvent> _playSoundEvents = new List<PlaySoundEvent>();
        [SerializeField] private AudioSource _audioSource;
        private AudioDirector _audioDirector;
        public event Action<ActiveSound> OnDeath;

        public void Start()
        {
            _audioDirector = FindObjectOfType<AudioDirector>();
            foreach (GameObject gObject in playSoundEvents)
            {
                PlaySoundEvent[] events = gObject.GetComponents<PlaySoundEvent>();
                foreach (PlaySoundEvent soundEvent in events)
                {
                    Debug.Log($"Sound Event Added");
                    _playSoundEvents.Add(soundEvent);
                }
            }
            foreach (PlaySoundEvent playSoundEvent in _playSoundEvents)
            {
                playSoundEvent.onPlaySound += HandleAudioChange;
                playSoundEvent.onDeath += HandleDeath;
            }
        }

        public GameObject ActiveSoundGameObject()
        {
            return gameObject;
        }

        public AudioSource GETAudioSource()
        {
            return _audioSource;
        }

        public bool HasAuthorityToPlay()
        {
            return true;
        }

        private void HandleAudioChange(AudioEvent audioEvent, int id)
        {
            if (_audioDirector == null)
            {
                Debug.Log("No Audio Director Found");
                return;
            }
            if (!_audioDirector.RequestPermissionToPlay(audioEvent, this))
            {
                return;
            }
            
            Debug.Log($"audio event called: {audioEvent.ToString()}");
            string nameOfVoice = "";
            List<VoiceSample> samples = new List<VoiceSample>();
            foreach (Voice voice in voices)
            {
                if (voice.voiceID == id)
                {
                    nameOfVoice = voice.voiceName;
                    foreach (VoiceSample sample in voice.samples)
                    {
                        if (sample.audioEvent == audioEvent)
                        {
                            samples.Add(sample);
                        }
                    }
                    
                }
            }
            if (samples.Count > 0)
            {
                VoiceSample sample = samples[Random.Range(0, samples.Count - 1)];
                _audioDirector.PlayingAudio(this, sample.voiceClip);
                _audioSource.clip = sample.voiceClip;
                Debug.Log($"{nameOfVoice} says '{sample.dialogue}'");
                _audioSource.Play();
            }
        }

        private void OnDestroy()
        {
            OnDeath?.Invoke(this);
        }

        private void HandleDeath(PlaySoundEvent soundEvent)
        {
            soundEvent.onPlaySound -= HandleAudioChange;
        }
    }
    
}
