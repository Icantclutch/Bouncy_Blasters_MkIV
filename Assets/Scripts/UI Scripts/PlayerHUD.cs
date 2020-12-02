using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{

    [SerializeField]
    private Text _batteryCountText;

    [SerializeField]
    private Text _reserveBatteryCountText;

    [SerializeField]
    private Text _teamAScoreText;
    
    [SerializeField]
    private Text _teamBScoreText;
  
    [SerializeField]
    private Text _playerHealthText;
  
    [SerializeField]
    private Text _playerWeaponText;

    [SerializeField]
    private Text _matchTimer;
    [SerializeField]
    private NetworkManager _networkManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _networkManager = GameObject.FindGameObjectWithTag("Management").GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_networkManager == null)
        {
            _networkManager = GameObject.FindGameObjectWithTag("Management").GetComponent<NetworkManager>();
        }
        else
        {
            _batteryCountText.text = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentAmmo.ToString();
            _reserveBatteryCountText.text = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentReserve.ToString();
            _playerHealthText.text = GetComponent<PlayerHealth>().GetCharge().ToString();
            _playerWeaponText.text = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].weapon.name;

            _teamAScoreText.text = _networkManager.GetComponentInChildren<GameManagement>().teamAScore.ToString();
            _teamBScoreText.text = _networkManager.GetComponentInChildren<GameManagement>().teamBScore.ToString();
            _matchTimer.text = _networkManager.GetComponentInChildren<GameManagement>().MatchTimer.ToString();
        }
      
     
        

    }
}
