using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerRTS.Audio
{
    [CreateAssetMenu(fileName = "Voice", menuName = "MultiplayerRTS/Voices/New Voice", order = 0)]
    public class Voice : ScriptableObject
    {
        [SerializeField] public string voiceName;
        [SerializeField] public int voiceID = 0;
        [SerializeField] public VoiceSample[] samples;
    }
    public enum AudioEvent
    {
        MoveOut,
        Attack,
        Defend,
        FindTarget,
        FailedToTarget,
        TargetFound,
        GivenTarget,
        AtEase,
        KillConfirmed,
        Death,
        Explosion,
        Fire
    }
    [Serializable]
    public class VoiceSample
    {
        [SerializeField] public string dialogue;
        [SerializeField] public AudioClip voiceClip;
        [FormerlySerializedAs("voiceEvent")] [SerializeField] public AudioEvent audioEvent;
    }
}