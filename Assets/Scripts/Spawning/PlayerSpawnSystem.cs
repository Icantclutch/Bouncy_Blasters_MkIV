using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    //List of all of the spawn points in the scene
    private static List<PlayerSpawnPoint> spawnPoints = new List<PlayerSpawnPoint>();

    public static void AddSpawnPoint(PlayerSpawnPoint spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
    }

    public static void RemoveSpawnPoint(PlayerSpawnPoint spawnPoint)
    {
        spawnPoints.Remove(spawnPoint);
    }

    //Sets the player's position to a chosen spawn point
    public static void SpawnPlayer(GameObject player, bool respawn = true, bool initialSpawn = false)
    {
        if(spawnPoints.Count > 0)
        {
            //List of acceptable spawnpoints based on boolean parameters and player's team
            List<PlayerSpawnPoint> points = new List<PlayerSpawnPoint>();
            //Spawn point that will be used
            PlayerSpawnPoint spawnPoint = null;

            //Loop to find acceptable spawn points
            foreach (PlayerSpawnPoint point in spawnPoints) {
                //Make sure the parameter booleans are the same with the current spawn point
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

            //Choose a random acceptable spawn point
            //Temporary, until below loop is made
            if(points.Count > 0)
            {
                spawnPoint = points[Random.Range(0, points.Count)];
            }

            /*
            //Go though list of acceptable spawn points and choose one
            //To-do: choose spawn point furthest away from enemy players
            foreach(PlayerSpawnPoint point in points)
            {
                if (!spawnPoint)
                {
                    spawnPoint = point;
                }
            }*/

            //Only set the players transform if a spawn point was found
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
                player.transform.rotation = spawnPoint.transform.rotation;
                //player.transform.Find("Eyes").rotation = spawnPoint.transform.rotation;
            }
        }
    }
}
