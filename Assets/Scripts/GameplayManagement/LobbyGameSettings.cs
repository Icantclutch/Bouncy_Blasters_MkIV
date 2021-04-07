﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGameSettings : MonoBehaviour
{
    //Stored Variables from the Online Lobby Settings Panel
    [SerializeField]
    private Dropdown _playerHealth = null;
    [SerializeField]
    private Dropdown _playerMovementSpeed = null;
    [SerializeField]
    private Dropdown _playerJumpHeight = null;
    [SerializeField]
    private Dropdown _playerRespawnTime = null;

    [SerializeField]
    private Dropdown _gamemodeSetting = null;
    [SerializeField]
    private Dropdown _maxGameScore = null;
    [SerializeField]
    private Dropdown _matchTimer = null;
    [SerializeField]
    private GameObject _OverchargeLabel = null;

    [SerializeField]
    private Text _TwitchUsername = null;
    [SerializeField]
    private Text _TwitchOauth = null;

    [SerializeField]
    private OnlineLobbyButtons _lobbyButtons = null;

    private GameObject _gameManager;
    private bool _buttonsSetup = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameManager)
        {
            _gameManager = GameObject.FindGameObjectWithTag("Management");
        }
        else if(!_buttonsSetup)
        {
            //Enable buttons only for the host
            if (_gameManager.GetComponent<LobbyManager>().isServer)
            {
                _gamemodeSetting.interactable = true;
                foreach (Button button in GetComponentsInChildren<Button>())
                {
                    button.interactable = true;
                }
                foreach (InputField Input in GetComponentsInChildren<InputField>())
                {
                    Input.interactable = true;
                }
                foreach (Dropdown d in GetComponentsInChildren<Dropdown>())
                {
                    d.interactable = true;
                }
            }
            else
            {
                _gamemodeSetting.interactable = false;
                foreach (Button button in GetComponentsInChildren<Button>())
                {
                    button.interactable = false;
                }
                foreach (InputField Input in GetComponentsInChildren<InputField>())
                {
                    Input.interactable = false;
                    //Set placeholder text to "Host | Hidden"
                    Text PlaceHolderUsername = Input.transform.Find("Placeholder").GetComponent<Text>();
                    Text PlaceHolderOauth = Input.transform.Find("Placeholder").GetComponent<Text>();
                    PlaceHolderOauth.text = "Host Only | Hidden";
                    PlaceHolderUsername.text = "Host Only | Hidden";
                }
                foreach (Dropdown d in GetComponentsInChildren<Dropdown>())
                {
                    d.interactable = false;
                }
               
                _OverchargeLabel.GetComponentInChildren<Dropdown>().interactable = false;

            }
            _buttonsSetup = true;
        }

        
    }

    public void UpdateClientLobby(int playerHealth, int moveSpeed, int jumpHeight, int gamemode, int maxScore, int time, int respawnTime, int overchargeTime)
    {
        _playerHealth.value = playerHealth;
        _playerMovementSpeed.value = moveSpeed;
        _playerJumpHeight.value = jumpHeight;
        _gamemodeSetting.value = gamemode;
     
        _matchTimer.value = time;
        _playerRespawnTime.value = respawnTime;

        if(gamemode == 0)
        {
            _OverchargeLabel.SetActive(false);
        }
        else if(gamemode == 1)
        {
            _OverchargeLabel.SetActive(true);
            _OverchargeLabel.GetComponentInChildren<Dropdown>().value = overchargeTime;
        }

        _maxGameScore.value = maxScore;
    }

    public int GetGameModeSetting()
    { 
        return _gamemodeSetting.value;
    }
    public string GetGameModeName()
    {
        return _gamemodeSetting.captionText.text;
    }
    public int GetPlayerHealthSetting()
    {
        return int.Parse(_playerHealth.captionText.text);
    }

   public float GetPlayerSpeedSetting()
    {
        return float.Parse(_playerMovementSpeed.captionText.text);
    }

    public float GetPlayerJumpHeightSetting()
    {
        return float.Parse(_playerJumpHeight.captionText.text);
    }
    public float GetPlayerRespawnTimeSetting()
    {
        return float.Parse(_playerRespawnTime.captionText.text);
    }

    public int GetMatchScoreSetting()
    {
        return int.Parse(_maxGameScore.captionText.text);
    }

    public string GetTwitchUsername()
    {
        return _TwitchUsername.text;
    }

    public string GetTwitchOauth()
    {
        return _TwitchOauth.text;
    }

    public int GetMatchTimeSetting()
    {
        if (_matchTimer.captionText.text == "Infinite")
        {
            return int.MaxValue;
        }
        else
        {
            return int.Parse(_matchTimer.captionText.text);
        }
    }

    public int GetOverchargeTimeSetting()
    {
        return int.Parse(_OverchargeLabel.GetComponentInChildren<Dropdown>().captionText.text);
    }

    public int[] GetDropdownValues()
    {
        int[] values = { _playerHealth.value, _playerMovementSpeed.value, _playerJumpHeight.value, _gamemodeSetting.value, _maxGameScore.value, _matchTimer.value, _playerRespawnTime.value, _OverchargeLabel.GetComponentInChildren<Dropdown>().value};
        return values;
    }

    public void UpdatePlayerDisplayLocation(int playerIndex, int team)
    {
        _lobbyButtons.UpdateDisplayLocation(playerIndex, team);
    }
}
