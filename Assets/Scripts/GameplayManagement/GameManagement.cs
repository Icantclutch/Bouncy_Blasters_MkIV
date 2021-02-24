using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class GameManagement : NetworkBehaviour
{
    //The Teams for the match
    public Team teamA;
    public Team teamB;

    [SyncVar]
    public int teamAScore;
    [SyncVar]
    public int teamBScore;

    //List of players in Match
    private List<PlayerData> playerList;
    

    //The Selected Gamemode
    public Gamemode matchGamemode;


    //The timer for the ongoing match
    [SyncVar]
    private float _matchTimer;
    //A bool for pausing the match
    [SerializeField]
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

    //This is a flag for making sure the match isn't constantly resumed
    private bool _startLock = false;

    private NetworkManager _networkManager;

    // Start is called before the first frame update
    
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        _gamePaused = true;
        //playerList = new List<PlayerData>();
        //SetUpMatch(new Gamemode(0, 30, 0, 300), new Team("Nova", new List<PlayerData>()), new Team("Super Nova", new List<PlayerData>()));
  
    }
   
    
    // Update is called once per frame
    
    void Update()
    {
   
        if (matchGamemode == null)
        {
            //Allow the match to start
            PauseMatch();
        }
        else
        {
            if (!_startLock)
            {
                UpdateTeamScores();
                ResumeMatch();
                _startLock = true;
            }
          
        }

        //If the game is paused, freeze the match timer
        if (!_gamePaused)
        {
            _matchTimer -= Time.deltaTime;

            UpdateTeamScores();
            //Execute the gamemode specific instructions
            gamemodeExecution();

            //Printing match score
            //DebugTeamScore();

            //Does a Score and timer check to see if there is a winner
            CheckMatchEnd();
        }
     



    }
    
    public void UpdateTeamScores()
    {
        teamAScore = 0;
        teamBScore = 0;
        for (int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i].team == 1)
            {
                teamAScore += playerList[i].playerScore;
            }
            else if(playerList[i].team == 2)
            {
                teamBScore += playerList[i].playerScore;
            }
        }
    }
    private void DebugTeamScore()
    {
        Debug.Log("Team A Score: " + teamAScore);
        Debug.Log("Team B Score: " + teamBScore);
    }


    //Function for checking if the match should end
  
    private void CheckMatchEnd()
    {
        //If the match timer runs out
        if (_matchTimer <= 0)
        {
            if (teamAScore > teamBScore)
            {
                MatchEnd(1);
            }
            else if (teamBScore > teamAScore)
            {
                MatchEnd(2);
            }
            else
            {
                MatchEnd();
            }
            HeatMap.StoreAndSave();
        }
        //If Team A reaches the score cap
        if (teamAScore >= matchGamemode.maxScore)
        {
            MatchEnd(1);
        }
        //If Team B reaches the score cap
        if (teamBScore >= matchGamemode.maxScore)
        {
            MatchEnd(2);
        }

    }
    //Function for ending the match and declaring a winner
   
    private void MatchEnd(int winningTeam)
    {
        /*
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].playerTeam == winningTeam)
            {
                DeclareWinState(playerList[i], "You Win");
            }
            else
            {
                DeclareWinState(playerList[i], "You Lose");
            }
            
        }
        */
        DeclareWinState(winningTeam);
        Debug.Log("Match Ending and Pausing the Game");
        _gamePaused = true;
        Invoke("ReturnToLobby", 5f);
    }

    //Overloaded Match end for tie game
    
    private void MatchEnd()
    {
        //Game is Tied
        //Debug.Log("Match Ending with a Tie and Pausing the Game");
        DeclareWinState(-1);
        /*
        for(int i = 0; i < playerList.Count; i++)
        {
            DeclareWinState(playerList[i], "Tie Game");
        }
        */
        _gamePaused = true;
        Invoke("ReturnToLobby", 5f);
    }

    private void ReturnToLobby()
    {
        ResetMatch();
       
        GetComponent<LobbyManager>().ReturnPlayers();
        _networkManager.ServerChangeScene("OnlineLobby Scene");
    }

    //Function to be called that sets up the match. THE USE OF THIS FUNCTION MAY CHANGE DEPENDING ON HOW THE MATCH IS LOADED
    public void SetUpMatch(Gamemode game, Team a, Team b)
    {
        playerList = new List<PlayerData>();
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
  
    private void ResetMatch()
    {
        /*
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].GetComponent<PlayerHUD>().DeclareWinState("");
        }
        */
        ResetWinState();
        _startLock = false;
        _gamePaused = true;
        teamAScore = 0;
        teamBScore = 0;
        teamA = null;
        teamB = null;
        playerList = null;
        matchGamemode = null;
        _matchTimer = 300f;
       
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

    
    public void PauseMatch()
    {
        _gamePaused = true;
    }

    
    public void ResumeMatch()
    {
        _gamePaused = false;
    }

    
    [ClientRpc]
    private void DeclareWinState(int winningTeam)
    {
        string state = "Game Over";
        GameObject p = GetComponent<LobbyManager>().GetLocalPlayer();
        if (winningTeam == -1)
        {
            state = "Tie Game";
        }
        else if(winningTeam != 0 && p.GetComponent<PlayerData>().team == winningTeam)
        {
            state = "You Win!";
        }
        else if (winningTeam != 0 && p.GetComponent<PlayerData>().team != winningTeam)
        {
            state = "You Lose!";
        }
        p.GetComponent<PlayerHUD>().DeclareWinState(state);
        //player.GetComponent<PlayerHUD>().DeclareWinState(state);
    }

    [ClientRpc]
    private void ResetWinState()
    {
        GetComponent<LobbyManager>().GetLocalPlayer().GetComponent<PlayerHUD>().DeclareWinState("");
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
