using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerInfoDisplay : NetworkBehaviour
{
    [SerializeField]
    private Text _nameDisplayText;
    [SerializeField]
    private SpriteRenderer _miniMapDisplay;
    [SerializeField]
    private GameObject _infoDisplay;

    private MyNetworkManager _networkManager;

    private static int localPlayerTeam;

    public static void SetLocalPlayerTeam(int team = 0)
    {
        localPlayerTeam = team;
    }
    // Start is called before the first frame update
    void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<MyNetworkManager>();
        if (isLocalPlayer)
        {
            //_infoDisplay.SetActive(false);
            //this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _nameDisplayText.text = GetComponent<PlayerData>().playerName;
        int team = GetComponent<PlayerData>().team;
        //GameObject localPlayer = _networkManager.GetLocalPlayer();
        //int localPlayer = CmdLocalPlayerTeam();
        if (isLocalPlayer)
        {
            if (_miniMapDisplay)
                _miniMapDisplay.color = Color.yellow;
        }
        else if (team == 0 || (/*localPlayer &&*/ team != localPlayerTeam))
        {
            _nameDisplayText.color = Color.red;
            if (_miniMapDisplay)
                _miniMapDisplay.color = Color.red;
        }
        else
        {
            _nameDisplayText.color = Color.blue;
            if(_miniMapDisplay)
                _miniMapDisplay.color = Color.blue;
        }
        if(!isLocalPlayer && Camera.main)
            _infoDisplay.transform.LookAt(Camera.main.transform);
    }

    //[Command]
    //public int CmdLocalPlayerTeam()
    //{
    //    return _networkManager.GetLocalPlayerTeam();
    //}
}
