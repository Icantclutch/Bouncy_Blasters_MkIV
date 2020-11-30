using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HardpointArea : NetworkBehaviour
{
    //Variables for the number of players from each team attempting to capture the hardpoint area
    [SyncVar]
    private int _numTeamAPlayers = 0;
    [SyncVar]
    private int _numTeamBPlayers = 0;

    //The team controlling the hardpoint area
    [SyncVar]
    private int _controllingTeam = -1;

    public int ControllingTeam { get => _controllingTeam; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void CompareAreaController()
    {
        if (_numTeamAPlayers == 0 && _numTeamBPlayers == 0)
        {
            _controllingTeam = -1;
        }
        else if(_numTeamAPlayers > _numTeamBPlayers)
        {
            _controllingTeam = 1;
        }
        else if (_numTeamBPlayers > _numTeamAPlayers)
        {
            _controllingTeam = 2;
        }
    }
}
