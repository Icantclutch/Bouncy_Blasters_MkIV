using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class MyNetworkManager : NetworkManager
{
    public List<NetworkConnection> players = new List<NetworkConnection>();
    //Overrides OnServerAddPlayer to also get and set the players Steam Id
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        
        CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.lobbyId, numPlayers - 1);

        conn.identity.GetComponent<PlayerData>().SetSteamId(steamId.m_SteamID);
        players.Add(conn);
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        players.Remove(conn);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        //conn.identity.GetComponent<PlayerData>().RpcSpawnPlayer();
        if (!networkSceneName.Contains("OnlineLobby Scene"))
        {
            conn.identity.GetComponent<PlayerData>().SpawnPlayer();
        }
    }

    public GameObject GetLocalPlayer()
    {
        foreach(NetworkConnection conn in players)
        {
            if(conn.identity.isLocalPlayer)
            {
                return conn.identity.gameObject;
            }
        }
        return null;
    }
}
