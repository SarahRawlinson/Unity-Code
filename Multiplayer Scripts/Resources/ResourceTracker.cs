using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MultiplayerRTS.Resources
{
    public class ResourceTracker : NetworkBehaviour
    {
        [SyncVar(hook = nameof(HandleResourcesUpdated))]
        [SerializeField] private int resources = 500;
        public event Action<int> ClientOnResourceUpdate;
        private List<ResourceModifier> _modifiers = new List<ResourceModifier>();
        [SerializeField] private float calculateResourcesOverTime = 30f;
        public event Action<float, float, float> OnResourcesDetails;
        private float timer = 0f;
        [SyncVar]
        private float _incomingResources;
        [SyncVar]
        private float _outgoingResources;
        [SyncVar]
        private float _totalResources;
        public int Resources { get => resources;}
        
        [Server]
        public void GiveResources(int resource)
        {
            resources += resource;
        }
        
        public void GiveResourcesLocal(int resource)
        {
            CmdGiveResources(resource);
        }

        [Command]
        private void CmdGiveResources(int resource)
        {
            resources += resource;
        }

        [ServerCallback]
        public void Update()
        {
            timer += Time.deltaTime;
            if (timer >= calculateResourcesOverTime) return;
            timer = 0f;
            _incomingResources = 0f;
            _outgoingResources = 0f;
            _totalResources = 0f;
            foreach (ResourceModifier modifier in _modifiers)
            {
                float qty = modifier.ResourcesPerInterval;
                float time = modifier.Interval;
                float qtyOverTime = qty / time;
                qtyOverTime *= calculateResourcesOverTime;
                if (qtyOverTime < 0)
                {
                    _outgoingResources += qtyOverTime * -1;
                }
                else
                {
                    _incomingResources += qtyOverTime;
                }
            }
            _totalResources = _incomingResources - _outgoingResources;
            ResourcesUpdated();

        }

        [ClientRpc]
        private void ResourcesUpdated()
        {
            OnResourcesDetails?.Invoke(_incomingResources, _outgoingResources, _totalResources);
        }

        [Server]
        public void ActiveModifier(ResourceModifier modifier)
        {
            if (_modifiers.Contains(modifier)) return;
            _modifiers.Add(modifier);
            modifier.OnDestroyed += HandleModifierDestroyed;
        }

        private void HandleModifierDestroyed(ResourceModifier modifier)
        {
            if (!_modifiers.Contains(modifier)) return;
            _modifiers.Remove(modifier);
        }

        [Server]
        public void PayResources(int resource)
        {
            resources -= resource;
        }
        
        private void HandleResourcesUpdated(int oldResources, int newResources)
        {
            if (isClient && hasAuthority) ClientOnResourceUpdate?.Invoke(newResources);
        }
    }
}
