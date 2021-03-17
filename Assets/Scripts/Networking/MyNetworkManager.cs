using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class MyNetworkManager : NetworkManager
{
    public GameObject gameManager;
    public List<NetworkConnection> players = new List<NetworkConnection>();

    //Overrides OnServerAddPlayer to also get and set the players Steam Id
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        
        CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.lobbyId, numPlayers - 1);

        conn.identity.GetComponent<PlayerData>().SetSteamId(steamId.m_SteamID);
        int[] teams = { 0, 0 };
        foreach(NetworkConnection player in players)
        {
            if(player.identity.GetComponent<PlayerData>().team == 1)
            {
                teams[0]++;
            }
            else if (player.identity.GetComponent<PlayerData>().team == 2)
            {
                teams[1]++;
            }
        }
        if(teams[1] < teams[0])
        {
            conn.identity.GetComponent<PlayerData>().team = 2;
        }
        else
        {
            conn.identity.GetComponent<PlayerData>().team = 1;
        }
        players.Add(conn);
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        GameObject.FindGameObjectWithTag("Management").GetComponent<LobbyManager>().RemovePlayer(conn.identity.GetComponent<PlayerData>());
        players.Remove(conn);
        base.OnServerDisconnect(conn);
        
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        //Debug.Log("Dont Destroy on Load");
        //DontDestroyOnLoad(gameObject);
        if(gameManager)
            NetworkServer.Spawn(Instantiate(gameManager));
    }

    public override void OnStopHost()
    {
        GetComponent<SteamLobby>().button.SetActive(true);
        GetComponent<SteamLobby>().ExitLobby();
        //LeaveSteamLobby();
        base.OnStopHost();
        
    }
    /*[ClientRpc]
    public void LeaveSteamLobby()
    {
        GetComponent<SteamLobby>().ExitLobby();
        StopClient();
    }
    */
    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Client Started");
        //DontDestroyOnLoad(gameObject);
    }

    public override void OnStopClient()
    {

        GetComponent<SteamLobby>().button.SetActive(true);
        GetComponent<SteamLobby>().ExitLobby();
        base.OnStopClient();

        
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        // always become ready.
        //if (!ClientScene.ready) ClientScene.Ready(conn);
        base.OnClientSceneChanged(conn);
        //conn.identity.GetComponent<PlayerData>().RpcSpawnPlayer();
        if (!networkSceneName.Contains("OnlineLobby Scene"))
        {
            conn.identity.GetComponent<PlayerData>().SpawnPlayer(true, true);
        }
        
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        if (!sceneName.Contains("OnlineLobby Scene") && !sceneName.Contains("Tutorial2"))
        {
            GameObject.FindGameObjectWithTag("Management").GetComponent<GameManagement>().StartPreMatch();
          
            /*foreach (NetworkConnection player in players)
            {
                player.identity.GetComponent<PlayerData>().RpcSpawnPlayer();
            }*/
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

    public int GetLocalPlayerTeam()
    {
        foreach (NetworkConnection conn in players)
        {
            if (conn.identity.isLocalPlayer)
            {
                return conn.identity.GetComponent<PlayerData>().team;
            }
        }
        return -1;
    }
}
