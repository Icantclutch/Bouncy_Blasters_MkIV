using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
