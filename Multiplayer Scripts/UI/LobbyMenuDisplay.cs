using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MultiplayerRTS.Networking;
using System;
using Mirror;
using UnityEngine.UI;
using TMPro;

namespace MultiplayerRTS.UI
{
    public class LobbyMenuDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyUI = null;
        [SerializeField] private Image[] connectedIcons = new Image[4];
        [SerializeField] private Button startGameButton = null;
        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];

        private void Start()
        {
            NetworkManager_RTS.ClientOnConnected += HandleClientConnected;
            NetworkManager_RTS.ClientOnDisconnected += HandleClientDisconnected;
            RTSNetworkPlayer.AuthorityOnPartOwnerStateUpdated += AuthorityHandlePartOwnerStateUpdated;
            RTSNetworkPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
        }

        private void AuthorityHandlePartOwnerStateUpdated(bool obj)
        {
            startGameButton.gameObject.SetActive(obj);
        }

        private void OnDestroy()
        {
            NetworkManager_RTS.ClientOnConnected -= HandleClientConnected;
            NetworkManager_RTS.ClientOnDisconnected -= HandleClientDisconnected;
            RTSNetworkPlayer.AuthorityOnPartOwnerStateUpdated -= AuthorityHandlePartOwnerStateUpdated;
            RTSNetworkPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        }

        private void ClientHandleInfoUpdated()
        {
            List<RTSNetworkPlayer> players = ((NetworkManager_RTS)NetworkManager.singleton).Players;

            for (int i = 0; i < players.Count; i++)
            {
                playerNameTexts[i].text = players[i].DisplayName;
                connectedIcons[i].color = players[i].DisplayColor;
                connectedIcons[i].gameObject.SetActive(true);
            }
            for (int i = players.Count; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = "Waiting For Player...";
                connectedIcons[i].color = Color.white;
                connectedIcons[i].gameObject.SetActive(false);
            }
            startGameButton.interactable = players.Count >= 2;
        }

        private void HandleClientDisconnected()
        {
            
        }

        public void StartGame()
        {
            NetworkClient.connection.identity.GetComponent<RTSNetworkPlayer>().CmdStartGame();
        }

        private void HandleClientConnected()
        {
            lobbyUI.SetActive(true);
        }

        public void LeaveLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
                SceneManager.LoadScene(0);
            }
        }
    }
}
