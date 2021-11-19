using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiplayerRTS.EndGame;
using TMPro;
using Mirror;

namespace MultiplayerRTS.UI
{
    public class GameOverDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverDisplayParent = null;
        [SerializeField] private TMP_Text winnerNameText = null;
        void Start()
        {
            EndGameHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void OnDestroy()
        {
            EndGameHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        public void LeaveGame()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }
        }

        private void ClientHandleGameOver(string winner)
        {
            winnerNameText.text = $"{winner} Wins The Game!";
            gameOverDisplayParent.SetActive(true);
        }
    }
}
