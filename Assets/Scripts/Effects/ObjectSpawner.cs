using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    //Reference the ObjectPooler similar to a modular script (singleton in unity)
    ObjectPooler objectPooler;
    private void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }
    void FixedUpdate()
    {
        //Reference the SpawnFromPool function through singleton
        ObjectPooler.Instance.SpawnFromPool("Cube", transform.position, Quaternion.identity);
    }
}
