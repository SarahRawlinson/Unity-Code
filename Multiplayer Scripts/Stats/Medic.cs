using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace MultiplayerRTS.Stats
{
    public class Medic : NetworkBehaviour
    {
        [SerializeField] private int healthQtyOverTime;
        [SerializeField] private float timePeriod;
        [SerializeField] private float healthRadius;
        public int HealthQtyOverTime => healthQtyOverTime;
        public float TimePeriod => timePeriod;
        public float HealthRadius => healthRadius;
        private float counter = 0f;
        private Health_RTS _health;
        [SerializeField]
        private Color healthRadiusColor = Color.green;

        private void Start()
        {
        _health = GetComponent<Health_RTS>();
        }

        [ServerCallback]
        private void Update()
        {
        counter += Time.deltaTime;
        if (counter<timePeriod) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, healthRadius);

        List<Health_RTS> healths = new List<Health_RTS>();
            foreach (Collider collider in colliders)
        {
            if (!collider.TryGetComponent(out Health_RTS health)) continue;
            if (healths.Contains(health)) continue;
            if (health.connectionToClient != connectionToClient) continue;
            if (health == _health) return;
            healths.Add(health);
            health.GiveHealth(healthQtyOverTime);
        }
        }

        private void OnDrawGizmos()
        {
        Gizmos.color = healthRadiusColor;
        Gizmos.DrawWireSphere(transform.position, healthRadius);
    }
}

}
