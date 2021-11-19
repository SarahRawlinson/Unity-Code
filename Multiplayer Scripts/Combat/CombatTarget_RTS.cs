using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MultiplayerRTS.Combat
{
    public class CombatTarget_RTS : NetworkBehaviour
    {
        [SerializeField] private Transform[] aimAtPoint = null;
        [SyncVar] private bool _isConnected = false;
        public Transform AimAtPoint(Vector3 pos)
        {
            if (aimAtPoint == null) return transform;
            Transform aim = aimAtPoint[0];
            float distance = Vector3.Distance(aim.position, pos);
            foreach (Transform pTransform in aimAtPoint)
            {
                if (Vector3.Distance(pTransform.position, pos) < distance)
                {
                    aim = pTransform;
                    distance = Vector3.Distance(pTransform.position, pos);
                }
            }
            return aim;
        }
        private void OnDestroy()
        {
            if (!_isConnected) return;
            onDestroy?.Invoke(this);
        }

        public override void OnStopServer()
        {
            _isConnected = false;
        }

        public override void OnStartServer()
        {
            _isConnected = true;
        }

        public event Action<CombatTarget_RTS> onDestroy;
        #region Server

        #endregion

        #region Client
        
        #endregion
    }
}
