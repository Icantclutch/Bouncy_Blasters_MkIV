using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawning : MonoBehaviour
{
    public GameObject spawnable;
    public Transform spawnableParent;
    private Transform[] objects;
    public int numOfWallsSpawned = 7;
    public float yBoost = 0f;
    // Start is called before the first frame update
    void Start()
    {
        objects = GetComponentsInChildren<Transform>();
        
        InvokeRepeating("SpawnWalls", 5f, 8f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnWalls()
    {
        List<int> r = new List<int>();
        for(int i = 0; i < numOfWallsSpawned; i++)
        {
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
        
        for(int i = 0; i < numOfWallsSpawned; i++)
        {
            
            Transform spawnSlot = objects[r[i]];
            GameObject wall = Instantiate(spawnable, new Vector3(spawnSlot.position.x, spawnSlot.position.y + yBoost, spawnSlot.position.z), spawnSlot.rotation, spawnableParent);
           
            Destroy(wall, 7f);
        }
       
    }
}
