using System;

namespace MultiplayerRTS.Audio
{
    public interface PlaySoundEvent
    {
        event Action<AudioEvent, int> onPlaySound;
        event Action<PlaySoundEvent> onDeath;
    }
}