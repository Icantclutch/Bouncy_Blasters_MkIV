using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{

    [SerializeField]
    private Text _batteryCount;

    [SerializeField]
    private Text _reserveBatteryCount;

    [SerializeField]
    private Text _teamAScore;

    [SerializeField]
    private Text _teamBScore;

    [SerializeField]
    private Text _playerHealth;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _batteryCount.text = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentAmmo.ToString();
        _reserveBatteryCount.text = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentReserve.ToString();
        _teamAScore.text = "" + 0;
        _teamBScore.text = "" + 0;
        _playerHealth.text = GetComponent<PlayerHealth>().GetCharge().ToString();




    }
}
