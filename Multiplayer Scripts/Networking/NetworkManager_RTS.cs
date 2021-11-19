using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MultiplayerRTS.EndGame;
using UnityEngine.SceneManagement;
using System;
using MultiplayerRTS.Movement;

namespace MultiplayerRTS.Networking
{
    public class NetworkManager_RTS : NetworkManager
    {
        [Tooltip("This is the the start of the scene name in a live game eg 'Map' for the actual name of Map_01")]
        [SerializeField] private string startOfSceneName = "Space_Solar_System";
        [SerializeField] string startScene;
        [SerializeField] private GameObject unitBasePrefab = null;
        [SerializeField] private EndGameHandler endGameHandlerPrefab = null;
        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;

        public List<RTSNetworkPlayer> Players { get; } = new List<RTSNetworkPlayer>();
        private bool isGameInProgress = false;

        #region Server

        public override void OnServerConnect(NetworkConnection conn)
        {
            if (!isGameInProgress) return;
            conn.Disconnect();
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            RTSNetworkPlayer player = conn.identity.GetComponent<RTSNetworkPlayer>();
            Players.Add(player);
            player.SetDisplayName($"Player {numPlayers}");

            player.SetDisplayColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
            //Debug.Log($"Player {player.DisplayName} Has Connected");
            //player.RpcLogTest();
            player.SetPartyOwner(Players.Count == 1);
        }

        private void SpawnStartObjects(RTSNetworkPlayer player)
        {
            Transform startPosition = GetStartPosition();
            GameObject unitSpawnerInstance = Instantiate(unitBasePrefab, startPosition.position, Quaternion.identity);
            NetworkServer.Spawn(unitSpawnerInstance, player.connectionToClient);
            player.transform.position = startPosition.position;
        }

        public override void OnStopServer()
        {
            Players.Clear();
            isGameInProgress = false;
        }

        public void StartGame()
        {
            if (Players.Count < 2) return;
            isGameInProgress = true;
            ServerChangeScene(startScene);
            
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            RTSNetworkPlayer player = conn.identity.GetComponent<RTSNetworkPlayer>();
            Players.Remove(player);
            base.OnServerDisconnect(conn);
        }

        // remeber this string refference must stay consistant
        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith(startOfSceneName))
            {
                EndGameHandler endGameHandlerInstance = Instantiate(endGameHandlerPrefab);
                NetworkServer.Spawn(endGameHandlerInstance.gameObject);

                foreach (RTSNetworkPlayer player in Players)
                {
                    SpawnStartObjects(player);
                    player.GetComponent<CameraController>().SceneLoaded();
                }
                
            }            
        }

        #endregion

        #region Client

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            ClientOnConnected?.Invoke();

        }
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            ClientOnDisconnected?.Invoke();
        }

        public override void OnStopClient()
        {
            Players.Clear();
        }

        #endregion



    }
}
