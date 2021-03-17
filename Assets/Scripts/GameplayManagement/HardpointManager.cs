using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardpointManager : MonoBehaviour
{
    //The list of hardpoints for the map
    [SerializeField]
    private HardpointArea[] _ListOfHardpoints;

    //the active hardpoint index
    private int _activeHardpoint;

    // Start is called before the first frame update
    void Start()
    {
        //_ListOfHardpoints = GetComponentsInChildren<HardpointArea>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectNewHardpoint()
    {
        int listSize = _ListOfHardpoints.Length;
        _activeHardpoint = Random.Range(0, listSize - 1);

        ActivateNewHardpoint();
    }

    private void ActivateNewHardpoint()
    {
        for(int i = 0; i < _ListOfHardpoints.Length; i++)
        {
            _ListOfHardpoints[i].enabled = false;
        }
        _ListOfHardpoints[_activeHardpoint].enabled = true;
    }

}
