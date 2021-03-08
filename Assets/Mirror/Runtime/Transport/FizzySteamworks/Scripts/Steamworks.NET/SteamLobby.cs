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
    //Might be used in the future to display player info
    public struct Player
    {
        public CSteamID steamID;
        public string playerName;
    };

    //Holds lobby info related to matchmaking
    public struct Lobby
    {
        public CSteamID steamID;
        public string lobbyName;
        public string gamemode;
        public string map;
        public int numOfPlayers;
        public int playerLimit;
        //public List<Player> players;
        //look into if we can get info for:
        public int ping;
    };

    //references to objects in the scene
    [SerializeField] public GameObject button = null;
    [SerializeField] public Dropdown lobbyDropDown = null;
    [SerializeField] public ToggleGroup _lobbyToggles = null;
    [SerializeField] public GameObject lobbyPrefab = null;

    //Callback variables to handle Steam calls
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected CallResult<LobbyMatchList_t> lobbyMatchListCallResult;

    //Constant variables used for setting lobby data
    private const string HostAddressKey = "HostAddress";
    private const string GameKey = "GameName";
    private string GameName = "BouncyBlasters";

    private NetworkManager networkManager;
    public static CSteamID lobbyId;


    private List<Lobby> lobbies = new List<Lobby>();
    private int _selectedLobby = 0;


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
        //GameName += Application.version;
    }

    //Makes a CreateLobby call for a public lobby
    public void HostPublicLobby()
    {
        //Deactivate the host button so they can't press it again
        button.SetActive(false);
        //Create a lobby only friends can join with a max number of players, from the network manager
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManager.maxConnections);
    }

    //Makes a CreateLobby call for a private lobby
    public void HostPrivateLobby()
    {
        //Deactivate the host button so they can't press it again
        button.SetActive(false);
        //Create a lobby only friends can join with a max number of players, from the network manager
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, networkManager.maxConnections);
    }

    //Starts the coroutine to find and join an open lobby
    public void JoinLobby()
    {
        //lobbies.Clear();
        
        //Start coroutine to search for lobbies, starting with the most full lobbies
        UnityEngine.Debug.Log("Search for lobbies");
        StartCoroutine(SearchForLobby());
    }

    public void ExitLobby()
    {
        SteamMatchmaking.LeaveLobby(lobbyId);
    }

    //Coroutine to search for an open lobby and join it, if no lobby is found then it will create a lobby
    IEnumerator SearchForLobby()
    {
        lobbies.Clear();
        lobbyFound = false;
        //ClearLobbyBrowser();
        //lobbyDropDown.ClearOptions();
        //Search for lobbies by open slots, starting with 1 open slot
        for (int i = 1; i < networkManager.maxConnections; ++i)
        {
            //Stop searching if an open lobby is found
            if (lobbyFound)
            {
                UnityEngine.Debug.Log(lobbies[0]);
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

    //Coroutine to refresh the lobby list info
    IEnumerator RefreshLobbyList()
    {
        //Reset List and dropdown menu
        
        lobbyDropDown.ClearOptions();
        //ClearLobbyBrowser();
        //Make a call request, OnLobbyMatchList() will be called when call is completed
        lobbyMatchListCallResult.Set(SteamMatchmaking.RequestLobbyList());
        yield return new WaitForSeconds(1);
        DisplayLobbies();
    }

    private void ClearLobbyBrowser()
    {
        if (_lobbyToggles)
        {
            int size = _lobbyToggles.transform.childCount;
            for (int i = 0; i < size; ++i)
            {
                _lobbyToggles.UnregisterToggle(_lobbyToggles.transform.GetChild(i).GetComponent<Toggle>());
                Destroy(_lobbyToggles.transform.GetChild(i).gameObject);
            }
        }
    }

    //Called when a CreateLobby call returns
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
        SteamMatchmaking.SetLobbyData(lobbyId, "LobbyName", SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID()) + "'s Lobby");
        SteamMatchmaking.SetLobbyData(lobbyId, "Gamemode", "Pre-game");
        SteamMatchmaking.SetLobbyData(lobbyId, "Map", "Lobby");
    }

    
    public void SetLobbyMatchData(string gamemode, string map)
    {
        SteamMatchmaking.SetLobbyData(lobbyId, "Gamemode", gamemode);
        SteamMatchmaking.SetLobbyData(lobbyId, "Map", map);
    }

    //Handles joining through the Steam interface
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        //UnityEngine.Debug.Log("Join request:" + callback.m_steamIDLobby);
        //Calls OnLobbyEntered()
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    
    // Handles joining a lobby and staring the client. It is called when there is a SteamMatchmaking.JoinLobby() call
    //callback: Stores the Lobby's ID
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

    //Called when a RequestLobbyList call returns with results
    void OnLobbyMatchList(LobbyMatchList_t pCallback, bool bIOFailure)
    {
        //Search through the list of lobbies
        //UnityEngine.Debug.Log(pCallback.m_nLobbiesMatching+ " lobbies found");
        lobbies.Clear();
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

    

    //Reads in lobby info into a Lobby struct and adds it to the lobby list
    void StoreLobbyInfo(CSteamID lobbyID)
    {
        Lobby lobby = new Lobby();
        lobby.steamID = lobbyID;
        lobby.lobbyName = SteamMatchmaking.GetLobbyData(lobbyID, "LobbyName");
        lobby.gamemode = SteamMatchmaking.GetLobbyData(lobbyID, "Gamemode");
        lobby.map = SteamMatchmaking.GetLobbyData(lobbyID, "Map");
        lobby.numOfPlayers = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
        lobby.playerLimit = SteamMatchmaking.GetLobbyMemberLimit(lobbyID);

        lobbies.Add(lobby);
        if(lobbyDropDown)
            lobbyDropDown.AddOptions(new List<string> { lobby.lobbyName + " " + lobby.numOfPlayers + "/" + lobby.playerLimit});
        

        
    }
    void DisplayLobbies()
    {
        ClearLobbyBrowser();
        if (_lobbyToggles)
        {
            int count = 0;
            foreach (Lobby lobby in lobbies)
            {
                GameObject b = Instantiate(lobbyPrefab, _lobbyToggles.transform);
                _lobbyToggles.RegisterToggle(b.GetComponent<Toggle>());
                Text[] lobbyInfo = b.GetComponentsInChildren<Text>();
                lobbyInfo[0].text = lobby.lobbyName;
                lobbyInfo[1].text = lobby.numOfPlayers + "/" + lobby.playerLimit;
                lobbyInfo[2].text = lobby.gamemode;
                lobbyInfo[3].text = lobby.map;
                b.GetComponent<Toggle>().group = _lobbyToggles;

                if (count == 0)
                {
                    b.GetComponent<Toggle>().Select();
                }
                count++;
            }
        }
    }
    //Starts the coroutine to refresh the lobby list in the dropdown menu
    public void StartRefresh()
    {
        StartCoroutine(RefreshLobbyList());
    }

    //Joins the lobby currently selected in the dropdown menu
    public void JoinSelectedLobby()
    {
        if (_lobbyToggles && lobbies.Count > 0)
        {

            IEnumerator<Toggle> toggles = _lobbyToggles.ActiveToggles().GetEnumerator();
            toggles.MoveNext();
            for (int i = 0; i < lobbies.Count; ++i)
            {
                if (toggles.Current && toggles.Current.isOn)
                {
                    SteamMatchmaking.JoinLobby(lobbies[i].steamID);
                    break;
                }
                toggles.MoveNext();
            }
        }
    }
}
