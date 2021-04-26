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

    private void OnDisable()
    {
        _occupants.Clear();
    }

    private void OnEnable()
    {
        
    }

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
