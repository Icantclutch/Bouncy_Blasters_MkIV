using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerData
{
    public string playerName;
    public int playerNum;

    public int playerElims;
    public int playerDeaths;

    public Team playerTeam;

    public int playerScore;

    public PlayerData(Team team)
    {
        playerTeam = team;
        playerName = "Generic Player Name";
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
}
