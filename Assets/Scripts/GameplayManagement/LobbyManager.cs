using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyManager : NetworkBehaviour
{
    private List<PlayerData> players;
    private Gamemode gamemode;
    private Team teamA, teamB;
    

    private NetworkManager networkManager;

    public int minPlayersNeeded = 2;
    public string mapName = "";

    public GameManagement gameManager;
    // Start is called before the first frame update
    void Start()
    {
        players = new List<PlayerData>();
        networkManager = GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlayer(PlayerData player)
    {
        players.Add(player);
        DisplayPlayers();
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

    public void SetMap(string map)
    {
        mapName = map;
    }
    public void StartGame()
    {
        //To-do: check if is host


        if(networkManager.numPlayers >= minPlayersNeeded)
        {
            //To-do:
            //Set up components needed for gamemode
            //Create Gamemode: default of DeathMatch temporarily
            gamemode = new Gamemode(0, 30, 0, 420);
            //Create teams
            List<PlayerData> teamPlayers = new List<PlayerData>();
            foreach (PlayerData player in players)
            {
                if(player.team == 1)
                {
                    teamPlayers.Add(player);
                }
            }
            teamA = new Team("Nova",teamPlayers);
            teamPlayers.Clear();
            foreach (PlayerData player in players)
            {
                if (player.team == 2)
                {
                    teamPlayers.Add(player);
                }
            }
            teamB = new Team("Super Nova", teamPlayers);
            //Setup Game Management
            gameManager.SetUpMatch(gamemode, teamA, teamB);

            //Change scene
            networkManager.ServerChangeScene(mapName);
            //SpawnPlayers

        }
    }

    public void DisplayPlayers()
    {
        foreach(PlayerData player in players)
        {
            //Set UI components to display name and team
        }
    }
}
