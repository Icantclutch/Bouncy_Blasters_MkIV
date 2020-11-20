using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerData : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleSteamIdUpdated))]
    private ulong _steamId;
    private void HandleSteamIdUpdated(ulong oldSteamId, ulong newSteamId)
    {
        CSteamID steamID = new CSteamID(newSteamId);

        playerName = SteamFriends.GetFriendPersonaName(steamID);
    }

    public string playerName = "";
    public int playerNum;

    public int playerElims;
    public int playerDeaths;
    [NonSerialized]
    public Team playerTeam;
    [SyncVar]
    public int team;

    [SyncVar]
    public int playerScore;

    private LobbyManager _lobbyManager;
    private bool inLobby = false;
    /*
    public PlayerData(int teamNum = 0, string name = "Name")
    {
        this.team = teamNum;
        playerName = name;
        playerElims = 0;
        playerDeaths = 0;
        playerScore = 0;
    }*/
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Update()
    {
        if (!_lobbyManager)
        {
            _lobbyManager = GameObject.FindGameObjectWithTag("Management").GetComponent<LobbyManager>();
        }
        else
        {
            if (!inLobby)
            {
                //_lobbyManager.AddPlayer(this);
                CmdJoinLobby();
                inLobby = true;
            }
        }

    }

    
    private void CmdJoinLobby()
    {
        _lobbyManager.AddPlayer(this);
        if (!_lobbyManager.gameObject.GetComponent<NetworkManager>().onlineScene.Contains("OnlineLobby Scene"))
        {
            GameObject.FindGameObjectWithTag("Management").GetComponent<GameManagement>().JoinTeam(this);
        }
    }

    [ClientRpc]
    public void RpcSpawnPlayer()
    {
        //transform.Find("Player").gameObject.SetActive(true);
        GetComponent<Shooting>().enabled = true;
        GetComponent<Shooting>().active = true;
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<MouseLook2>().enabled = true;
        GetComponent<PlayerReference>().enabled = true;
        GetComponent<PlayerHUD>().enabled = true;
        //if (!PlayerSpawnSystem.SpawnPlayer(gameObject, true, true)) 
        {
            PlayerSpawnSystem.SpawnPlayer(gameObject);
        }
        
    }
    public void AddPlayerElim()
    {
        playerElims += 1;
    }
    public void AddPlayerDeaths()
    {
        playerDeaths += 1;
    }
    public void AddPlayerScore(int score)
    {
        playerScore += score;
        playerTeam.UpdateTeamScore();
    }

    public void SetPlayerScore(int score)
    {
        playerScore = score;
        playerTeam.UpdateTeamScore();
    }

    public void SetSteamId(ulong steamId)
    {
        _steamId = steamId;
    }

    private void OnDestroy()
    {
        _lobbyManager.RemovePlayer(this);
    }
}
