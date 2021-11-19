using System;
using UnityEngine;

namespace MultiplayerRTS.Audio
{
    public interface ActiveSound
    {
        event Action<ActiveSound> OnDeath;
        GameObject ActiveSoundGameObject();
        AudioSource GETAudioSource();
        bool HasAuthorityToPlay();

    }
}