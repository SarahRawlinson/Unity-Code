using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using MultiplayerRTS.Networking;
using  MultiplayerRTS.Resources;
using MultiplayerRTS.UnitControl;
using UnityEngine;

namespace MultiplayerRTS.PickUp
{
    public class PickUp : MonoBehaviour
    {
        [SerializeField] private int resources = 1000;
        private RTSNetworkPlayer player = null;
        private void Start()
        {
            player = NetworkClient.connection.identity.GetComponent<RTSNetworkPlayer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"collided with object {other.name}");
            if (other.attachedRigidbody == null) return;
            if (other.attachedRigidbody.TryGetComponent(out Unit unit))
            {
                Debug.Log($"{other.name} has rigidbody");
                if (unit.GetPlayer() == player)
                {
                    Debug.Log($"{other.name} has connection to player {player.DisplayName}");
                    player.GetComponent<ResourceTracker>().GiveResourcesLocal(resources);
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log($"{other.name} dose not have connection to player {player.DisplayName}");
                }
                
            }
        }
    }
}
