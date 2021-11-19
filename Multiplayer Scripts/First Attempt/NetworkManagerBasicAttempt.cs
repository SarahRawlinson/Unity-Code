using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MultiplayerGame.CustomNetworkManager
{   
    public class NetworkManagerBasicAttempt : NetworkManager
    {
        
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            Debug.Log("Connection to Server Established");
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
            player.SetDisplayName($"Player {numPlayers}");
            
            player.SetDisplayColor(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
            Debug.Log($"Player {player.DisplayName} Has Connected");
            player.RpcLogTest();
        }

        
    }
}
