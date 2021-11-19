using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiplayerRTS.StationControl;
using MultiplayerRTS.UnitControl;
using MultiplayerRTS.Control;
using UnityEngine.UI;
using TMPro;
using MultiplayerRTS.Networking;
using Mirror;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;
using MultiplayerRTS.Resources;

namespace MultiplayerRTS.UI
{
    public class BuildButton : ControlItemButton, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private ControlItem stationOrUnit;
        //[SerializeField] private Image iconActive = null;
        //[SerializeField] private Image iconInactive = null;
        //[SerializeField] private TMP_Text priceText = null;
        [SerializeField] private LayerMask ground = new LayerMask();
        //public event Action<BuildButton> OnButtonDestroy;
        //public event Action OnButtonActive;
        //public event Action OnButtonDeactivate;
        private Camera mainCamera;
        //private RTSNetworkPlayer player;
        private GameObject buildPreviewInstance;
        //private Renderer stationRenderInstance;
        private BoxCollider buildCollider;
        [SerializeField] private bool FixedButton = false;
        private bool isBuilding = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            //Debug.Log("Button Down");
            SetUpBuild();
        }

        private void SetUpBuild()
        {
            if (Player.GetComponent<ResourceTracker>().Resources < stationOrUnit.Price)
            {
                return;
            }
            //if (!isBuilding) OnButtonActive?.Invoke();
            if (!isBuilding) Activate();
            if (!isBuilding) buildPreviewInstance = Instantiate(stationOrUnit.buildPrefab);
            //stationRenderInstance = buildPreviewInstance.GetComponentInChildren<Renderer>();
            buildPreviewInstance.SetActive(false);
            isBuilding = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (Mouse.current.leftButton.IsPressed()) return;
            BuildItem();
        }

        private void BuildItem()
        {            
            if (!isBuilding) return;            
            //Debug.Log($"Button Up, build {stationOrUnit.gameObject.name}");            
            //if (stationRenderInstance == null) return;
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
            {
                //Debug.Log("try place");
                Player.CmdTryPlaceBuild(stationOrUnit.ID, hit.point);
            }
            //if (Keyboard.current.ctrlKey.isPressed) return;
            isBuilding = false;
            //OnButtonDeactivate?.Invoke();
            Deactivate();
            Destroy(buildPreviewInstance);
        }

        public void Start()
        {
            if (!NetworkClient.isConnected) return;
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
            if (FixedButton) SetDetails(stationOrUnit);
            buildCollider = stationOrUnit.GetComponent<BoxCollider>();
        }

        public void SetDetails(ControlItem item)
        {
            ThisControlItem = item;
            stationOrUnit = item;
            iconActive.sprite = item.Icon;
            iconInactive.sprite = item.Icon;
            priceText.text = item.Price.ToString();
        }

        private void Update()
        {
            if (!NetworkClient.isConnected) return;
            if (!NetworkClient.connection.identity.TryGetComponent<ResourceTracker>(out ResourceTracker tracker)) return;
            //if (Mouse.current.leftButton.wasPressedThisFrame && Keyboard.current.ctrlKey.isPressed) SetUpBuild();
            if (isBuilding && Mouse.current.leftButton.wasReleasedThisFrame) BuildItem();
            if (tracker.Resources < stationOrUnit.price && iconActive.enabled)
            {
                //Debug.Log($"Not Enough Resources for {stationOrUnit.gameObject.name}");
                iconActive.enabled = false;
            }
            if (tracker.Resources > stationOrUnit.price && !iconActive.enabled)
            {
                //Debug.Log($"Now Have Enough Resources for {stationOrUnit.gameObject.name}");
                iconActive.enabled = true;
            }
            if (buildPreviewInstance == null) return;
            UpdateBuildPreview();
        }

        private void UpdateBuildPreview()
        {
            //Debug.Log("Update Build Preview");
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
            {
                //Debug.Log($"No Hit To Build {Mouse.current.position.ReadValue().ToString()}");                
                return;
            }
            else
            {
                //Debug.Log($"Has Hit Try Build {Mouse.current.position.ReadValue().ToString()}");
            }
            buildPreviewInstance.transform.position = hit.point;
            if (!buildPreviewInstance.activeSelf)
            {
                //Debug.Log("Show Build Preview");
                buildPreviewInstance.SetActive(true);
            }

            Color colour = Player.CanPlaceBuild(buildCollider, hit.point, stationOrUnit.transform.localScale.x) ? Color.green : Color.red;
            foreach (MeshRenderer rnd in buildPreviewInstance.GetComponentsInChildren<MeshRenderer>())
            {
                rnd.sharedMaterial.SetColor("_graphColour", colour);
                // rnd.material.SetColor("_BaseColor", colour);
            }
        }

        //private void OnDestroy()
        //{
        //    OnButtonDestroy?.Invoke(this);
        //}
    }

}