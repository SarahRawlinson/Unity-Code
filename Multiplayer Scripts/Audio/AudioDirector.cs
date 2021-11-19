using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

namespace MultiplayerRTS.Audio
{
    public class AudioDirector : MonoBehaviour
    {
        private List<ActiveSound> _voicesAudios = new List<ActiveSound>();

        [SerializeField] private int numberOfVoicesActive = 5;
        private bool audioOn = true;

        public void SetAudioOn(bool on)
        {
            audioOn = on;
        }
        public bool RequestPermissionToPlay(AudioEvent audioEvent, ActiveSound activeSound)
        {
            if (activeSound.GETAudioSource().isPlaying) return false;
            if (!audioOn || !activeSound.HasAuthorityToPlay()) return false;
            if (_voicesAudios.Count > numberOfVoicesActive)
            {
                // //TODO add in priority to certain events
                // switch (audioEvent)
                // {
                //     case AudioEvent.Attack:
                //         break;
                //     case AudioEvent.Death:
                //         break;
                //     case AudioEvent.Defend:
                //         break;
                //     case AudioEvent.AtEase:
                //         break;
                //     case AudioEvent.FindTarget:
                //         break;
                //     case AudioEvent.GivenTarget:
                //         break;
                //     case AudioEvent.KillConfirmed:
                //         break;
                //     case AudioEvent.MoveOut:
                //         break;
                //     case AudioEvent.TargetFound:
                //         break;
                //     case AudioEvent.FailedToTarget:
                //         break;
                //     default:
                //         return false;
                // }
                //
                return false;
            }
            return AudioWithinRange(activeSound);
        }

        private bool AudioWithinRange(ActiveSound activeSound)
        {
            AudioListener listener = FindObjectOfType<AudioListener>();
            if (Vector3.Distance(activeSound.ActiveSoundGameObject().transform.position, listener.transform.position) >
                activeSound.GETAudioSource().maxDistance) return false;
            _voicesAudios.Add(activeSound);
            activeSound.OnDeath += RemoveVoice;
            return true;
        }

        public void PlayingAudio(ActiveSound activeSound, AudioClip clip)
        {
            StartCoroutine(AudioHasStoppedIn(clip.length, activeSound));
        }

        IEnumerator AudioHasStoppedIn(float time, ActiveSound activeSound)
        {
            yield return new WaitForSeconds(time);
            if (_voicesAudios.Contains(activeSound))
            {
                RemoveVoice(activeSound);
            }
        }

        private void RemoveVoice(ActiveSound activeSound)
        {
            activeSound.OnDeath -= RemoveVoice;
            _voicesAudios.Remove(activeSound);
        }
    }
}
