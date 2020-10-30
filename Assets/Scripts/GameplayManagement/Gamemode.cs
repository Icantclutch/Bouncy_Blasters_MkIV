using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemode
{

    public int maxScore;
    public int startingScore;

    public int matchTime;
  
    public int selectedGameMode;
    public Gamemode(int mode, int mScore, int sScore, int time)
    {
        selectedGameMode = mode;
        maxScore = mScore;
        startingScore = sScore;
        matchTime = time;

    
    }

    //Functions that act as a single Update() call for a given gamemode
    /* 
   public void GameModeName()
   {
      The operation of the gamemode goes here
   }
   */
    public void TDM()
    {
        Debug.Log("TDM");
    }

    public void Hardpt()
    {
        Debug.Log("HardPt");
    }
 

}
