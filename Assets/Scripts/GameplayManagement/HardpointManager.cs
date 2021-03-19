using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HardpointManager : NetworkBehaviour
{
    //The list of hardpoints for the map
    [SerializeField]
    private HardpointArea[] _ListOfHardpoints;

    //the active hardpoint index
    private int _activeHardpoint;

    // Start is called before the first frame update
    void Start()
    {
       
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void InitializeHardPoints()
    {
        
        _ListOfHardpoints = GameObject.FindGameObjectWithTag("Objective").GetComponentsInChildren<HardpointArea>();
        foreach(HardpointArea g in _ListOfHardpoints)
        {
            g.gameObject.SetActive(false);
        }
       
    }


    public void SelectNewHardpoint()
    {
        Debug.Log("Selecting new Hardpoint");
        int listSize = _ListOfHardpoints.Length;
        _activeHardpoint = Random.Range(0, listSize - 1);

        ActivateNewHardpoint();
    }

  
    private void ActivateNewHardpoint()
    {
        for(int i = 0; i < _ListOfHardpoints.Length; i++)
        {
            _ListOfHardpoints[i].gameObject.SetActive(false);
        }
        Debug.Log("Enabled: " + _activeHardpoint);
        _ListOfHardpoints[_activeHardpoint].gameObject.SetActive(true);
        Rpc_ActivateNewHardpoint(_activeHardpoint);
       
    }

    [ClientRpc]
    private void Rpc_ActivateNewHardpoint(int index)
    {
        for (int i = 0; i < _ListOfHardpoints.Length; i++)
        {
            _ListOfHardpoints[i].gameObject.SetActive(false);
        }
        _ListOfHardpoints[index].gameObject.SetActive(true);
    }

}
