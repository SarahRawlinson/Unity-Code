using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerRTS.Networking
{
    public partial class RTSNetworkPlayer
    {
        [System.Serializable]
        private class CheckName
        {
            [SerializeField] public int maxLengthOfName = 10;
            [SerializeField] public int minLengthOfName = 3;
            [SerializeField] public List<string> illigalNames;
            [SerializeField] public List<char> bandCharactersSymbles;
        }
    }
}