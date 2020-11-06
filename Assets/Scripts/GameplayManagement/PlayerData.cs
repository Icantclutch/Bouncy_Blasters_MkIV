using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerData : NetworkBehaviour
{
    public string playerName = "";
    public int playerNum;

    public int playerElims;
    public int playerDeaths;

    public Team playerTeam;
    public int team;

    public int playerScore;

    private LobbyManager _lobbyManager;
    private bool inLobby = false;

    public PlayerData(int teamNum = 0, string name = "Name")
    {
        this.team = teamNum;
        playerName = name;
        playerElims = 0;
        playerDeaths = 0;
        playerScore = 0;
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

    [Command]
    private void CmdJoinLobby()
    {
        _lobbyManager.AddPlayer(this);
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
}
