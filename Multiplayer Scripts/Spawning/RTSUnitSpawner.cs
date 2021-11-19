using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using MultiplayerRTS.Networking;
using MultiplayerRTS.Stats;
using MultiplayerRTS.UnitControl;
using MultiplayerRTS.Movement;
using MultiplayerRTS.Resources;

namespace MultiplayerRTS.Spawning
{
    // ReSharper disable once InconsistentNaming
    public class RTSUnitSpawner : NetworkBehaviour/*, IPointerClickHandler*/
    {
        [SerializeField] private Unit[] unitPrefabs;
        [SerializeField] private Transform unitSpawnTransform;
        [SerializeField] private Health_RTS health;
        [SerializeField] private TMP_Text unitQueueQtyText;
        [SerializeField] private Image unitProgressImage;
        [SerializeField] private int maxUnitQueue = 20;
        [SerializeField] private float spawnMoveRange = 10f;
        [SerializeField] private float unitSpawnDuration = 10f;
        [SerializeField] private GameObject parentFill;

        [SyncVar(hook = nameof(HandleUnitQueuedUpdated))]
        private int _queuedUnitsInt;
        [SyncVar]
        private readonly List<int> _queuedUnits = new List<int>();

        [SyncVar]
        private float _unitTimer;

        private float _progressImageVelocity;

        public Unit[] UnitPrefabs { get => unitPrefabs; set => unitPrefabs = value; }

        private void Update()
        {
            if (isServer)
            {
                ProduceUnits();
            }
            if (isClient)
            {
                UpdateTimerDisplay();
            }
        }

        #region Server        

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleDie;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleDie;
        }

        [Server]
        private void ProduceUnits()
        {
            if (_queuedUnitsInt == 0) return;
            _unitTimer += Time.deltaTime;
            if (_unitTimer < unitSpawnDuration) return;
            int unitIndex = _queuedUnits[0];
            _queuedUnits.RemoveAt(0);
            var position = unitSpawnTransform.position;
            GameObject unitInstance = Instantiate(unitPrefabs[unitIndex].gameObject, position, unitSpawnTransform.rotation);
            NetworkServer.Spawn(unitInstance, connectionToClient);
            Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
            spawnOffset.y = position.y;
            RTSUnitMovement mover = unitInstance.GetComponent<RTSUnitMovement>();
            mover.ServerMove(position + spawnOffset);
            _queuedUnitsInt--;
            _unitTimer = 0f;
        }

        [Server]
        private void ServerHandleDie()
        {
            //NetworkServer.Destroy(gameObject);
        }

        [Command]
        private void CmdSpawnUnit(int unitIndex)
        {
            if (_queuedUnitsInt == maxUnitQueue) return;
            RTSNetworkPlayer player = connectionToClient.identity.GetComponent<RTSNetworkPlayer>();
            if (player.GetComponent<ResourceTracker>().Resources < unitPrefabs[unitIndex].Price) return;
            player.GetComponent<ResourceTracker>().PayResources(unitPrefabs[unitIndex].Price);
            _queuedUnitsInt++;
            _queuedUnits.Add(unitIndex);
            //Debug.Log("Unit Queued!");
        }

        #endregion

        #region Client

        //public void OnPointerClick(PointerEventData eventData)
        //{
        //    if (eventData.button != PointerEventData.InputButton.Left) return;
        //    if (!hasAuthority) return;
        //    ////Debug.Log("Spawn!");
        //    //cmdSpawnUnit(1);
        //    ClientOnSelected?.Invoke(unitPrefabs);
        //}

        public void SpawnObject(int unitIndex)
        {
            if (!hasAuthority) return;
            CmdSpawnUnit(unitIndex);
        }

        private void UpdateTimerDisplay()
        {
            float newProgress = _unitTimer / unitSpawnDuration;

            if (newProgress < unitProgressImage.fillAmount)
            {
                unitProgressImage.fillAmount = newProgress;
            }
            else
            {
                unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, 
                    newProgress, ref _progressImageVelocity, 0.1f);
            }
        }

        // ReSharper disable once UnusedParameter.Local
        private void HandleUnitQueuedUpdated(int oldQueue, int newQueue)
        {
            unitQueueQtyText.text = newQueue.ToString();
            if (parentFill == null) return;
            parentFill.SetActive(newQueue > 0);
        }

        #endregion


    }
}
