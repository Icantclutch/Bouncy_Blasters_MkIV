using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineLobbyButtons : MonoBehaviour
{
   
    [SerializeField]
    private Button _nextMapButton;
    [SerializeField]
    private Text _mapName;
    [SerializeField]
    private List<String> _maps;

    [SerializeField]
    private Button _nextGamemodeButton;
    [SerializeField]
    private Text _gamemodeName;
    private int _gamemodeInt = 0;
    [SerializeField]
    private List<String> _gamemodes;

    [SerializeField]
    private List<Text> _teamDisplay;
    [SerializeField]
    private List<Text> _playerNames;

    [SerializeField]
    private Button _startMatchButton;

    private NetworkManager _networkManager;


    // Start is called before the first frame update
    void Start()
    {
        _startMatchButton = GameObject.Find("Start Match Button").GetComponent<Button>();
        _startMatchButton.interactable = false;
        _startMatchButton.onClick.AddListener(StartMatch);
        if(_maps.Count > 1)
        {
            _nextMapButton.interactable = true;
            _mapName.text = "" + _maps[0];
        }
        else
        {
            if(_maps.Count > 0)
            {
                _mapName.text = "" + _maps[0];
                _networkManager.GetComponent<LobbyManager>().SetMap(_mapName.text);
            }
            _nextMapButton.interactable = false;
        }
        if (_gamemodes.Count > 1)
        {
            _nextGamemodeButton.interactable = true;
            _gamemodeName.text = "" + _gamemodes[0];
        }
        else
        {
            if (_gamemodes.Count > 0)
            {
                _gamemodeName.text = "" + _gamemodes[0];
            }
            _nextGamemodeButton.interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_networkManager)
        {
            _networkManager = GameObject.FindGameObjectWithTag("Management").GetComponent<NetworkManager>();
        }
        else
        {
            _startMatchButton.interactable = true;
            for(int i = 0; i < _playerNames.Count; ++i)
            {
                if (i < _networkManager.GetComponent<LobbyManager>().players.Count) {
                    _playerNames[i].text = _networkManager.GetComponent<LobbyManager>().players[i].playerName;
                    _teamDisplay[i].text = "Team: " + _networkManager.GetComponent<LobbyManager>().players[i].team;
                }
            }
            if (_networkManager.GetComponent<LobbyManager>().gamemodeIndex < _gamemodes.Count)
            {
                _gamemodeName.text = "" + _gamemodes[_networkManager.GetComponent<LobbyManager>().gamemodeIndex];
            }
            _mapName.text = _networkManager.GetComponent<LobbyManager>().mapName;
        }

    }

    private void StartMatch()
    {
        _networkManager.GetComponent<LobbyManager>().SetMap(_mapName.text);
        _networkManager.GetComponent<LobbyManager>().SetGamemode(_gamemodeInt, 30, 0, 420);
        _networkManager.GetComponent<LobbyManager>().StartGame();
    }

    public void CycleTeam(int playerIndex = 0)
    {
        if(_teamDisplay.Count > playerIndex)
            _teamDisplay[playerIndex].text = "Team: " + _networkManager.GetComponent<LobbyManager>().CycleTeam(playerIndex);
    }
    public void CycleMap()
    {
        if (_maps.Count > 0)
        {
            int mapIndex = _maps.IndexOf(_mapName.text);
            if (mapIndex < 0)
            {
                return;
            }
            ++mapIndex;
            if (mapIndex >= _maps.Count)
            {
                mapIndex = 0;
            }
            
            _mapName.text = "" + _maps[mapIndex];
            //Debug.Log(_maps[mapIndex]);
            _networkManager.GetComponent<LobbyManager>().SetMap(_maps[mapIndex]);
        }
    }
    public void CycleGamemode()
    {
        if (_gamemodes.Count > 0)
        {
            ++_gamemodeInt;
            if (_gamemodeInt >= _gamemodes.Count)
            {
                _gamemodeInt = 0;
            }
            _networkManager.GetComponent<LobbyManager>().gamemodeIndex = _gamemodeInt;
            _gamemodeName.text = "" + _gamemodes[_gamemodeInt];
        }
    }
}
