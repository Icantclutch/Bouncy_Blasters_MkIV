using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class NetworkButtons : MonoBehaviour
{
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
        }
    }

    public void CreatePrivateLobby()
    {
        if (_networkManager)
        {
            _networkManager.onlineScene = _sceneName;
            _networkManager.offlineScene = SceneManager.GetActiveScene().name;
            _networkManager.GetComponent<SteamLobby>().HostPrivateLobby();
        }
    }
    public void CreatePublicLobby()
    {
        if (_networkManager)
        {
            _networkManager.onlineScene = _sceneName;
            _networkManager.offlineScene = SceneManager.GetActiveScene().name;
            _networkManager.GetComponent<SteamLobby>().HostPublicLobby();
        }
    }
    public void JoinPublicLobby()
    {
        if (_networkManager)
        {
            _networkManager.onlineScene = _sceneName;
            _networkManager.offlineScene = SceneManager.GetActiveScene().name;
            _networkManager.GetComponent<SteamLobby>().JoinLobby();
        }
    }
    public void RefreshLobbyList(Dropdown dropdown)
    {
        if (_networkManager)
        {
            _networkManager.GetComponent<SteamLobby>().lobbyDropDown = dropdown;
            _networkManager.GetComponent<SteamLobby>().StartRefresh();
        }
    }

    public void JoinSelectedLobby(Dropdown dropdown)
    {
        if (_networkManager)
        {
            _networkManager.GetComponent<SteamLobby>().lobbyDropDown = dropdown;
            _networkManager.GetComponent<SteamLobby>().JoinSelectedLobby();
        }
    }
}
