using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MultiplayerRTS.Networking;
using MultiplayerRTS.EndGame;
using MultiplayerRTS.UI;
using Mirror;
using MultiplayerRTS.StationControl;

namespace MultiplayerRTS.UnitControl
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] RectTransform unitSelectionArea = null;
        [SerializeField] LayerMask layerMask = new LayerMask();
        private Camera mainCamera;
        [SerializeField] private List<Unit> selectedUnits = new List<Unit>();
        [SerializeField] private List<Station> selectedStations = new List<Station>();
        private Vector2 startPosition;
        private RTSNetworkPlayer player = null;
        public List<Unit> SelectedUnits { get => selectedUnits; }

        private void SelectedUnitStationsUpdate()
        {
            AddSpawnableObjectsToDisplay display = FindObjectOfType<AddSpawnableObjectsToDisplay>();
            //Debug.Log($"Units = {selectedUnits.Count}");
            display.ClearSpawnButtons();
            display.ClearBuildButtons();            
            if (selectedStations.Count + selectedUnits.Count != 1)
            {
                display.AddBuildButton(player.BuildItems);
                return;
            }
            if (selectedStations.Count == 1)
            {
                display.AddSpawnButton(selectedStations[0]);
            }
            if (selectedUnits.Count == 1)
            {
                display.AddSpawnButton(selectedUnits[0]);
            }
        }

        private void Start()
        {
            try
            {
                player = NetworkClient.connection.identity.GetComponent<RTSNetworkPlayer>();
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
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
            Station.AuthorityOnStationDespawned += AuthorityHandleStationDespawned;
            EndGameHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void OnDestroy()
        {
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
            Station.AuthorityOnStationDespawned -= AuthorityHandleStationDespawned;
            EndGameHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartSelectionArea();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                IncludeSelectedInList();
                
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateSelectionArea();
            }
        }

        private void IncludeSelectedInList()
        {
            unitSelectionArea.gameObject.SetActive(false);
            if (unitSelectionArea.sizeDelta.magnitude == 0)
            {
                AddUnitStationToSelectedList();
                return;
            }
            AddFromSelectionBox();
        }

        private void AddFromSelectionBox()
        {
            Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
            Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);
            foreach (Unit unit in player.MyUnits)
            {
                if (selectedUnits.Contains(unit)) continue;
                //Debug.Log($"{unit.gameObject.name} checking on screen");
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);
                if (
                    screenPosition.x > min.x &&
                    screenPosition.x < max.x &&
                    screenPosition.y > min.y &&
                    screenPosition.y < max.y)
                {
                    //Debug.Log($"{unit.gameObject.name} is on screen");
                    SelectedUnits.Add(unit);
                    unit.Select();
                }
            }
            foreach (Station station in player.MyStations)
            {
                if (CheckUnitsSelected()) return;
                if (selectedStations.Contains(station)) continue;
                //Debug.Log($"{unit.gameObject.name} checking on screen");
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(station.transform.position);
                if (
                    screenPosition.x > min.x &&
                    screenPosition.x < max.x &&
                    screenPosition.y > min.y &&
                    screenPosition.y < max.y )
                {
                    //Debug.Log($"{unit.gameObject.name} is on screen");
                    selectedStations.Add(station);
                    station.Select();
                }
            }
        }

        private bool CheckUnitsSelected()
        {
            if (selectedUnits.Count > 0)
            {
                ClearStationsList();
                return true;
            }
            return false;
        }

        private void StartSelectionArea()
        {
            if (FindObjectOfType<AddSpawnableObjectsToDisplay>().UIActive) return;
            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                ClearUnitList();
                ClearStationsList();
            }            
            unitSelectionArea.gameObject.SetActive(true);
            startPosition = Mouse.current.position.ReadValue();
            UpdateSelectionArea();
        }

        private void UpdateSelectionArea()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            float areaWidth = mousePosition.x - startPosition.x;
            float areaHeight = mousePosition.y - startPosition.y;
            unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
            unitSelectionArea.position = startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
        }

        private void ClearUnitList()
        {            
            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Deselect();
            }
            selectedUnits.Clear();
            SelectedUnitStationsUpdate();
        }

        private void ClearStationsList()
        {
            foreach (Station selectedStation in selectedStations)
            {
                selectedStation.Deselect();
            }
            selectedStations.Clear();
            SelectedUnitStationsUpdate();
        }

        private void AddUnitStationToSelectedList() //clear selection area
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;
            AddUnit(hit);
            AddStation(hit);
        }

        private void AddUnit(RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;
            if (!unit.hasAuthority) return;
            selectedUnits.Add(unit);
            foreach (Unit selectedUnit in selectedUnits)
            {

                selectedUnit.Select();
            }
            SelectedUnitStationsUpdate();
        }

        private void AddStation(RaycastHit hit)
        {
            if (CheckUnitsSelected()) return;
            if (!hit.collider.TryGetComponent<Station>(out Station station)) return;
            if (!station.hasAuthority) return;
            selectedStations.Add(station);
            foreach (Station selectedStation in selectedStations)
            {

                selectedStation.Select();
            }
            SelectedUnitStationsUpdate();
        }

        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            SelectedUnits.Remove(unit);
            SelectedUnitStationsUpdate();
        }

        private void AuthorityHandleStationDespawned(Station station)
        {
            selectedStations.Remove(station);
            SelectedUnitStationsUpdate();
        }

        private void ClientHandleGameOver(string winnerName)
        {
            enabled = false;
        }
    }
}
