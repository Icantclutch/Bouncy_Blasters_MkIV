using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{

    [SerializeField]
    private Text _batteryCountText;

    private Weapon wep;

    [SerializeField]
    private Text _reserveBatteryCountText;
   

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

        //_teamAScore = _networkManager.GetComponent<GameManagement>().teamA.teamScore;
        //_teamBScore = _networkManager.GetComponent<GameManagement>().teamB.teamScore;
        //_playerHealth = GetComponent<PlayerHealth>().GetCharge();
    }

    // Update is called once per frame
    void Update()
    {

        _batteryCountText.text = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentAmmo.ToString();
        _reserveBatteryCountText.text = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentReserve.ToString();
        //_teamAScoreText.text = _teamAScore.ToString();
        //_teamBScoreText.text = _teamBScore.ToString();
        _playerHealthText.text = GetComponent<PlayerHealth>().GetCharge().ToString(); ;

    }
}
