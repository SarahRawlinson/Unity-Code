using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using MultiplayerRTS.Stats;

namespace MultiplayerRTS.Combat
{
    public class CombatProjectile : NetworkBehaviour
    {
        [SerializeField] private Rigidbody rb = null;
        [SerializeField] float liftime = 50f;
        [SerializeField] public float launchForce = 10f;
        [SerializeField] private float hitDammage = 10f;
        [SerializeField] private string projectileName;
        private GameObject instigator;
        public string Name { get => projectileName; }
        public GameObject Instigator { get => instigator; set => instigator = value; }
        public float HitDamage { get => hitDammage; set => hitDammage = value; }

        private void Start()
        {
            rb.velocity = transform.forward * launchForce;
        }

        public override void OnStartServer()
        {
            Invoke(nameof(DestroyThis), liftime);
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity otherID))
            {
                if (otherID.connectionToClient == connectionToClient) return;
            }

            if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                if (rb.TryGetComponent<Health_RTS>(out Health_RTS health))
                {
                    health.DealDamage(hitDammage /*, other*/);
                    DestroyThis();
                }
                else
                {
                    Debug.Log($"hit other collider {other.gameObject.name}");
                }
            }
        }

        [Server]
        private void DestroyThis()
        {
            NetworkServer.Destroy(gameObject);
        }

        private void OnDestroy()
        {
            //Debug.Log("Bullet Destroyed");
        }
    }
}
