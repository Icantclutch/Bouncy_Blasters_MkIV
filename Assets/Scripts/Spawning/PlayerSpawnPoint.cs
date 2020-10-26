using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField]
    public int team = 0;
    [SerializeField]
    public bool isRespawnRoom = false;
    [SerializeField]
    public bool isStartingPoint = false;

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

    private void OnDrawGizmos()
    {
        if(team < teamColors.Count)
            Gizmos.color = teamColors[team];
        else
            Gizmos.color = Color.grey;
        Gizmos.DrawSphere(transform.position, 1f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward*2);
    }
}
