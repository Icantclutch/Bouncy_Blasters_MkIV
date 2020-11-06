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

    public PlayerData(Team team, int teamNum = 0, string name = "Name")
    {
        playerTeam = team;
        this.team = teamNum;
        playerName = name;
        playerElims = 0;
        playerDeaths = 0;
        playerScore = 0;
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
