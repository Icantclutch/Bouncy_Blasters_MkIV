﻿using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

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

    [SyncVar]
    public int playerElims;
    [SyncVar]
    public int killStreak;
    [SyncVar]
    public int playerDeaths;
    [NonSerialized]
    public Team playerTeam;
    [SyncVar(hook = nameof(HandleTeamUpdated))]
    public int team;
    public GameObject GO_killStreak;
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
    
    public static bool hostSpawned = false;

    public GameObject firstPersonModels = null;
    public GameObject playerModel = null;
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
        GO_killStreak.GetComponent<Text>().text = "";
        GO_killStreak.SetActive(false);
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
    public void RpcSpawnPlayer(bool partialSpawn, bool prematch)
    {
        GetComponent<PlayerEffects>().LoadingScreen(false);
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

        SpawnPlayer(partialSpawn, prematch);
    }

    public void SpawnPlayer(bool partialSpawn = false, bool prematch = false)
    {
        if (team >= 0)
        {
            if (hasAuthority)
            {
                firstPersonModels.SetActive(true);
                playerModel.SetActive(false);
            }
            //transform.Find("Player").gameObject.SetActive(true);
            KillStreakUpdate();
            GetComponent<Shooting>().enabled = true;
            GetComponent<Shooting>().GetNewLoadout();
            if (!partialSpawn)
            {
                GetComponent<Shooting>().active = true;

                GetComponent<PlayerMovement>().enabled = true;
                GetComponent<PlayerReference>().enabled = true;
            }
            //GetComponent<MouseLook2>().enabled = true;
            GetComponent<PlayerHUD>().enabled = true;
            //Stops the players momentum
            //Should prevent them from falling through the floor
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0);
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
        }
        if (!prematch || partialSpawn)
        {
            StartCoroutine(DelaySpawn());
        }
    }
    IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(0.2f);
        //Debug.Log(_lobbyManager.GetComponent<GameManagement>().hostSpawned);
        if (isServer || _lobbyManager.GetComponent<GameManagement>().hostSpawned)
        {
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
            if (team >= 0)
            {
                GetComponent<MouseLook2>().enabled = true;
            }
            else
            {
                GetComponent<SpectatorMovement>().enabled = true;
            }
        }
    }

    [TargetRpc]
    public void RPCDespawnPlayer()
    {
        //Debug.Log("Attempting to despawn the player by removing their hud and stuff");
        GetComponent<PlayerHUD>().enabled = false;
        GetComponent<PlayerReference>().enabled = false;
        GetComponent<MouseLook2>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<Shooting>().active = false;
        GetComponent<Shooting>().enabled = false;

    }

    private void KillStreakUpdate()
    {
        if (killStreak >= 2)
        {
            GO_killStreak.GetComponent<Text>().text = "Kill Streak x" + killStreak.ToString();
            GO_killStreak.SetActive(true);
        } else
        {
            GO_killStreak.GetComponent<Text>().text = "";
            GO_killStreak.SetActive(false);
        }
    }
    public void AddPlayerElim()
    {
        playerElims += 1;
        killStreak += 1;
        KillStreakUpdate();
    }
    public void AddPlayerDeaths()
    {
        playerDeaths += 1;
        killStreak = 0;
        KillStreakUpdate();
    }
    public void AddPlayerScore(int score)
    {
        playerScore += score;
        //playerTeam.UpdateTeamScore();
    }

    public void SetPlayerScore(int score)
    {
        playerScore = score;
        //playerTeam.UpdateTeamScore();
    }

    public void SetSteamId(ulong steamId)
    {
        _steamId = steamId;
    }

    private void OnDestroy()
    {
        _lobbyManager.RemovePlayer(this);
    }

    public void ResetPlayerStats()
    {
        playerElims = 0;
        playerDeaths = 0;
        playerScore = 0;
        killStreak = 0;
        KillStreakUpdate();
    }

    public static int CompareByScore(PlayerData a, PlayerData b)
    {
        if(a.playerScore > b.playerScore)
        {
            return -1;
        }
        else if(a.playerScore < b.playerScore)
        {
            return 1;
        }
        else 
        { 
            return 0; 
        }
    }
}
