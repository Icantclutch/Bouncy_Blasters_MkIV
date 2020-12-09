using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{

    [SerializeField]
    private Text _batteryCountText = null;

    [SerializeField]
    private Text _reserveBatteryCountText = null;

    [SerializeField]
    private Text _teamAScoreText = null;
    
    [SerializeField]
    private Text _teamBScoreText = null;
  
    [SerializeField]
    private Text _playerHealthText = null;
  
    [SerializeField]
    private Text _playerWeaponText = null;

    [SerializeField]
    private Text _matchTimer = null;
    [SerializeField]
    private NetworkManager _networkManager = null;

    private GameObject _gameManager = null;

    [SerializeField]
    private Text _matchEndText = null;
    
    // Start is called before the first frame update
    void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        _gameManager = GameObject.FindGameObjectWithTag("Management");
    }

    // Update is called once per frame
    void Update()
    {
        if(_networkManager == null)
        {
            _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        }
        else
        {
            _batteryCountText.text = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentAmmo.ToString();
            _reserveBatteryCountText.text = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentReserve.ToString();
            _playerHealthText.text = GetComponent<PlayerHealth>().GetCharge().ToString();
            _playerWeaponText.text = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].weapon.name;

            _teamAScoreText.text = _gameManager.GetComponentInChildren<GameManagement>().teamAScore.ToString();
            _teamBScoreText.text = _gameManager.GetComponentInChildren<GameManagement>().teamBScore.ToString();
            _matchTimer.text = _gameManager.GetComponentInChildren<GameManagement>().MatchTimer.ToString();
        }
      
    }

    
    public void DeclareWinState(string state)
    {
       _matchEndText.text = state;
        
    }
}
