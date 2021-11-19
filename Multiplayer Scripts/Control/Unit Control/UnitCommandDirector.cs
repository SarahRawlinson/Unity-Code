using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MultiplayerRTS.Combat;
using MultiplayerRTS.EndGame;

namespace MultiplayerRTS.UnitControl
{
    public class UnitCommandDirector : MonoBehaviour
    {
        [SerializeField] float formationPaddingX = 2f;
        [SerializeField] float formationPaddingZ = 2f;
        [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
        [SerializeField] LayerMask layerMask = new LayerMask();
        //[SerializeField] GameObject prefabTarget = null;
        //[SerializeField] GameObject prefabActual = null;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            EndGameHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void OnDestroy()
        {
            EndGameHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) return;
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;
            if (hit.collider.TryGetComponent<CombatTarget_RTS>(out CombatTarget_RTS target))
            {
                if (target.hasAuthority)
                {
                    //Debug.Log($"player owns target");
                    TryMove(hit.point);
                    return;
                }
                //Debug.Log($"player doesnt own target");
                //TryMove(hit.point);
                TryTarget(target);
                return;
            }
            //Debug.Log($"no target");
            TryMove(hit.point);
        }

        private void TryTarget(CombatTarget_RTS target)
        {
            List<Unit> units = unitSelectionHandler.SelectedUnits;
            foreach (Unit unit in units)
            {
                //Debug.Log($"{unit.gameObject.name} try set target to {target.gameObject.name}");
                try
                {
                    unit.MovePointer(target.transform.position);
                    unit.Fighter.CmdSetTarget(target.gameObject);
                }
                catch (Exception e)
                {
                    Debug.Log($"{target.gameObject.name} {e.Message}");
                }
            }
        }

        private void TryMove(Vector3 point)
        {            
            List<Unit> units;
            //GameObject aimPointer = Instantiate(prefabTarget, point, Quaternion.identity);
            //Destroy(aimPointer, 5f);
            units = unitSelectionHandler.SelectedUnits;
            List<Vector3> posistionGrid = CreateGrid(units, point);
            foreach (Unit unit in units)
            {
                try
                {
                    Vector3 FarthestVector = posistionGrid[0];
                    foreach (Vector3 vector in posistionGrid)
                    {
                        FarthestVector = GetFarthestDistanceVector(unit, FarthestVector, vector);
                    }
                    //GameObject pointer = Instantiate(prefabActual, FarthestVector, Quaternion.identity);
                    //Destroy(pointer, 5f);
                    unit.RtsUnitMovement.CmdMove(FarthestVector);
                    unit.MovePointer(FarthestVector);
                    posistionGrid.Remove(FarthestVector);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
                
            }
        }

        private static Vector3 GetFarthestDistanceVector(Unit unit, Vector3 FarthestVector, Vector3 vector)
        {
            if (Vector3.Distance(vector, unit.transform.position) > Vector3.Distance(FarthestVector, unit.transform.position))
            {
                FarthestVector = vector;
            }

            return FarthestVector;
        }

        private List<Vector3> CreateGrid(List<Unit> units, Vector3 point)
        {
            float numberOfUnits;
            int numberOfRows, numberOfColumns;
            List<Vector3> posistionGrid = new List<Vector3>();            
            numberOfUnits = units.Count;
            numberOfRows = 1;
            numberOfColumns = units.Count;
            if (numberOfUnits > 3)
            {
                numberOfRows = Mathf.RoundToInt(Mathf.Pow(numberOfUnits, .5f));
                numberOfColumns = numberOfRows;
                //Debug.Log(units.Count % numberOfRows);
                while (units.Count > (numberOfRows * numberOfColumns))
                {
                    numberOfColumns += 1;
                }
            }
            units.Sort();
            float xPad = units[0].Size.x;
            float zPad = units[0].Size.z;
            foreach (Unit unit in units)
            {
                if (xPad < unit.Size.x) xPad = unit.Size.x;
                if (zPad < unit.Size.z) zPad = unit.Size.z;
            }
            
            int i = 0;
            float gridposX = point.x;
            float gridposZ = point.z;
            if (xPad > zPad) zPad = xPad;
            else if (zPad > xPad) xPad = zPad;
            for (int r = 0; r < numberOfRows; r++)
            {
                gridposZ = point.z;
                for (int c = 0; c < numberOfColumns; c++)
                {
                    Vector3 gridPoint = new Vector3(gridposX, point.y, gridposZ);
                    posistionGrid.Add(gridPoint);
                    //Debug.Log(gridPoint.ToString());
                    i += 1;
                    gridposZ = gridposZ + zPad + formationPaddingZ;
                }
                gridposX = gridposX + xPad + formationPaddingX;                
            }
            //Debug.Log($"Number of Units = {numberOfUnits}, number of rows = {numberOfRows}, number of columns = {numberOfColumns}");
            //Debug.Log($"Number OF Vectors = {posistionGrid.Count}");
            return posistionGrid;
        }

        private void ClientHandleGameOver(string winnerName)
        {
            enabled = false;
        }
    }
}
