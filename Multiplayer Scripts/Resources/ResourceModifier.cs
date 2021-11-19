using Mirror;
using MultiplayerRTS.Networking;
using MultiplayerRTS.Stats;
using MultiplayerRTS.EndGame;
using System;
using UnityEngine;

namespace MultiplayerRTS.Resources
{
    public class ResourceModifier : NetworkBehaviour
    {
        [SerializeField] private Health_RTS health;
        [SerializeField] private int resourcesPerInterval = 10;
        [SerializeField] private float interval = 2f;
        public event Action<ResourceModifier> OnDestroyedDueToLackOfRecourses;
        public event Action<ResourceModifier> OnDestroyed;
        private float _timer;
        private ResourceTracker _resourceTracker;
        [SyncVar] private bool _isConnected = false;
        public float Interval => interval;
        public int ResourcesPerInterval => resourcesPerInterval;

        public override void OnStartServer()
        {
            _isConnected = true;
            _timer = interval;
            _resourceTracker = connectionToClient.identity.GetComponent<ResourceTracker>();
            _resourceTracker.ActiveModifier(this);
            health.ServerOnDie += ServerHandleDie;
            EndGameHandler.ServerOnGameOver += ServerHandleGameOver;
        }
        
        private void OnDestroy()
        {
            if (_isConnected) return;
            OnDestroyed?.Invoke(this);
        }

        [ServerCallback]
        private void Update()
        {
            _timer -= Time.deltaTime;
            if (!(_timer <= 0)) return;
            if (resourcesPerInterval < 0 && _resourceTracker.Resources + resourcesPerInterval < 0)
            {
                HandleNotEnoughResources();
            }
            _resourceTracker.GiveResources(resourcesPerInterval);
            _timer += interval;
        }

        private void HandleNotEnoughResources()
        {
            if (!_isConnected) return;
            OnDestroyedDueToLackOfRecourses?.Invoke(this);
            NetworkServer.Destroy(gameObject);
        }

        public override void OnStopServer()
        {
            _isConnected = true;
            health.ServerOnDie -= ServerHandleDie;
            EndGameHandler.ServerOnGameOver -= ServerHandleGameOver;
        }


        private void ServerHandleDie()
        {
            if (_isConnected) return;
            NetworkServer.Destroy(gameObject);
        }


        private void ServerHandleGameOver()
        {
            enabled = false;
        }
    }
}
