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
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward*2);
    }
}
