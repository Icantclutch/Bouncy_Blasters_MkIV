using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyManager : NetworkBehaviour
{
    public List<PlayerData> players;
    public Gamemode gamemode;

    private NetworkManager networkManager;

    public int minPlayersNeeded = 2;
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

    public void StartGame()
    {
        //To-do: check if is host


        if(networkManager.numPlayers >= minPlayersNeeded)
        {
            //To-do:
            //Set up components needed for gamemode
            //Change scene
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
