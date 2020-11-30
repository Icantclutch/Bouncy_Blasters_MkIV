using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerInfoDisplay : MonoBehaviour
{
    [SerializeField]
    private Text _nameDisplayText;

    private MyNetworkManager _networkManager;

    private static int localPlayerTeam;

    public static void SetLocalPlayerTeam(int team = 0)
    {
        localPlayerTeam = team;
    }
    // Start is called before the first frame update
    void Start()
    {
        _networkManager = GameObject.FindGameObjectWithTag("Management").GetComponent<MyNetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        _nameDisplayText.text = GetComponent<PlayerData>().playerName;
        int team = GetComponent<PlayerData>().team;
        GameObject localPlayer = _networkManager.GetLocalPlayer();
        //int localPlayer = CmdLocalPlayerTeam();
        if (team == 0 || (/*localPlayer &&*/ team != localPlayerTeam))
        {
            _nameDisplayText.color = Color.red;
        }
        else
        {
            _nameDisplayText.color = Color.white;
        }

    }

    //[Command]
    //public int CmdLocalPlayerTeam()
    //{
    //    return _networkManager.GetLocalPlayerTeam();
    //}
}
