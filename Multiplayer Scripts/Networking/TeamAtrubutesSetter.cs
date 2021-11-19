using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MultiplayerRTS.Control;
using TMPro;

namespace MultiplayerRTS.Networking
{
    public class TeamAtrubutesSetter : NetworkBehaviour
    {
        [SerializeField] private Renderer[] colorRenderer = new Renderer[0];
        [SerializeField] private TMP_Text[] displayNameTexts;

        [SyncVar(hook = nameof(HandleTeamColourUpdated))]
        private Color teamColour = new Color();

        [SyncVar(hook = nameof(HandleTeamNameUpdated))]
        private string teamName = "None";

        public override void OnStartServer()
        {
            RTSNetworkPlayer player = connectionToClient.identity.GetComponent<RTSNetworkPlayer>();
            player.ClientOnTeamColourUpdated += ChangeColour;
            player.ClientOnTeamNameUpdated += ChangeName;
            teamColour = player.DisplayColor;
            teamName = player.DisplayName;
        }
        private void OnDestroy()
        {
            try
            {
                RTSNetworkPlayer player = connectionToClient.identity.GetComponent<RTSNetworkPlayer>();
                player.ClientOnTeamColourUpdated -= ChangeColour;
                player.ClientOnTeamNameUpdated -= ChangeName;
            }
            catch
            {

            }
        }

        #region Server

        #endregion

        #region Client

        private void HandleTeamColourUpdated(Color oldColour, Color newColor)
        {
            ChangeColour(newColor);
        }

        private void HandleTeamNameUpdated(string oldName, string newName)
        {
            ChangeName(newName);
        }

        private void ChangeColour(Color newColor)
        {
            foreach (Renderer renderer in colorRenderer)
            {
                renderer.material.SetColor("_BaseColor", newColor);
            }
            foreach (TMP_Text text in displayNameTexts)
            {
                text.color = newColor;
            }
        }

        private void ChangeName(string newTeamName)
        {
            foreach (TMP_Text text in displayNameTexts)
            {
                if (TryGetComponent(out ControlItem item))
                {
                    text.text = $"{newTeamName}\n" +
                        $"{item.NameOfItem}";
                }
                else
                {
                    text.text = newTeamName;
                }
            }
        }

        #endregion
    }

}