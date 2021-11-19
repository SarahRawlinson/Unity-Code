using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Sound
{
    public class SoundFX : MonoBehaviour
    {
        public enum FXSound
        {
            Owch1,
            Dieing1,
            Coins1,
            WeaponClash1,
            Armour1,
            BookTurn1,
            BookTurn2,
            BookTurn3,
            Cards1,
            Jewel1,
            PositiveEffect1,
            SpecialClick1,
            UIClick1,
            PositiveEffect2,
            NegativeEffect1,
            FireSpell1,
            Punch1,
            Punch2,
            BodyFall

        }
        [System.Serializable]
        public class FXSounds
        {
            [SerializeField] FXSound fXSound;
            [SerializeField] AudioClip[] clips;
            public AudioClip[] Clips { get => clips; }
            public FXSound FXSound { get => fXSound; }
            public AudioClip GetRandomClip()
            {
                return clips[Random.Range(0, clips.Length)];
            }
        }
        [SerializeField] FXSounds[] sounds;
        public void PlaySound(FXSound soundType)
        {
            foreach (FXSounds sound in sounds)
            {
                if (sound.FXSound == soundType)
                {
                    GetComponent<AudioSource>().clip = sound.GetRandomClip();
                    GetComponent<AudioSource>().Play();
                    return;
                }
            }
        }
        public void PlayDeath()
        {
            PlaySound(FXSound.Dieing1);
        }
        public void PlayOwch()
        {
            PlaySound(FXSound.Owch1);
        }
        public void PlayPunch()
        {
            PlaySound(FXSound.Punch1);
        }
        public void PlayFireSpell()
        {
            PlaySound(FXSound.FireSpell1);
        }
        public void PlayPositiveEffect()
        {
            PlaySound(FXSound.PositiveEffect1);
        }
        public void PlayNegativeEffect()
        {
            PlaySound(FXSound.NegativeEffect1);
        }
        public void PlayWeaponClash()
        {
            PlaySound(FXSound.WeaponClash1);
        }
    }
}
