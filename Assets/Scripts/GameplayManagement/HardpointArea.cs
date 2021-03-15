using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HardpointArea : NetworkBehaviour
{
    //Variables for the number of players from each team attempting to capture the hardpoint area
    [SyncVar]
    [SerializeField]
    private int _numTeamAPlayers = 0;
    [SyncVar]
    [SerializeField]
    private int _numTeamBPlayers = 0;

    //The team controlling the hardpoint area
    [SyncVar]
    private int _controllingTeam = -1;

    public int ControllingTeam { get => _controllingTeam; }


    [SyncVar]
    [SerializeField]
    private List<PlayerData> _occupants;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        _occupants.Clear();
    }

    private void OnEnable()
    {
        InvokeRepeating("AssignPoints", 0, 1f);
    }

    private void AssignPoints()
    {
        foreach (PlayerData p in _occupants)
        {
            if(p.team == ControllingTeam)
            {
                p.AddPlayerScore(1);
            }
        }

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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            _occupants.Add(other.GetComponent<PlayerData>());

            if (other.gameObject.GetComponent<PlayerData>().team == 1)
            {
                _numTeamAPlayers++;
            }
            else if(other.gameObject.GetComponent<PlayerData>().team == 2)
            {
                _numTeamBPlayers++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
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
