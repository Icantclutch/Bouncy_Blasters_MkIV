using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOnCollide : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerSpawnSystem.SpawnPlayer(collision.gameObject);
        }
    }
}
