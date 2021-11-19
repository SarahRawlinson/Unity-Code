using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using MultiplayerRTS.Networking;
using Mirror;

namespace MultiplayerRTS.UI
{
    public class JoinLobbyMenuDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject landingPage = null;
        [SerializeField] private TMP_InputField addressImput = null;
        [SerializeField] private Button joinButton = null;

        private void OnEnable()
        {
            NetworkManager_RTS.ClientOnConnected += HandleClientConnected;
            NetworkManager_RTS.ClientOnDisconnected += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            NetworkManager_RTS.ClientOnConnected -= HandleClientConnected;
            NetworkManager_RTS.ClientOnDisconnected -= HandleClientDisconnected;
        }

        public void Join()
        {
            string address = addressImput.text;
            NetworkManager.singleton.networkAddress = address;
            NetworkManager.singleton.StartClient();
            joinButton.interactable = false;
        }

        private void HandleClientConnected()
        {
            joinButton.interactable = true;
            landingPage.SetActive(false);
            gameObject.SetActive(false);
        }

        private void HandleClientDisconnected()
        {
            joinButton.interactable = true;
            landingPage.SetActive(true);
            gameObject.SetActive(true);
        }
    }
}
