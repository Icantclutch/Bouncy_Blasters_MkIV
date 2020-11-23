using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManagement : NetworkBehaviour
{
    //The Teams for the match
    public Team teamA;
    public Team teamB;

    //List of players in Match
    private List<PlayerData> playerList;
    
    //The Selected Gamemode
    public Gamemode matchGamemode;


    //The timer for the ongoing match
    [SyncVar]
    private float _matchTimer;
    //A bool for pausing the match
    private bool _gamePaused;

    /*
     This is a function delegate. This allows the functions to be treated and passed as variables
     The Gamemode class has different function for executing various gamemodes
     And the GameManagement script (This one) gets which gamemode should be executed and thus will 
     call the appropriate function from Gamemode.cs
     */
    private delegate void _matchDelegate();
    _matchDelegate gamemodeExecution;

    public float MatchTimer { get => (int)_matchTimer; }


    // Start is called before the first frame update
    void Start()
    {
        _gamePaused = true;
        playerList = new List<PlayerData>();
        SetUpMatch(new Gamemode(0, 30, 0, 300), new Team("Nova", new List<PlayerData>()), new Team("Super Nova", new List<PlayerData>()));
  
    }
   
    // Update is called once per frame
    void Update()
    {
        if(matchGamemode == null)
        {
            //Allow the match to start
            _gamePaused = true;
        }
        else
        {
            _gamePaused = false;
        }

        //If the game is paused, freeze the match timer
        if (!_gamePaused)
        {
            _matchTimer -= Time.deltaTime;


            //Execute the gamemode specific instructions
            gamemodeExecution();

            //Printing match score
            InvokeRepeating("DebugTeamScore", 5f, 5f);

            //Does a Score and timer check to see if there is a winner
            CheckMatchEnd();
        }

        
    }

    private void DebugTeamScore()
    {
        Debug.Log("Team A Score: " + teamA.teamScore);
        Debug.Log("Team B Score: " + teamB.teamScore);
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

    //Function to be called that sets up the match. THE USE OF THIS FUNCTION MAY CHANGE DEPENDING ON HOW THE MATCH IS LOADED
    public void SetUpMatch(Gamemode game, Team a, Team b)
    {
        matchGamemode = game;
        teamA = a;
        teamB = b;
        for (int i = 0; i < teamA.playerList.Count; i++)
        {
            playerList.Add(teamA.playerList[i]);
        }
        for (int i = 0; i < teamB.playerList.Count; i++)
        {
            playerList.Add(teamB.playerList[i]);
        }


        switch (matchGamemode.selectedGameMode)
        {
            case 0:
                gamemodeExecution = TDM;
                break;
            case 1:
                gamemodeExecution = Hardpt;
                break;
        }


        _matchTimer = (float)matchGamemode.matchTime;
    }

    public void JoinTeam(PlayerData player)
    {
        playerList.Add(player);
        if(player.team == 1)
        {
            teamA.playerList.Add(player);
            player.playerTeam = teamA;
        }
        else if (player.team == 2)
        {
            teamB.playerList.Add(player);
            player.playerTeam = teamB;
        }
    }
    //Functions that act as a single Update() call for a given gamemode
    /* 
   public void GameModeName()
   {
      The operation of the gamemode goes here
   }
   */
    private void TDM()
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            playerList[i].SetPlayerScore(playerList[i].playerElims);
        }

    }

    private void Hardpt()
    {
        Debug.Log("HardPt");
    }
}
