using MultiplayerRTS.Control;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MultiplayerRTS.Spawning;
using MultiplayerRTS.Networking;
using Mirror;
using UnityEngine.EventSystems;
using System;
using MultiplayerRTS.Resources;

namespace MultiplayerRTS.UI
{
    public class SpawnButton : ControlItemButton, IPointerDownHandler, IPointerUpHandler
    {
        
        ControlItem stationOrUnit;
        //[SerializeField] private Image iconActive = null;
        //[SerializeField] private Image iconInactive = null;
        //[SerializeField] private TMP_Text priceText = null;
        //public event Action<SpawnButton> OnButtonDestroy;
        //public event Action OnButtonActive;
        //public event Action OnButtonDeactivate;
        RTSUnitSpawner spawner;
        private int spawnerUnitIndex = -1;
        private bool spawn = false;
        private Camera mainCamera;
        //private RTSNetworkPlayer player;

        public void Start()
        {
            try
            {
                Player = NetworkClient.connection.identity.GetComponent<RTSNetworkPlayer>();
            }
            catch (Exception e)
            {
                try
                {
                    Debug.Log($"{NetworkClient.connection.identity.gameObject.name} {e.Message}");
                }
                catch
                {
                    Debug.Log($"{e.Message}");
                }

                return;
            }
            mainCamera = Camera.main;
        }

        public void SetSpawner(RTSUnitSpawner spawn, int unitID, ControlItem item)
        {
            ThisControlItem = item;
            spawner = spawn;
            spawnerUnitIndex = unitID;
            stationOrUnit = item;
            priceText.text = item.price.ToString();
            iconActive.sprite = item.Icon;
            iconInactive.sprite = item.Icon;
        }

        private void Update()
        {
            ResourceTracker resourceTracker = NetworkClient.connection.identity.GetComponent<ResourceTracker>();
            //if (Mouse.current.leftButton.wasPressedThisFrame && Keyboard.current.ctrlKey.isPressed) SetUpBuild();
            if (resourceTracker.Resources < stationOrUnit.price && iconActive.enabled)
            {
                //Debug.Log($"Not Enough Resources for {stationOrUnit.gameObject.name}");
                iconActive.enabled = false;
            }
            if (resourceTracker.Resources > stationOrUnit.price && !iconActive.enabled)
            {
                //Debug.Log($"Now Have Enough Resources for {stationOrUnit.gameObject.name}");
                iconActive.enabled = true;
            }
        }

        public void SpawnItem()
        {
            //Debug.Log("Spawn");
            spawner.SpawnObject(spawnerUnitIndex);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //OnButtonActive?.Invoke(this);
            Activate();
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!spawn) SpawnItem();
            spawn = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //OnButtonDeactivate?.Invoke(this);
            Deactivate();
            spawn = false;
        }

        //private void OnDestroy()
        //{
        //    OnButtonDestroy?.Invoke(this);
        //}
    }
}
