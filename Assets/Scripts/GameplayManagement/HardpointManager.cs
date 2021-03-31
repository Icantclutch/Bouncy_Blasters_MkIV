using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HardpointManager : NetworkBehaviour
{
    //The list of hardpoints for the map
    [SerializeField]
    private HardpointArea[] _ListOfHardpoints;

   [SerializeField]
    private List<int> _pointIndices;

    [SerializeField]
    private List<int> _runtimeIndices;

    //the active hardpoint index
    [SerializeField]
    private int _activeHardpointIndex = -1;

    [SerializeField]
    private int tempIndex;

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
        Invoke("Rpc_InitializeHardPoints", 0.5f);
    }

    [ClientRpc]
    public void Rpc_InitializeHardPoints()
    {
        Debug.Log("Rpc_InitializingHardpoints");
        _ListOfHardpoints = GameObject.FindGameObjectWithTag("Objective").GetComponentsInChildren<HardpointArea>();
        _pointIndices = new List<int>();
        for (int i = 0; i < _ListOfHardpoints.Length; i++)
        {
            _pointIndices.Add(i);
        }
        _runtimeIndices = new List<int>(_pointIndices);
        foreach (HardpointArea g in _ListOfHardpoints)
        {
            g.gameObject.SetActive(false);
        }

    }


    public void SelectNewHardpoint()
    {
        //First hardpoint of the cycle
        if(_activeHardpointIndex == -1)
        {
            _activeHardpointIndex = 0;
            _runtimeIndices.Remove(_activeHardpointIndex);
            Debug.Log("New Hardpoint: " + _activeHardpointIndex);
            Rpc_ActivateNewHardpoint(_activeHardpointIndex);
            return;
        }

        //Selecting a new point
        tempIndex = Random.Range(0, _runtimeIndices.Count);
        _activeHardpointIndex = _runtimeIndices[tempIndex];

        //Remove the old point from the list of potential options
        _runtimeIndices.Remove(_activeHardpointIndex);
           
        Rpc_ActivateNewHardpoint(_activeHardpointIndex);

        //Refreshing the list of available hardpoints
        if(_runtimeIndices.Count == 0)
        {
            _runtimeIndices = new List<int>(_pointIndices);
            _activeHardpointIndex = -1;
        }


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
