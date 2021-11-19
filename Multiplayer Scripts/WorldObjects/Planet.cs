using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MultiplayerRTS.WorldObjects
{
    public class Planet : MonoBehaviour
    {
        [SerializeField] private float captureRadius = .5f;
        [SerializeField] Color gizmoColor = Color.cyan;
        [SerializeField] private GameObject captureSphere;
        [SerializeField] public bool isSun = false;
        public float CaptureRadius
        {
            get => captureRadius;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, captureRadius);
        }

        private void Update()
        {
            DrawSphere();
        }
        
        private void DrawSphere()
        {
            float scale = transform.localScale.x;
            float size = (captureRadius * 2) / scale;
            captureSphere.transform.localScale = new Vector3(size, size, size);
        }
    }
}
