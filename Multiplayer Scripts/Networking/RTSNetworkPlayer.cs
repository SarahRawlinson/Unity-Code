using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using MultiplayerRTS.UnitControl;
using MultiplayerRTS.StationControl;
using MultiplayerRTS.Control;
using MultiplayerRTS.Movement;
using System;
using MultiplayerRTS.Combat;
using MultiplayerRTS.Resources;
using MultiplayerRTS.WorldObjects;
using Unity.Mathematics;
using UnityEngine.Serialization;

namespace MultiplayerRTS.Networking
{
    // ReSharper disable once InconsistentNaming
    public partial class RTSNetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private LayerMask buildLayer;
        [SerializeField] private ControlItem[] buildItems = Array.Empty<ControlItem>();
        [SerializeField] private TMP_Text displayNameText;
        [SerializeField] private Renderer displayColourRenderer;
        [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
        [SerializeField] private string displayName = "None";
        [SyncVar(hook = nameof(HandleDisplayColourUpdated))]
        [SerializeField] private Color displayColor = Color.white;
        [SerializeField] private CheckName checkName;
        [SyncVar] public int connectionID;
        
        
        // [SerializeField] private float buildRangeLimit = 5f;
        [SerializeField] private ResourceTracker _resourceTracker;
        public Color DisplayColor { get => displayColor; }
        public string DisplayName { get => displayName; }
        public List<Unit> MyUnits { get => _myUnits; }
        public List<Station> MyStations { get => _myStations; }
        public List<ControlItem> MyControlItems { get => _myControlItems; }
        public Transform CameraTransform { get => cameraTransform;}
        public bool IsPartOwner { get => _isPartOwner; }
        public ControlItem[] BuildItems { get => buildItems;  }
        public event Action<Color> ClientOnTeamColourUpdated;
        public event Action<string> ClientOnTeamNameUpdated;
        public static event Action<bool> AuthorityOnPartOwnerStateUpdated;
        public static event Action ClientOnInfoUpdated;

        [SyncVar(hook = nameof(AuthorityHandlePartOwnerChange))]
        private bool _isPartOwner;
        private readonly List<Unit> _myUnits = new List<Unit>();
        private readonly List<Station> _myStations = new List<Station>();
        private readonly List<ControlItem> _myControlItems = new List<ControlItem>();
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        public bool CanPlaceBuild(BoxCollider buildBoxCollider, Vector3 point, float scale)
        {
            if (CheckForObsticles(buildBoxCollider, point, scale) || !CanMoveToLocation(point)) return false;
            return InRangeOfABaseObject(point) && InRangeOfAPlanet(point);
        }

        private bool InRangeOfAPlanet(Vector3 pos)
        {
            foreach (Planet planet in FindObjectsOfType<Planet>())
            {
                if (Vector3.Distance(planet.transform.position, pos) <= planet.CaptureRadius)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckForObsticles(BoxCollider buildBoxCollider, Vector3 point, float scale)
        {
            return Physics.CheckBox(point + (buildBoxCollider.center * scale),
                (buildBoxCollider.size * scale) / 2, Quaternion.identity, buildLayer);
        }

        private bool InRangeOfABaseObject(Vector3 targetPosition)
        {
            bool inRange = false;
            foreach (Unit unit in MyUnits)
            {
                if (inRange) break;
                if (unit.TryGetComponent(out BaseObject baseObject))
                {
                    inRange = InRange(targetPosition, unit, baseObject.baseRange);
                }                
            }
            foreach (Station station in _myStations)
            {
                if (inRange) break;
                if (station.TryGetComponent(out BaseObject baseObject))
                {
                    inRange = InRange(targetPosition, station, baseObject.baseRange);
                }
            }
            //if (!inRange) Debug.Log("Not in Range of base objects");
            return inRange;
        }

        private bool CanMoveToLocation(Vector3 targetPosition)
        {
            bool canMove = RTSUnitMovement.CanMoveTo(targetPosition, MyUnits[0].transform.position);
            if (!canMove) Debug.Log("Cant Make Path To Location");
            return canMove;
        }

        private ControlItem TryValidateBuild(int prefabID, ControlItem buildToPlace)
        {
            foreach (ControlItem buildItem in buildItems)
            {
                if (buildItem.ID == prefabID)
                {
                    //Debug.Log("Build ID found");
                    buildToPlace = buildItem;
                    break;
                }
            }
            return buildToPlace;
        }

        // private bool AnyObjectsInRange(Vector3 point)
        // {
        //     bool inRange = false;
        //     foreach (ControlItem item in MyStations)
        //     {
        //         if (inRange) break;
        //         inRange = InRange(point, item);
        //     }
        //     foreach (ControlItem item in MyUnits)
        //     {
        //         if (inRange) break;
        //         inRange = InRange(point, item);
        //     }
        //     //Debug.Log("Not Close Enough To Other Builds");
        //     return inRange;
        // }

        private bool InRange(Vector3 point, ControlItem item, float itemRange)
        {
            bool inRange = (point - item.transform.position).sqrMagnitude <= itemRange * itemRange;

            return inRange;
        }

        #region Server

        

        public override void OnStartServer()
        {
            connectionID = connectionToClient.connectionId;
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
            Station.ServerOnStationSpawned += ServerHandleStationSpawned;
            Station.ServerOnStationDespawned += ServerHandleStationDespawned;
            DontDestroyOnLoad(gameObject);
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
            Station.ServerOnStationSpawned -= ServerHandleStationSpawned;
            Station.ServerOnStationDespawned -= ServerHandleStationDespawned;
        }

        [Command]
        public void CmdTryPlaceBuild(int prefabID, Vector3 point)
        {
            var buildToPlace = TryValidateBuild(prefabID, null);
            if (buildToPlace == null)
            {
                //Debug.Log("Build is null");
                return;
            }
            if (_resourceTracker.Resources < buildToPlace.Price) return;
            BoxCollider buildBoxCollider = buildToPlace.GetComponent<BoxCollider>();
            if (!CanPlaceBuild(buildBoxCollider, point, buildToPlace.transform.localScale.x))
            {
                //Debug.Log("Cant Place Build");
                return;
            }
            GameObject build = Instantiate(buildToPlace.gameObject, point, buildToPlace.transform.rotation);
            NetworkServer.Spawn(build, connectionToClient);
            _resourceTracker.PayResources(buildToPlace.Price);
        }

        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            _myControlItems.Add(unit);
            _myUnits.Add(unit);
        }

        private void ServerHandleUnitDespawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            _myControlItems.Remove(unit);
            _myUnits.Remove(unit);
        }

        private void ServerHandleStationSpawned(Station station)
        {
            if (station.connectionToClient.connectionId != connectionToClient.connectionId) return;
            _myControlItems.Add(station);
            _myStations.Add(station);
        }

        private void ServerHandleStationDespawned(Station station)
        {
            if (station.connectionToClient.connectionId != connectionToClient.connectionId) return;
            _myControlItems.Remove(station);
            _myStations.Remove(station);
        }

        [Server]
        public void SetDisplayName(string newDisplayName)
        {
            displayName = newDisplayName;
        }

        [Server]
        public void SetDisplayColor(Color newDisplayColor)
        {
            displayColor = newDisplayColor;
        }

        [Server]
        public void SetPartyOwner(bool state)
        {
            _isPartOwner = state;
        }

        [Command]
        public void CmdStartGame()
        {
            if (!_isPartOwner) return;
            ((NetworkManager_RTS)NetworkManager.singleton).StartGame();
        }

        [Command]
        // ReSharper disable once UnusedMember.Local
        private void CmdSetDisplayName(string newDisplayName)
        {
            if (!CheckNameIsValid(newDisplayName))
            {
                Debug.Log("Name is not allowed");
                return;
            }
            //RpcLogNewName(newDisplayName);
            SetDisplayName(newDisplayName);
        }

        [Command]
        // ReSharper disable once UnusedMember.Local
        private void CmdSetDisplayColor(Color newDisplayColour)
        {
            SetDisplayColor(newDisplayColour);
        }


        private bool CheckNameIsValid(string nameToCheck)
        {
            if (nameToCheck.Length > checkName.maxLengthOfName || nameToCheck.Length < checkName.minLengthOfName) return false;
            foreach (char c in nameToCheck.ToLower())
            {
                foreach (char i in checkName.bandCharactersSymbles)
                {
                    string lowerI = i.ToString();
                    char ch = lowerI[0];
                    if (ch == c) return false;
                }
            }
            foreach (string bannedName in checkName.illigalNames)
            {
                if (nameToCheck.ToLower().Contains(bannedName.ToLower())) return false;
            }
            Debug.Log("Name is OK");
            return true;
        }

        #endregion

        #region client

        public override void OnStartAuthority()
        {
            if (NetworkServer.active) return;
            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
            Station.AuthorityOnStationSpawned += AuthorityHandleStationSpawned;
            Station.AuthorityOnStationDespawned += AuthorityHandleStationDespawned;
        }

        public override void OnStartClient()
        {
            if (NetworkServer.active) return;
            DontDestroyOnLoad(gameObject);
            ((NetworkManager_RTS)NetworkManager.singleton).Players.Add(this);
        }

        public override void OnStopClient()
        {
            ClientOnInfoUpdated?.Invoke();
            if (!isClientOnly) return;
            ((NetworkManager_RTS)NetworkManager.singleton).Players.Remove(this);
            if (!hasAuthority) return;
            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
            Station.AuthorityOnStationSpawned -= AuthorityHandleStationSpawned;
            Station.AuthorityOnStationDespawned -= AuthorityHandleStationDespawned;
        }

        // ReSharper disable once UnusedParameter.Local
        private void AuthorityHandlePartOwnerChange(bool oldState, bool newState)
        {
            if (!hasAuthority) return;
            AuthorityOnPartOwnerStateUpdated?.Invoke(newState);
        }

        private void AuthorityHandleUnitSpawned(Unit unit)
        {
            _myControlItems.Add(unit);
            _myUnits.Add(unit);
        }

        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            _myControlItems.Remove(unit);
            _myUnits.Remove(unit);
        }

        private void AuthorityHandleStationSpawned(Station station)
        {
            _myControlItems.Add(station);
            _myStations.Add(station);
        }

        private void AuthorityHandleStationDespawned(Station station)
        {
            _myControlItems.Remove(station);
            _myStations.Remove(station);
        }

        // ReSharper disable once UnusedParameter.Local
        private void HandleDisplayColourUpdated(Color oldColour, Color newColour)
        {
            ClientOnTeamColourUpdated?.Invoke(newColour);
            displayNameText.color = newColour;
            displayColourRenderer.material.SetColor(BaseColor, newColour);
            ClientOnInfoUpdated?.Invoke();
        }

        // ReSharper disable once UnusedParameter.Local
        private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
        {
            ClientOnInfoUpdated?.Invoke();
            ClientOnTeamNameUpdated?.Invoke(newDisplayName);
            displayNameText.text = newDisplayName;
        }

        //[ContextMenu("SetMyName")]
        //private void SetMyName()
        //{
        //    CmdSetDisplayName("Sarah");
        //}

        //[ContextMenu("RandomColour")]
        //private void RandomColour()
        //{
        //    Color colour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        //    CmdSetDisplayColor(colour);
        //}

        //[ClientRpc]
        //private void RpcLogNewName(string newDisplayName)
        //{
        //    Debug.Log("Name Logged");
        //}

        //[TargetRpc]
        //public void RpcLogTest()
        //{
        //    //Debug.Log("test Logged");
        //}

        #endregion
    }
}
