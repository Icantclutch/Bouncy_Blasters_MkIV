using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemode
{
    public string gamemodeName;
    public List<Team> teamList;

    public int maxScore;
    public int startingScore;

    public int matchTime;

    public Gamemode(string modeName,int mScore, int sScore, int time, List<Team> teams)
    {
        gamemodeName = modeName;
        maxScore = mScore;
        startingScore = sScore;
        matchTime = time;
        teamList = teams;
    }

   
    /* 
     public void GameModeName()
     {
        The operation of the gamemode goes here
     }
     */

}
