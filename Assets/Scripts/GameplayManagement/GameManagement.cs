using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public Team teamA;
    public Team teamB;
      
    public Gamemode matchGamemode;
    
    //The timer for the ongoing match
    private float _matchTimer;
    //A bool for pausing the match
    private bool _gamePaused;

    private delegate void _matchDelegate();
    _matchDelegate matchMode;


    // Start is called before the first frame update
    void Start()
    {
        matchGamemode = new Gamemode(0, 25, 0, 300);


        _matchTimer = (float)matchGamemode.matchTime;
        switch (matchGamemode.selectedGameMode)
        {
            case 0:
                matchMode = matchGamemode.TDM;
                break;
            case 1:
                matchMode = matchGamemode.Hardpt;
                break;
        }
    }
   
    // Update is called once per frame
    void Update()
    {
        //If the game is paused, freeze the match timer
        if (!_gamePaused)
        {
            _matchTimer -= Time.deltaTime;
           
        }
        //Execute the gamemode specific instructions
        matchMode();

        //Does a Score and timer check to see if there is a winner
        CheckMatchEnd();
       


    }

    //Function for checking if the match should end
    private void CheckMatchEnd()
    {
        //If the match timer runs out
        if (_matchTimer <= 0)
        {
            if (teamA.teamScore > teamB.teamScore)
            {
                MatchEnd(teamA);
            }
            else if (teamB.teamScore > teamA.teamScore)
            {
                MatchEnd(teamB);
            }
            else
            {
                MatchEnd();
            }
        }
        //If Team A reaches the score cap
        if (teamA.teamScore >= matchGamemode.maxScore)
        {
            MatchEnd(teamA);
        }
        //If Team B reaches the score cap
        if (teamB.teamScore >= matchGamemode.maxScore)
        {
            MatchEnd(teamB);
        }

    }
    //Function for ending the match and declaring a winner
    private void MatchEnd(Team winningTeam)
    {
        _gamePaused = true;
    }

    //Overloaded Match end for tie game
    private void MatchEnd()
    {
        //Game is Tied
        _gamePaused = true;
    }
}
