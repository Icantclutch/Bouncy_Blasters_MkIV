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
    private Dropdown _maxGameScore;
    [SerializeField]
    private Dropdown _matchTimer;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
