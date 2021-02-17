using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineLobbyButtons : MonoBehaviour
{
   
    [SerializeField]
    private Button _nextMapButton = null;
    [SerializeField]
    private Text _mapName = null;
    [SerializeField]
    private List<String> _maps = null;

    [SerializeField]
    private Button _nextGamemodeButton = null;
    [SerializeField]
    private Text _gamemodeName = null;
    private int _gamemodeInt = 0;
    [SerializeField]
    private List<String> _gamemodes = null;

    [SerializeField]
    private List<Text> _teamDisplay = null;
    [SerializeField]
    private List<Text> _playerNames = null;

    [SerializeField]
    private Button _startMatchButton = null;

    private NetworkManager _networkManager;
    private GameObject _gameManager;

    private bool _buttonsSetup = false;

    [SerializeField]
    private GameObject _playerSlot;
    [SerializeField]
    private Transform _playerListLocation;

    // Start is called before the first frame update
    void Start()
    {
        
        _startMatchButton.interactable = false;
        _startMatchButton.onClick.AddListener(StartMatch);
        if(_maps.Count > 1)
        {
            
            _nextMapButton.interactable = true;
            _mapName.text = "" + _maps[0];
          
            //Debug.Log("Default Map: " + _mapName.text);
        }
        else
        {
            if(_maps.Count > 0)
            {
                _mapName.text = "" + _maps[0];
                _gameManager.GetComponent<LobbyManager>().SetMap(_mapName.text);
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
            _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        }
        if (!_gameManager)
        {
            _gameManager = GameObject.FindGameObjectWithTag("Management");
        }
        else if (!_buttonsSetup)
        {
            //Enable buttons only for the host
            if (_gameManager.GetComponent<LobbyManager>().isServer)
            {
                _startMatchButton.interactable = true;
                _nextGamemodeButton.interactable = true;
                _nextMapButton.interactable = true;
                foreach(Button button in GetComponentsInChildren<Button>())
                {
                    button.interactable = true;
                }
            }
            else
            {
                _startMatchButton.interactable = false;
                _nextGamemodeButton.interactable = false;
                _nextMapButton.interactable = false;
                foreach (Button button in GetComponentsInChildren<Button>())
                {
                    button.interactable = false;
                }
            }

            for(int i = 0; i < _playerNames.Count; ++i)
            {
                if (i < _gameManager.GetComponent<LobbyManager>().players.Count) {
                    _playerNames[i].text = _gameManager.GetComponent<LobbyManager>().players[i].playerName;
                    _teamDisplay[i].text = "Team: " + _gameManager.GetComponent<LobbyManager>().players[i].team;
                }
            }
            if (_gameManager.GetComponent<LobbyManager>().gamemodeIndex < _gamemodes.Count)
            {
                _gamemodeName.text = "" + _gamemodes[_gameManager.GetComponent<LobbyManager>().gamemodeIndex];
            }
            //_mapName.text = _networkManager.GetComponentInChildren<LobbyManager>().mapName;
        }

        if(_gameManager && _playerNames.Count < _gameManager.GetComponent<LobbyManager>().players.Count)
        {
            AddPlayer();
        }
        else if(_gameManager && _playerNames.Count > _gameManager.GetComponent<LobbyManager>().players.Count){
            RemovePlayer();
        }

    }

    public void AddPlayer()
    {
        GameObject b = Instantiate(_playerSlot, _playerListLocation);
        _playerNames.Add(b.GetComponent<Text>());
        _teamDisplay.Add(b.GetComponentsInChildren<Text>()[1]);
        b.GetComponentInChildren<Button>().onClick.AddListener(delegate{ CycleTeam(_playerNames.Count - 1); });
    }

    public void RemovePlayer()
    {
        int i = _playerNames.Count - 1;
        GameObject b = _playerNames[i].gameObject;
        _playerNames.RemoveAt(i);
        _teamDisplay.RemoveAt(i);
        Destroy(b);
    }

    private void StartMatch()
    {
        _gameManager.GetComponent<LobbyManager>().SetMap(_mapName.text);
        _gameManager.GetComponent<LobbyManager>().SetGamemode();
        _gameManager.GetComponent<LobbyManager>().StartGame();
    }

    public void CycleTeam(int playerIndex = 0)
    {
        if(_teamDisplay.Count > playerIndex)
            _teamDisplay[playerIndex].text = "Team: " + _gameManager.GetComponent<LobbyManager>().CycleTeam(playerIndex);
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
            _gameManager.GetComponent<LobbyManager>().SetMap(_maps[mapIndex]);
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
            _gameManager.GetComponent<LobbyManager>().gamemodeIndex = _gamemodeInt;
            _gamemodeName.text = "" + _gamemodes[_gamemodeInt];
        }
    }

    public void LeaveLobby()
    {
        _gameManager.GetComponent<LobbyManager>().LeaveLobby();
    }
}
