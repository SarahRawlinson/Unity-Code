using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.Events;
using MultiplayerRTS.Control;

namespace MultiplayerRTS.StationControl
{
    public class Station : ControlItem
    {       

        [SerializeField] private UnityEvent onSelected = null;
        [SerializeField] private UnityEvent onDeselected = null;       

        public static event Action<Station> ServerOnStationSpawned;
        public static event Action<Station> ServerOnStationDespawned;
        public static event Action<Station> AuthorityOnStationSpawned;
        public static event Action<Station> AuthorityOnStationDespawned;


        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            ServerOnStationSpawned?.Invoke(this);
            Health.ServerOnDie += ServerHandleDie;
        }

        public override void OnStopServer()
        {
            ServerOnStationDespawned?.Invoke(this);
            Health.ServerOnDie -= ServerHandleDie;
        }

        [Server]
        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            AuthorityOnStationSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority) return;
            AuthorityOnStationDespawned?.Invoke(this);
        }

        [Client]
        public void Select()
        {
            if (!hasAuthority) return;
            onSelected?.Invoke();
        }

        [Client]
        public void Deselect()
        {
            if (!hasAuthority) return;
            onDeselected?.Invoke();
        }



        #endregion
    }
}
