using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Team
{
    public string teamName;
   
   
    public int teamScore;

    public List<PlayerData> playerList;

    public Team(string name, List<PlayerData> players,  int score = 0)
    {
        teamName = name;
        teamScore = score;
        playerList = players;
        foreach(PlayerData player in playerList)
        {
            player.playerTeam = this;
        }
    }


    public void UpdateTeamScore()
    {
        int tempScore = 0;
        for (int i = 0; i < playerList.Count; i++)
        {
            tempScore += playerList[i].playerScore;
        }
        teamScore = tempScore;
        Debug.Log("Team Score has been updated for " + teamName + ": " + teamScore);
        
    }
    
  
}
