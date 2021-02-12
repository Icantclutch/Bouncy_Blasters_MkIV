using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class NetworkButtons : MonoBehaviour
{
    [SerializeField]
    private Dropdown _dropdown;

    [SerializeField]
    private string _sceneName = "";
    private NetworkManager _networkManager;
    // Start is called before the first frame update
    void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_networkManager)
        {
            _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            if (_dropdown)
            {
                RefreshLobbyList(_dropdown);
            }
        }
    }

    //Creates a private lobby of the scene (_sceneName)
    public void CreatePrivateLobby()
    {
        if (_networkManager)
        {
            //Sets scene it will change to
            _networkManager.onlineScene = _sceneName;
            //Sets scene it will go back to when the player leaves the lobby
            _networkManager.offlineScene = SceneManager.GetActiveScene().name;
            //Calls function to do the steamworks setup for creating a lobby
            _networkManager.GetComponent<SteamLobby>().HostPrivateLobby();
        }
    }

    //Creates a public lobby of the scene (_sceneName)
    public void CreatePublicLobby()
    {
        if (_networkManager)
        {
            //Sets scene it will change to
            _networkManager.onlineScene = _sceneName;
            //Sets scene it will go back to when the player leaves the lobby
            _networkManager.offlineScene = SceneManager.GetActiveScene().name;
            //Calls function to do the steamworks setup for creating a lobby
            _networkManager.GetComponent<SteamLobby>().HostPublicLobby();
        }
    }

    //Joins any open public lobby
    public void JoinPublicLobby()
    {
        if (_networkManager)
        {
            //Sets scene it will change to
            _networkManager.onlineScene = _sceneName;
            //Sets scene it will go back to when the player leaves the lobby
            _networkManager.offlineScene = SceneManager.GetActiveScene().name;
            //Calls function to do the steamworks setup for joining a lobby
            _networkManager.GetComponent<SteamLobby>().JoinLobby();
        }
    }

    //Updates the lobby list in the dropdown menu
    public void RefreshLobbyList(Dropdown dropdown)
    {
        if (_networkManager)
        {
            //Sets the dropdown to be used for the lobby list
            _networkManager.GetComponent<SteamLobby>().lobbyDropDown = dropdown;
            _networkManager.GetComponent<SteamLobby>().StartRefresh();
        }
    }

    //Joins the lobby currently selected in the dropdown parameter
    public void JoinSelectedLobby(Dropdown dropdown)
    {
        if (_networkManager)
        {
            //Sets the dropdown to be used for the lobby list
            _networkManager.GetComponent<SteamLobby>().lobbyDropDown = dropdown;
            _networkManager.GetComponent<SteamLobby>().JoinSelectedLobby();
        }
    }
}
