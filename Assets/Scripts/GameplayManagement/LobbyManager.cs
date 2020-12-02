using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyManager : NetworkBehaviour
{
    //List of players in the lobby
    public List<PlayerData> players;

    //variables used to setup the game aspects
    [SyncVar]
    public int gamemodeIndex = 0;
    private Gamemode gamemode;
    private Team teamA, teamB;
    
    //Reference to the NetworkManager on the same gameobject
    private NetworkManager networkManager;

    
    public int minPlayersNeeded = 2;
    public int numOfTeams = 2;
    [SyncVar(hook = nameof(SetMap))]
    public string mapName = "RicochetTest";

    public GameManagement gameManager;

    //Object for the match settings
    private LobbyGameSettings _lobbySettings;

    // Start is called before the first frame update
    void Start()
    {
        players = new List<PlayerData>();
        networkManager = GetComponent<NetworkManager>();
       
        
    }


    public void AddPlayer(PlayerData player)
    {
        players.Add(player);
        DisplayPlayers();
        //Debug.Log(networkManager.onlineScene);
        //Used for backwards compatability with testing scenes
        if(!networkManager.onlineScene.Contains("OnlineLobby Scene"))
        {
            player.RpcSpawnPlayer();
        }
    }

    public void RemovePlayer(PlayerData player)
    {
        players.Remove(player);
        DisplayPlayers();
    }
    public void SetGamemode(int mode, int mScore, int sScore, int time)
    {
        gamemode = new Gamemode(mode, mScore, sScore, time);
    }

    public void SetGamemode()
    {
        GetLobbySettings();
        gamemode = new Gamemode(_lobbySettings.GetGameModeSetting(), _lobbySettings.GetMatchScoreSetting(), 0, _lobbySettings.GetMatchTimeSetting());
        Debug.Log("LobbyManager is setting the gamemode to: " + _lobbySettings.GetGameModeSetting());
        Debug.Log("LobbyManager is setting the gamemode Max score to : " + _lobbySettings.GetMatchScoreSetting());
        Debug.Log("LobbyManager is setting the gamemode time to: " + _lobbySettings.GetMatchTimeSetting());
    }

    public void SetMap(string oldMap, string map)
    {
        mapName = map;
    }
    
    /*Takes in the gamemode set by the settings. 
     * After setting the gamemode it takes the players and assigns them to teams
     * These teams get passed into the GameMangement's StartGame Function
     */
    public void StartGame()
    {
        //To-do: check if is host

        if(networkManager.numPlayers >= minPlayersNeeded)
        {
            //To-do:
            //Set up components needed for gamemode
            //Create Gamemode: default of DeathMatch temporarily
           
            
            //Create teams
            List<PlayerData> teamAPlayers = new List<PlayerData>();
            List<PlayerData> teamBPlayers = new List<PlayerData>();
            foreach (PlayerData player in players)
            {
                if(player.team == 1)
                {
                    teamAPlayers.Add(player);
                }
                else if (player.team == 2)
                {
                    teamBPlayers.Add(player);
                }
            }
            teamA = new Team("Nova",teamAPlayers);
            teamB = new Team("Super Nova", teamBPlayers);
            
            //Setup Game Management
            gameManager.SetUpMatch(gamemode, teamA, teamB);


            Debug.Log("Enabling player gameobjects");
            //SpawnPlayers
            SpawnPlayers();

            //Change scene
            networkManager.ServerChangeScene(mapName);
        }
    }

    //Cycles a players team based on the numOfTeams
    public int CycleTeam(int playerIndex = 0)
    {
        if (players.Count > playerIndex) {
            players[playerIndex].team++;
            if(players[playerIndex].team > numOfTeams)
            {
                players[playerIndex].team = 0;
            }
            return players[playerIndex].team;
        }
        //Returns -1 if it could not increment correctly
        return -1;
    }

    //Not implemented, will be used
    public void DisplayPlayers()
    {
        //Loop through all of the current players
        /*foreach(PlayerData player in players)
        {
            //Set UI components to display name and team
        }*/
    }

    //Run functions to spawn all of the players
    public void SpawnPlayers()
    {
        //SpawnPlayers
        foreach (PlayerData player in players)
        {
            player.RpcSpawnPlayer();
            player.GetComponent<Shooting>().Rpc_FullReload();
        }
    }

    private void GetLobbySettings()
    {
        if (_lobbySettings == null)
        {
            _lobbySettings = GameObject.FindGameObjectWithTag("Settings").GetComponent<LobbyGameSettings>();
        }
    }
}
