using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class MyNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.lobbyId, numPlayers - 1);

        conn.identity.GetComponent<PlayerData>().SetSteamId(steamId.m_SteamID);
    }
}
