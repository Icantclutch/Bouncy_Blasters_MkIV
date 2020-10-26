using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    private static List<PlayerSpawnPoint> spawnPoints = new List<PlayerSpawnPoint>();
    

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
            List<PlayerSpawnPoint> points = new List<PlayerSpawnPoint>();
            PlayerSpawnPoint spawnPoint = null;
            foreach (PlayerSpawnPoint point in spawnPoints) {
                if (respawn != point.isRespawnRoom && initialSpawn == point.isStartingPoint)
                {
                    int playerTeam = player.GetComponent<PlayerHealth>().GetTeam();
                    //int playerTeam = 0;
                    if (initialSpawn && playerTeam == point.team)
                    {
                        points.Add(point);
                    }
                    else if(!initialSpawn && (playerTeam == point.team || point.team == 0))
                    {
                        points.Add(point);
                    }
                    
                }

            }
            if(points.Count > 0)
            {
                spawnPoint = points[Random.Range(0, points.Count)];
            }
            /*
            foreach(PlayerSpawnPoint point in points)
            {
                if (!spawnPoint)
                {
                    spawnPoint = point;
                }
            }*/

            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
                player.transform.rotation = spawnPoint.transform.rotation;
                player.transform.Find("Eyes").rotation = spawnPoint.transform.rotation;
            }
        }
    }
}
