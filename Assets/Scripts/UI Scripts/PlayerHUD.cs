using DigitalRuby.Tween;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    

    [SerializeField]
    private Text _batteryCountText = null;

    //[SerializeField]
    //private Text _reserveBatteryCountText = null;

    [SerializeField]
    private Text _teamAScoreText = null;
    
    [SerializeField]
    private Text _teamBScoreText = null;
  
    [SerializeField]
    private Text _playerHealthText = null;

    [SerializeField]
    private Image _playerHealthBar = null;

    [SerializeField]
    private Text _playerWeaponText = null;

    [SerializeField]
    private Text _matchTimer = null;
    [SerializeField]
    private Text _preMatchTimer = null;
    [SerializeField]
    private NetworkManager _networkManager = null;
   
    [SerializeField]
    private Animator _anim = null;
    [SerializeField]
    private Animator _mapAnim = null;

    private GameObject _gameManager = null;
    public GameObject _miniMap = null;

    [SerializeField]
    private Text _matchEndText = null;

    
    [SerializeField]
    public Transform[] scoreboardTeamLocations;

    // Start is called before the first frame update
    void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        _gameManager = GameObject.FindGameObjectWithTag("Management");
    }

   // string minutes = $$anonymous$$athf.Floor(timer / 60).ToString("00");
   // string seconds = (timer % 60).ToString("00");

   // print(string.Format("{0}:{1}", minutes, seconds));
    // Update is called once per frame
    void Update()
    {
        if(_networkManager == null)
        {
            _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        }
        else
        {
            _batteryCountText.text = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentAmmo.ToString() + "/"
                + GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentReserve.ToString();
            //_reserveBatteryCountText.text = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].currentReserve.ToString();
            SetHealthDisplay(GetComponent<PlayerHealth>().GetCharge());
            _playerWeaponText.text = GetComponent<Shooting>().playerWeapons[GetComponent<Shooting>().currentWeapon].weapon.name;

            _teamAScoreText.text = _gameManager.GetComponentInChildren<GameManagement>().teamAScore.ToString();
            _teamBScoreText.text = _gameManager.GetComponentInChildren<GameManagement>().teamBScore.ToString();
            _matchTimer.text = FormatTime(_gameManager.GetComponentInChildren<GameManagement>().MatchTimer);//_gameManager.GetComponentInChildren<GameManagement>().MatchTimer.ToString();
            float preTimer = _gameManager.GetComponentInChildren<GameManagement>().PreMatchTimer;
            if (preTimer > 0)
            {
                _preMatchTimer.text = "Match Begins In\n" + preTimer;
            }
            else
            {
                _preMatchTimer.text = "";
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _anim.SetTrigger("Zoom");
            _mapAnim.SetTrigger("Start");
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            _anim.SetTrigger("Out");
            _mapAnim.SetTrigger("End");
        }

      

    }

    public string FormatTime(float Timer)
    {
        string minutes = Mathf.Floor(Timer / 60).ToString("00");
        string seconds = (Timer % 60).ToString("00");
        return string.Format("{0}:{1}", minutes, seconds);
    }

    public void SetHealthDisplay(int Charge)
    {
        //Set the bar color and display here
        float BarDisplayVal = ((float)Charge) / ((float)100);
        float Max = Mathf.Max(0, 0.75f - BarDisplayVal);
        //Color newColor = new Color(BarDisplayVal, 0, Max, 1);
        //_playerHealthBar.color = newColor;
        _playerHealthBar.fillAmount = BarDisplayVal;
        _playerHealthText.text = Charge.ToString();
    }

    public void DeclareWinState(string state)
    {
       _matchEndText.text = state;
    }

    public void LeaveMatch()
    {
        if (_gameManager)
        {
            _gameManager.GetComponent<LobbyManager>().LeaveLobby();
        }
    }
}
