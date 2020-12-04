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
    [SyncVar(hook = nameof(HandleTeamUpdated))]
    public int team;
    private void HandleTeamUpdated(int oldTeam, int newTeam)
    {
        if (isLocalPlayer)
        {
            PlayerInfoDisplay.SetLocalPlayerTeam(newTeam);
        }
    }



    [SyncVar]
    public int playerScore;

    private LobbyManager _lobbyManager;
    private bool inLobby = false;
    private bool _spawned = false;
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
        _spawned = false;
        DontDestroyOnLoad(this.gameObject);
        if (isLocalPlayer)
        {
            PlayerInfoDisplay.SetLocalPlayerTeam(team);
        }
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
                _lobbyManager.AddPlayer(this);
                //CmdJoinLobby();
                inLobby = true;
            }
        }
        /*if (!_spawned)
        {
            if (PlayerSpawnSystem.SpawnPlayer(gameObject))
            {
                _spawned = true;
            }
        }*/
    }


    private void CmdJoinLobby()
    {
        _lobbyManager.AddPlayer(this);

    }

    [TargetRpc]
    public void RpcSpawnPlayer()
    {
        //transform.Find("Player").gameObject.SetActive(true);
        PlayerInfoDisplay.SetLocalPlayerTeam(team);

        //GetComponent<Shooting>().enabled = true;
        //GetComponent<Shooting>().active = true;

        //GetComponent<PlayerMovement>().enabled = true;
        //GetComponent<MouseLook2>().enabled = true;
        //GetComponent<PlayerReference>().enabled = true;
        //GetComponent<PlayerHUD>().enabled = true;

        //if (!PlayerSpawnSystem.SpawnPlayer(gameObject, true, true)) 
        //{
        //    if (PlayerSpawnSystem.SpawnPlayer(gameObject))
        //    {
        //        _spawned = true;
        //    }
        //}

        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        //transform.Find("Player").gameObject.SetActive(true);
        GetComponent<Shooting>().enabled = true;
        GetComponent<Shooting>().active = true;

        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<MouseLook2>().enabled = true;
        GetComponent<PlayerReference>().enabled = true;
        GetComponent<PlayerHUD>().enabled = true;

        /*if (!PlayerSpawnSystem.SpawnPlayer(gameObject, true, true)) 
        {
            if (PlayerSpawnSystem.SpawnPlayer(gameObject))
            {
                _spawned = true;
            }
        }
        else
        {
            //_spawned = true;
        }*/
        StartCoroutine(DelaySpawn());
    }
    IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(0.01f);
        if (!PlayerSpawnSystem.SpawnPlayer(gameObject, true, true))
        {
            if (PlayerSpawnSystem.SpawnPlayer(gameObject))
            {
                _spawned = true;
            }
        }
        else
        {
            _spawned = true;
        }
    }

    [TargetRpc]
    public void RPCDespawnPlayer()
    {
        Debug.Log("Attempting to despawn the player by removing their hud and stuff");
        GetComponent<PlayerHUD>().enabled = false;
        GetComponent<PlayerReference>().enabled = false;
        GetComponent<MouseLook2>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<Shooting>().active = false;
        GetComponent<Shooting>().enabled = false;

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
