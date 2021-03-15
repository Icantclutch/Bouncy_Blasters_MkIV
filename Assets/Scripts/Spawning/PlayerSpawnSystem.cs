using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    //List of all of the spawn points in the scene
    private static List<PlayerSpawnPoint> spawnPoints = new List<PlayerSpawnPoint>();
    //List of all players using the spawn system (Used to determine spawn location)
    private static List<GameObject> players = new List<GameObject>();

    //Add a spawnpoint to the static list
    public static void AddSpawnPoint(PlayerSpawnPoint spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
    }
    //Remove a spawnpoint to the static list
    public static void RemoveSpawnPoint(PlayerSpawnPoint spawnPoint)
    {
        spawnPoints.Remove(spawnPoint);
    }

    //Add a player's gameobject to the static list
    public static void AddPlayer(GameObject player)
    {
        players.Add(player);
    }
    //Remove a player's gameobject to the static list
    public static void RemovePlayer(GameObject player)
    {
        players.Remove(player);
    }

    //Sets the player's position to a chosen spawn point
    public static bool SpawnPlayer(GameObject player, bool respawn = true, bool initialSpawn = false)
    {
        if(spawnPoints.Count > 0)
        {
            //List of acceptable spawnpoints based on boolean parameters and player's team
            List<PlayerSpawnPoint> points = new List<PlayerSpawnPoint>();
            //Spawn point that will be used
            PlayerSpawnPoint spawnPoint = null;
            int playerTeam = player.GetComponent<PlayerData>().team;

            //Loop to find acceptable spawn points
            foreach (PlayerSpawnPoint point in spawnPoints) {
                //Make sure the parameter booleans are the same with the current spawn point
                if (respawn != point.isRespawnRoom && initialSpawn == point.isStartingPoint)
                {
                    

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
            //Will be used if the algorithm doesnt find a suitable spawnpoint
            if(points.Count > 0)
            {
                spawnPoint = points[Random.Range(0, points.Count)];
            }


            //Go though list of acceptable spawn points and choose one
            //Chooses spawn point furthest away from other players
            //To-do: based on teams of players?
            if (players.Count > 1)
            {
                float distanceAway = 0;
                foreach (PlayerSpawnPoint point in points)
                {
                    //Loop to find the other player closest to the curren spawnpoint
                    float closestPlayerDistance = float.MaxValue;
                    foreach (GameObject p in players)
                    {
                        //Do not check it if it is the respawning player
                        if (p != player /*&& (p.GetComponent<PlayerData>().team == 0 || p.GetComponent<PlayerData>().team != playerTeam)*/)
                        {
                            //Calculate distance between player and spawnpoint
                            float distance = (p.transform.position - point.transform.position).magnitude;

                            if (playerTeam != 0 && p.GetComponent<PlayerData>() && p.GetComponent<PlayerData>().team == playerTeam)
                            {
                                if (distance < 3)
                                {
                                    closestPlayerDistance = -1;
                                }
                            }
                            else if (distance < closestPlayerDistance)
                            {
                                closestPlayerDistance = distance;
                            }
                        }
                    }
                    //use current spawn point if it is further away from other players then previously checked points
                    if (closestPlayerDistance > distanceAway && closestPlayerDistance != float.MaxValue)
                    {
                        distanceAway = closestPlayerDistance;
                        spawnPoint = point;
                    }
                }
            }

            //Only set the players transform if a spawn point was found
            //Todo: Rotation is not being set, might be something with the mouseLook script
            if (spawnPoint != null)
            {
                Debug.Log(spawnPoint);
                player.transform.position = spawnPoint.transform.position;
                player.transform.rotation = spawnPoint.transform.rotation;
                player.GetComponent<MouseLook2>().SetCharacterRotation(spawnPoint.transform.rotation);
                //player.transform.Find("Eyes").rotation = spawnPoint.transform.rotation;
                return true;
            }
        }
        return false;
    }
}
