using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Sound;

namespace RPG.FX
{

    [CreateAssetMenu(fileName = "SoundFXHandler", menuName = "RPG Project/FX/New Sound FX", order = 1)]
    public class SoundFXHandler : ScriptableObject
    {
        //[SerializeField] GameObject fx;
        [SerializeField] float aliveTime = 20f;
        [SerializeField] bool loop = false;
        [SerializeField] SoundFX.FXSounds sounds;

        public bool Loop { get => loop;}

        public AudioClip GetAudioClip()
        {
            return sounds.GetRandomClip();
        }

        public void CreateFX(Transform pos)
        {
            GameObject fxObject = Instantiate(new GameObject(), pos.position, Quaternion.identity);
            fxObject.AddComponent<AudioSource>().clip = GetAudioClip();
            AudioSource audio = fxObject.GetComponent<AudioSource>();
            audio.loop = loop;
            audio.spatialBlend = 1;
            audio.Play();
            fxObject.AddComponent(typeof(LifeTime));
            //Debug.Log("set lifetime");
            fxObject.GetComponent<LifeTime>().AliveTime = aliveTime;
            // not sure this will work, if start has been called before set
        }
    }
}
