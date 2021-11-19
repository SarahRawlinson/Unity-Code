using System.Collections;
using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using MultiplayerRTS.EndGame;
using MultiplayerRTS.Combat;

namespace MultiplayerRTS.Stats
{
    [RequireComponent(typeof(Rigidbody))]
    public class Health_RTS : NetworkBehaviour, IStat_RTS
    {        
        [SerializeField] private int maxHealth = 100;
        [SyncVar(hook = nameof(HandleHealthUpdated))]
        private int currentHealth;
        public event Action ServerOnDie;
        public event Action<int, int> ClientOnUpdateToStats;
        public event Action<CombatTarget_RTS> OnHitByTarget;
        public Collider AttackerCollider;

        #region Server

        public override void OnStartServer()
        {
            currentHealth = maxHealth;
            EndGameObject.ServerOnPlayerDie += ServerHandlePlayerDie;
        }

        public override void OnStopServer()
        {
            EndGameObject.ServerOnPlayerDie -= ServerHandlePlayerDie;
        }

        [Server]
        public void DealDamage(float damageAmount/*, Collider collider*/)
        {
            //AttackerCollider = collider;
            //if (collider.attachedRigidbody.TryGetComponent(out CombatTarget_RTS target))
            //{
            //    OnHitByTarget?.Invoke(target);
            //}
            if (currentHealth == 0) { return; }
            //Debug.Log($"{damageAmount} Dammage Recieved");
            currentHealth = Convert.ToInt32(Mathf.Max(currentHealth - damageAmount, 0));
            //Debug.Log($"{currentHealth} Health Left");
            if (currentHealth != 0) { return; }
            ServerOnDie?.Invoke();
            //Debug.Log("We Died");
        }

        #endregion

        #region Client

        private void HandleHealthUpdated(int oldHealth, int newHealth)
        {
            ClientOnUpdateToStats?.Invoke(currentHealth, maxHealth);
        }

        #endregion

        public int GetStatValue()
        {
            return currentHealth;
        }

        public int GetStartStatValue()
        {
            return maxHealth;
        }

        public float GetPercentageStatRemaining()
        {
            return currentHealth / maxHealth;
        }

        public void ServerHandlePlayerDie(int connectionId)
        {
            if (connectionId != connectionToClient.connectionId) return;
            DealDamage(currentHealth/*, AttackerCollider*/);
        }

        [Server]
        public void GiveHealth(int healQty)
        {
            if (currentHealth == maxHealth) return;
            if ((currentHealth + healQty) > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth += healQty;
            }
        }
    }
}

