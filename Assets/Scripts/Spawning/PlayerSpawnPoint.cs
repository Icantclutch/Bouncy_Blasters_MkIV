using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    //Variables used to determine who and when someone uses the spawnpoint
    public int team = 0;
    public bool isRespawnRoom = false;
    public bool isStartingPoint = false;

    //List of colors to be used with Gizmo display based on the set team
    [SerializeField]
    private List<Color> teamColors = new List<Color>()
    {
        Color.grey,
        Color.blue,
        Color.red,
        Color.green,
        Color.magenta
    };

    private void Awake()
    {
        PlayerSpawnSystem.AddSpawnPoint(GetComponent<PlayerSpawnPoint>());
    }
    private void OnDestroy()
    {
        PlayerSpawnSystem.RemoveSpawnPoint(GetComponent<PlayerSpawnPoint>());
    }

    //Draw a sphere to show where the spawn points are in the Unity editor
    private void OnDrawGizmos()
    {
        //Draws a sphere with the color of the team the spawnpoint is set to
        if(team < teamColors.Count)
            Gizmos.color = teamColors[team];
        else
            Gizmos.color = Color.grey;
        Gizmos.DrawSphere(transform.position, 1f);
        
        //Draws a line in the forward direction
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward*2);
    }
}
