using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{

    [SerializeField]
    private Text _batteryCountText;
    private int _batteryCount;

    [SerializeField]
    private Text _reserveBatteryCountText;
    private int _reserveBatteryCount;

    [SerializeField]
    private Text _teamAScoreText;
    private int _teamAScore;

    [SerializeField]
    private Text _teamBScoreText;
    private int _teamBScore;

    [SerializeField]
    private Text _playerHealthText;
    private int _playerHealth;

    private NetworkManager _networkManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _networkManager = GameObject.FindGameObjectWithTag("Management").GetComponent<NetworkManager>();
        _batteryCount = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentAmmo;
        _reserveBatteryCount = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentReserve;
        //_teamAScore = _networkManager.GetComponent<GameManagement>().teamA.teamScore;
        //_teamBScore = _networkManager.GetComponent<GameManagement>().teamB.teamScore;
        _playerHealth = GetComponent<PlayerHealth>().GetCharge();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        _batteryCountText.text = _batteryCount.ToString();
        _reserveBatteryCountText.text = _reserveBatteryCount.ToString();
        _teamAScoreText.text = _teamAScore.ToString();
        _teamBScoreText.text = _teamBScore.ToString();
        _playerHealthText.text = _playerHealth.ToString();*/
        _networkManager = GameObject.FindGameObjectWithTag("Management").GetComponent<NetworkManager>();
        _batteryCount = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentAmmo;
        _reserveBatteryCount = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentReserve;
        //_teamAScore = _networkManager.GetComponent<GameManagement>().teamA.teamScore;
        //_teamBScore = _networkManager.GetComponent<GameManagement>().teamB.teamScore;
        _playerHealth = GetComponent<PlayerHealth>().GetCharge();
    }
}
