using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnSystem : MonoBehaviour
{
    private static List<PlayerSpawnPoint> spawnPoints;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void AddSpawnPoint(PlayerSpawnPoint spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
    }

    public static void RemoveSpawnPoint(PlayerSpawnPoint spawnPoint)
    {
        spawnPoints.Remove(spawnPoint);
    }

    public static void SpawnPlayer(GameObject player, bool respawn = true, bool initialSpawn = false)
    {
        if(spawnPoints.Count > 0)
        {
            PlayerSpawnPoint spawnPoint = spawnPoints[0];
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;
        }
    }
}
