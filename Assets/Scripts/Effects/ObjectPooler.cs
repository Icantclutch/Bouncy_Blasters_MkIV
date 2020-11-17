using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    //Show up in the inspector
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    //Creating a singleton to be used in spawners with ease
    #region Singleton
    public static ObjectPooler Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public List<Pool> pools;

    //Create a pool dictionary to be called
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    //Create a poolDictionary with all the pools
    //Each pool can be used to create objects
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        //Loop through the possible pools created
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool (string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return null;
        }
        //Return the queue's first object
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        //"Creating" the object
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        //Reference the Interface used by the object
        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();

        if (pooledObj != null)
        {
            //Calls the function OnObjectSpawn to ensure a force is applied every time
            pooledObj.OnObjectSpawn();
        }

        //Add back to end of queue to be used again
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
