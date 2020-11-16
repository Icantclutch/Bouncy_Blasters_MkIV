using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class MyNetworkManager : NetworkManager
{
    //Overrides OnServerAddPlayer to also get and set the players Steam Id
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.lobbyId, numPlayers - 1);

        conn.identity.GetComponent<PlayerData>().SetSteamId(steamId.m_SteamID);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        conn.identity.GetComponent<PlayerData>().RpcSpawnPlayer();
    }

}
