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

    public Gamemode()
    {
        selectedGameMode = 0;
        maxScore = 30;
        startingScore = 0;
        matchTime = 223;
    }

   
 

}
