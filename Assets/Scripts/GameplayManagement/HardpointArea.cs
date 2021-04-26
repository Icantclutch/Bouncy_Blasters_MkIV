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

    [SerializeField]
    private float hardpointTimer = 1.0f;

    //The team controlling the hardpoint area
    [SyncVar]
    private int _controllingTeam = -1;

    public int ControllingTeam { get => _controllingTeam; }

    //PlayerData cant be a SyncVar or a SerializeField
    private List<PlayerData> _occupants;

    // Start is called before the first frame update
    void Start()
    {
        _occupants = new List<PlayerData>();
    }

    // Update is called once per frame
    void Update()
    {
        hardpointTimer -= Time.deltaTime;
        CompareAreaController();
        if(hardpointTimer < 0)
        {
            AssignPoints();
            hardpointTimer = 1.0f;
        }
    }

    //Clear the occupants from the overcharge point as it will be disabled when the point "moves"
    private void OnDisable()
    {
        _occupants.Clear();
    }

    private void OnEnable()
    {
        
    }
    //Assigns points to all players in the overcharge point if they are a part of the controlling team
    private void AssignPoints()
    {
        if (_occupants != null)
        {
            
            foreach (PlayerData p in _occupants)
            {
                Debug.Log("player datas: " + p.gameObject.name);
                if (p.team == ControllingTeam)
                {
                    p.AddPlayerScore(1);
                }
            }
        }
    }

    /*Checks to see if the overcharge point is controlled by team 1, 2, or if it is contested/empty as -1.
     * Team 0 is not used since that used to be the team for Free For All*/
    private void CompareAreaController()
    {
        if ((_numTeamAPlayers == 0 && _numTeamBPlayers == 0) || _numTeamAPlayers == _numTeamBPlayers )
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger == false)
        {
            if (other.gameObject.tag == "Player")
            {
                _occupants.Add(other.GetComponent<PlayerData>());

                if (other.gameObject.GetComponent<PlayerData>().team == 1)
                {
                    _numTeamAPlayers++;
                }
                else if (other.gameObject.GetComponent<PlayerData>().team == 2)
                {
                    _numTeamBPlayers++;
                }
            }
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.isTrigger == false)
        {
            if (other.gameObject.tag == "Player")
            {
                _occupants.Remove(other.GetComponent<PlayerData>());

                if (other.gameObject.GetComponent<PlayerData>().team == 1)
                {
                    _numTeamAPlayers--;
                }
                else if (other.gameObject.GetComponent<PlayerData>().team == 2)
                {
                    _numTeamBPlayers--;
                }
            }
        }
        
    }
}
