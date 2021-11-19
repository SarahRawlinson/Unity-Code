using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using MultiplayerRTS.Movement;
using MultiplayerRTS.Combat;
using MultiplayerRTS.Stats;
using MultiplayerRTS.Control;
using System;
using UnityEngine.Serialization;

namespace MultiplayerRTS.UnitControl
{
    public class Unit : ControlItem, IComparable
    {
        [FormerlySerializedAs("unitMovement")] [SerializeField] private RTSUnitMovement rtsUnitMovement = null;
        [SerializeField] private CombatFighter_RTS fighter = null;
        [SerializeField] private UnityEvent onSelected = null;
        [SerializeField] private UnityEvent onDeselected = null;
        [SerializeField] private GameObject movePointer = null;
        [SerializeField] private float timeForMovePointer = 5f;
        private Vector3 pointerTarget;

        public RTSUnitMovement RtsUnitMovement { get => rtsUnitMovement; }
        public CombatFighter_RTS Fighter { get => fighter; }
        public enum ShipType
        {
            Bomber,
            Colossal,
            Cruiser,
            FighterHeavy,
            FighterLight,
            Stealth,
            Transport
        }

        [SerializeField] private ShipType _shipType = ShipType.Transport;
        public ShipType TypeOfShip { get => _shipType; }
        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawned;
        public static event Action<Unit> AuthorityOnUnitSpawned;
        public static event Action<Unit> AuthorityOnUnitDespawned;
        public Vector3 Size { get => GetSize(); }
        private GameObject pointer = null;

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Unit otherTemperature = obj as Unit;
            if (otherTemperature != null)
                return this.SortByAscending().CompareTo(otherTemperature.SortByAscending());
            else
                throw new ArgumentException("Object is not a Unit");
        }

        private Vector3 GetSize()
        {
            BoxCollider box = GetComponent<BoxCollider>();
            Vector3 scale = transform.localScale;
            Vector3 size = new Vector3(box.size.x * scale.x, box.size.y * scale.y, box.size.z * scale.z);
            return size;
        }

        public float SortByAscending()
        {
            return Size.x * Size.z;
        }

        public void CreatePointerGameObject(Vector3 target)
        {
            pointer = Instantiate(movePointer, target, Quaternion.identity);
        }

        public void MovePointer(Vector3 target)
        {
            if (pointer == null) CreatePointerGameObject(target);
            pointer.SetActive(true);
            pointer.transform.position = target;
            pointerTarget = target;
            StartCoroutine(TurnOffPointer(target));
        }

        IEnumerator TurnOffPointer(Vector3 target)
        {
            yield return new WaitForSeconds(timeForMovePointer);
            if (target == pointerTarget) pointer.SetActive(false);
        }
        
        

        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();
            ServerOnUnitSpawned?.Invoke(this);
            Health.ServerOnDie += ServerHandleDie;
        }

        public override void OnStopServer()
        {
            
            ServerOnUnitDespawned?.Invoke(this);
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
            AuthorityOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority) return;
            AuthorityOnUnitDespawned?.Invoke(this);
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
