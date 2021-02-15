using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGameSettings : MonoBehaviour
{
    //Stored Variables from the Online Lobby Settings Panel
    [SerializeField]
    private Dropdown _playerHealth;
    [SerializeField]
    private Dropdown _playerMovementSpeed;
    [SerializeField]
    private Dropdown _playerJumpHeight;

    [SerializeField]
    private Dropdown _gamemodeSetting;
    [SerializeField]
    private Dropdown _maxGameScore;
    [SerializeField]
    private Dropdown _matchTimer;


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
                foreach(Dropdown d in GetComponentsInChildren<Dropdown>())
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
                foreach (Dropdown d in GetComponentsInChildren<Dropdown>())
                {
                    d.interactable = false;
                }
            }
            _buttonsSetup = true;
        }

        
    }

    public void UpdateClientLobby(int playerHealth, int moveSpeed, int jumpHeight, int gamemode, int maxScore, int time)
    {
        _playerHealth.value = playerHealth;
        _playerMovementSpeed.value = moveSpeed;
        _playerJumpHeight.value = jumpHeight;
        _gamemodeSetting.value = gamemode;
        _maxGameScore.value = maxScore;
        _matchTimer.value = time;
    }

    public int GetGameModeSetting()
    { 
        return _gamemodeSetting.value;
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

    public int GetMatchScoreSetting()
    {
        return int.Parse(_maxGameScore.captionText.text);
    }

    public int GetMatchTimeSetting()
    {
        return int.Parse(_matchTimer.captionText.text);
    }

    public int[] GetDropdownValues()
    {
        int[] values = { _playerHealth.value, _playerMovementSpeed.value, _playerJumpHeight.value, _gamemodeSetting.value, _maxGameScore.value, _matchTimer.value};
        return values;
    }
}
