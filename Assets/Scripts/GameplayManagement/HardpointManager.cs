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
        Invoke("Rpc_InitializeHardPoints", 2f);
    }

    [ClientRpc]
    public void Rpc_InitializeHardPoints()
    {
        Debug.Log("Rpc_InitializingHardpoints");
        _ListOfHardpoints = GameObject.FindGameObjectWithTag("Objective").GetComponentsInChildren<HardpointArea>();
        foreach (HardpointArea g in _ListOfHardpoints)
        {
            g.gameObject.SetActive(false);
        }

    }


    public void SelectNewHardpoint()
    {
        //Create a list of the overcharge points
        List<int> indices = new List<int>();
        for(int i = 0; i < _ListOfHardpoints.Length; i++)
        {
            indices.Add(i);
        }
        //Remove the old point from the list of potential options
        indices.Remove(_activeHardpoint);

        //Selecting a new point
        int tempIndex = Random.Range(0, indices.Count - 1);
        _activeHardpoint = tempIndex;
        Rpc_ActivateNewHardpoint(_activeHardpoint);

        
        
      
       
    }

  
  

    [ClientRpc]
    private void Rpc_ActivateNewHardpoint(int index)
    {
        Debug.Log("Rpc_ActivateNewHardpoint");
        for (int i = 0; i < _ListOfHardpoints.Length; i++)
        {
            _ListOfHardpoints[i].gameObject.SetActive(false);
        }
        _ListOfHardpoints[index].gameObject.SetActive(true);
    }

}
