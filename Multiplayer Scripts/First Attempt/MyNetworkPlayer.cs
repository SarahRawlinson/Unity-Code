using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

namespace MultiplayerGame.CustomNetworkManager
{
    public class MyNetworkPlayer : NetworkBehaviour
    {

        [System.Serializable]
        class CheckName
        {
            [SerializeField] public int maxLengthOfName = 10;
            [SerializeField] public int minLengthOfName = 3;
            [SerializeField] public List<string> illigalNames;
            [SerializeField] public List<char> bandCharactersSymbles;
        }

        [SerializeField] CheckName checkName;
        [SerializeField] private TMP_Text displayNameText;
        [SerializeField] private Renderer displayColourRenderer;
        [SyncVar(hook = nameof(HandleDisplayTextUpdated))]
        [SerializeField] string displayName = "None";
        [SyncVar(hook = nameof(HandleDisplayColourUpdated))]
        [SerializeField] Color displayColor = Color.white;
        public Color DisplayColor { get => displayColor; }
        public string DisplayName { get => displayName; }

        #region Server
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

        [Command] private void CmdSetDisplayName(string newDisplayName)
        {
            if (!CheckNameIsValid(newDisplayName))
            {
                Debug.Log("Name is not allowed");
                return;
            }
            RpcLogNewName(newDisplayName);
            SetDisplayName(newDisplayName);
        }

        [Command]
        private void CmdSetDisplayColor(Color newDisplayColour)
        {
            SetDisplayColor(newDisplayColour);
        }

        
        private bool CheckNameIsValid(string name)
        {
            if (name.Length > checkName.maxLengthOfName || name.Length < checkName.minLengthOfName) return false;
            foreach (char c in name.ToLower())
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
                if (name.ToLower().Contains(bannedName.ToLower())) return false;
            }
            Debug.Log("Name is OK");
            return true;
        }

        #endregion

        #region client
        private void HandleDisplayColourUpdated(Color oldColour, Color newColour)
        {
            displayNameText.color = newColour;
            displayColourRenderer.material.SetColor("_BaseColor", newColour);
        }

        private void HandleDisplayTextUpdated(string oldDisplayName, string newDisplayName)
        {
            displayNameText.text = newDisplayName;
        }

        [ContextMenu("SetMyName")]
        private void SetMyName()
        {
            CmdSetDisplayName("Sarah");
        }

        [ContextMenu("RandomColour")]
        private void RandomColour()
        {
            Color colour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            CmdSetDisplayColor(colour);
        }

        [ClientRpc]
        private void RpcLogNewName(string newDisplayName)
        {
            Debug.Log("Name Logged");
        }

        [TargetRpc]
        public void RpcLogTest()
        {
            Debug.Log("test Logged");
        }

        #endregion
    }
}
