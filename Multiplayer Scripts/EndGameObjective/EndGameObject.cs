using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MultiplayerRTS.Stats;
using System;

namespace MultiplayerRTS.EndGame
{
    public class EndGameObject : NetworkBehaviour, IEndGame
    {
        [SerializeField] private Health_RTS health;

        public static event Action<int> ServerOnPlayerDie;
        public static event Action<EndGameObject> ServerIEndGameSpawned;
        public static event Action<EndGameObject> ServerIEndGameDespawned;

        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleDie;
            ServerIEndGameSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            ServerIEndGameDespawned?.Invoke(this);
            health.ServerOnDie -= ServerHandleDie;
        }

        [Server]
        private void ServerHandleDie()
        {
            ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client





        #endregion
    }
}
