using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawning : MonoBehaviour
{
    //The wall being spawned
    public GameObject spawnable;
    //The parent gameobject that all the spawned objects will be children of
    public Transform spawnableParent;
    //The list of transforms that act as placeholders for where walls can appear
    private Transform[] objects;
    //The number of walls that will pop up a cycle
    public int numOfWallsSpawned = 7;
    //The Y value increase that a wall may need to sit on the ground properly
    public float yBoost = 0f;
    // Start is called before the first frame update
    void Start()
    {
        //Getting the placeholder transforms for all the wall spots
        objects = GetComponentsInChildren<Transform>();
        
        //Spawn the walls and continue the cycle
        InvokeRepeating("SpawnWalls", 5f, 8f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Function that picks random wall locations and spawns the walls
    private void SpawnWalls()
    {
        //A list of integers that stores the random wall slots selected
        List<int> r = new List<int>();
        //Looping through until the number of wall slots have been picked
        for(int i = 0; i < numOfWallsSpawned; i++)
        {
            //Picking a random slot then checking to make sure there are no dupplicates
            int q = (int)Random.Range(1, 20);
            if (!r.Contains(q))
            {
                r.Add(q);
            }
            else
            {
                i--;
            }
        }
        //Spawing walls in each of the selected slots then destroying the walls after 7 seconds
        for(int i = 0; i < numOfWallsSpawned; i++)
        {
            
            Transform spawnSlot = objects[r[i]];
            GameObject wall = Instantiate(spawnable, new Vector3(spawnSlot.position.x, spawnSlot.position.y + yBoost, spawnSlot.position.z), spawnSlot.rotation, spawnableParent);
           
            Destroy(wall, 7f);
        }
       
    }
}
