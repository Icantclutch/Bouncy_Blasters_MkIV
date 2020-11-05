using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using Steamworks;
using System.Diagnostics;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    public struct Player
    {
        public CSteamID steamID;
        public string playerName;
    };
    public struct Lobby
    {
        public CSteamID steamID;
        public string lobbyName;
        public string gamemode;
        public int numOfPlayers;
        public int playerLimit;
        //public List<Player> players;
        //look into if we can get info
        public int ping;
    };

    [SerializeField] private GameObject button = null;
    [SerializeField] private Dropdown lobbyDropDown = null;
    //Callback variables to handle Steam calls
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected CallResult<LobbyMatchList_t> lobbyMatchListCallResult;

    //Constant variables used for setting lobby data
    private const string HostAddressKey = "HostAddress";
    private const string GameKey = "GameName";
    private const string GameName = "BouncyBlasters";

    private NetworkManager networkManager;
    public static CSteamID lobbyId;


    private List<Lobby> lobbies = new List<Lobby>();

    private bool lobbyFound = false;

    

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        lobbyFound = false;

        if (!SteamManager.Initialized) { return; }

        //Link CallBacks and CallResults to the functions that will handle them
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create(OnLobbyMatchList);

    }

    public void HostPublicLobby()
    {
        //Deactivate the host button so they can't press it again
        button.SetActive(false);
        //Create a lobby only friends can join with a max number of players, from the network manager
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManager.maxConnections);
    }
    public void HostPrivateLobby()
    {
        //Deactivate the host button so they can't press it again
        button.SetActive(false);
        //Create a lobby only friends can join with a max number of players, from the network manager
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, networkManager.maxConnections);
    }

    public void JoinLobby()
    {
        //lobbies.Clear();
        
        //Start coroutine to search for lobbies, starting with the most full lobbies
        UnityEngine.Debug.Log("Search for lobbies");
        StartCoroutine(SearchForLobby());
    }
    IEnumerator SearchForLobby()
    {
        lobbies.Clear();
        lobbyDropDown.ClearOptions();
        //Search for lobbies by open slots, starting with 1 open slot
        for (int i = 1; i < networkManager.maxConnections; ++i)
        {
            //Stop searching if an open lobby is found
            if (lobbyFound)
            {
                SteamMatchmaking.JoinLobby(lobbies[0].steamID);
                break;
            }

            //Filter search for lobbies with only i open slots
            UnityEngine.Debug.Log(i + " open spaces");
            SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(i);

            //Make a call request, OnLobbyMatchList() will be called when call is completed
            lobbyMatchListCallResult.Set(SteamMatchmaking.RequestLobbyList());
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(1);
        if (!lobbyFound)
        {
            UnityEngine.Debug.Log("Could not find a lobby, Creating a new lobby");
            HostPublicLobby();
        }

    }

    IEnumerator RefreshLobbyList()
    {
        lobbies.Clear();
        lobbyDropDown.ClearOptions();
        //for (int i = 1; i < networkManager.maxConnections; ++i)
        //{

            //Filter search for lobbies with only i open slots
            //UnityEngine.Debug.Log(i + " open spaces");
            //SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(i);

            //Make a call request, OnLobbyMatchList() will be called when call is completed
            lobbyMatchListCallResult.Set(SteamMatchmaking.RequestLobbyList());
            yield return new WaitForSeconds(1);
        //}
    }
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        //Re-enable button and do nothing else if Steam lobby failed to be made
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            button.SetActive(true);
            return;
        }

        lobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        //Start the host through Mirror 
        networkManager.StartHost();
        
        //Set the Steam lobby data that will be needed by other players in order to join
        SteamMatchmaking.SetLobbyData(lobbyId, HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(lobbyId, GameKey, GameName);
        SteamMatchmaking.SetLobbyData(lobbyId, "LobbyName", networkManager.onlineScene);
    }

    //Handles joining through the Steam interface
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        //UnityEngine.Debug.Log("Join request:" + callback.m_steamIDLobby);
        //Calls OnLobbyEntered()
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    /// <summary>
    /// Handles joining a lobby and staring the client. It is called when there is a SteamMatchmaking.JoinLobby() call
    /// </summary>
    /// <param name="callback">Stores the Lobby's ID</param>
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) { return; }

        //Get the lobby host's Steam ID
        //UnityEngine.Debug.Log(HostAddressKey);
        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        //UnityEngine.Debug.Log(callback.m_ulSteamIDLobby);
        //UnityEngine.Debug.Log(hostAddress);

        //Set the networkAddress to the host's Steam ID and start the client
        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();

        //Disable menu UI
        button.SetActive(false);
    }

    void OnLobbyMatchList(LobbyMatchList_t pCallback, bool bIOFailure)
    {
        //Search through the list of lobbies
        //UnityEngine.Debug.Log(pCallback.m_nLobbiesMatching+ " lobbies found");
        for (int i = 0; i < pCallback.m_nLobbiesMatching; ++i)
        {
            //Make sure lobby is for bouncy blasters
            if (SteamMatchmaking.GetLobbyData(SteamMatchmaking.GetLobbyByIndex(i), GameKey) == GameName)
            {
                //ToDo - Check gamemode
                

                //Join lobby (Calls OnLobbyEntered)
                UnityEngine.Debug.Log("Game Name: " + SteamMatchmaking.GetLobbyData(SteamMatchmaking.GetLobbyByIndex(i), GameKey));
                StoreLobbyInfo(SteamMatchmaking.GetLobbyByIndex(i));
                

                //Sets flag to stop searching
                lobbyFound = true;
            }
        }
    }

    void StoreLobbyInfo(CSteamID lobbyID)
    {
        Lobby lobby = new Lobby();
        lobby.steamID = lobbyID;
        lobby.lobbyName = SteamMatchmaking.GetLobbyData(lobbyID, "LobbyName");
        lobby.gamemode = SteamMatchmaking.GetLobbyData(lobbyID, "Gamemode");
        lobby.numOfPlayers = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
        lobby.playerLimit = SteamMatchmaking.GetLobbyMemberLimit(lobbyID);

        lobbies.Add(lobby);
        if(lobbyDropDown)
            lobbyDropDown.AddOptions(new List<string> { lobby.lobbyName});
    }

    public void StartRefresh()
    {
        StartCoroutine(RefreshLobbyList());
    }

    public void JoinSelectedLobby()
    {
        if (lobbyDropDown && lobbies.Count > 0)
        {
            SteamMatchmaking.JoinLobby(lobbies[lobbyDropDown.value].steamID);
        }
    }
}
