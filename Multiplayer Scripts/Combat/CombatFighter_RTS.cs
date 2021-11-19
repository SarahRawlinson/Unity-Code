using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MultiplayerRTS.Audio;
using MultiplayerRTS.EndGame;
using MultiplayerRTS.Stats;
using UnityEngine.Serialization;

namespace MultiplayerRTS.Combat
{
    public class CombatFighter_RTS : NetworkBehaviour, PlaySoundEvent
    {
        
        public enum Status { Pursue, Defend, Formation, None}
        [SerializeField] Health_RTS health;
        private CombatTarget_RTS target;
        [SyncVar (hook = nameof(HandleStatusChange))]
        public Status status = Status.None;
        [SerializeField] private bool fireInRange = false;
        [FormerlySerializedAs("voidID")] [SerializeField] private int voiceID = 0;
        public event Action<Status> onStatusChange;
        public event Action<AudioEvent, int> onPlaySound;
        public event Action<PlaySoundEvent> onDeath;

        public CombatTarget_RTS Target { get => target; }
        public Status FighterStatus { get => status;}

        #region Server


        public override void OnStartServer()
        {
            EndGameHandler.ServerOnGameOver += ServerHandleGameOver;
            health.OnHitByTarget += HandleHitByCombatTarget;

        }
        
        [ServerCallback]
        public void Update()
        {
            if (fireInRange || target == null)
            {
                if (TryGetComponent(out CombatFiring combatFiring))
                {
                    target = combatFiring.TargetInRange();
                    if (target != null) PlaySound(AudioEvent.TargetFound);
                }
            }
        }

        public override void OnStopServer()
        {
            EndGameHandler.ServerOnGameOver -= ServerHandleGameOver;
            health.OnHitByTarget -= HandleHitByCombatTarget;

        }

        private void HandleHitByCombatTarget(CombatTarget_RTS newTarget)
        {
            if (target != null) return;
            PlaySound(AudioEvent.Defend);
            CmdSetTarget(target.gameObject);
        }

        [Command]
        public void CmdSetTarget(GameObject targetGameObject)
        {
            status = Status.Pursue;
            //Debug.Log("Check Game Object has Target");
            if (!targetGameObject.TryGetComponent<CombatTarget_RTS>(out CombatTarget_RTS newTarget))
            {
                //Debug.Log("No Target on game object");
                return;
            }
            //Debug.Log($"target Set {targetGameObject.name}");
            PlaySound(AudioEvent.GivenTarget);
            target = newTarget;
            target.onDestroy += ServerHandleTargetDeath;
        }

        public void ClearTarget()
        {
            if (status != Status.Pursue) return;
            PlaySound(AudioEvent.AtEase);
            status = Status.None;
            target = null;
        }

        [Server]
        public void ServerHandleGameOver()
        {
            ClearTarget();
        }
        [Server]
        public void ServerHandleTargetDeath(CombatTarget_RTS thisTarget)
        {
            PlaySound(AudioEvent.KillConfirmed);
            thisTarget.onDestroy -= ServerHandleTargetDeath;
            if (target == thisTarget)
            {
                ClearTarget();
            }
        }

        private void HandleStatusChange(Status oldStatus, Status newStatus)
        {
            onStatusChange?.Invoke(newStatus);
        }

        private void OnDestroy()
        {
            //onPlaySound?.Invoke(AudioEvent.Death, voiceID);
            onDeath?.Invoke(this);
        }

        #endregion

        #region Client
        [Client]
        private void PlaySound(AudioEvent audioEvent)
        {
            if (!hasAuthority) return;
            Debug.Log($"PLAY SOUND {audioEvent.ToString()}");
            onPlaySound?.Invoke(audioEvent, voiceID);
        }
        #endregion

        
    }
}
