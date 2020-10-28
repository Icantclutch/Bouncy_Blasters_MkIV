using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team
{
    public string teamName;
   

    public int teamScore;

    public List<PlayerData> playerList;

    public Team(string name, int score, List<PlayerData> players)
    {
        teamName = name;
        teamScore = score;
        playerList = players;
    }


    public void UpdateTeamScore()
    {
        int tempScore = 0;
        for (int i = 0; i < playerList.Count; i++)
        {
            tempScore += playerList[i].playerScore;
        }
        teamScore = tempScore;
    }
    
  
}
