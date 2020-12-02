using UnityEngine;

//Moves the object!
//Reference the IPooledObject
//Will run OnObjectSpawn every time instead of spawn
public class ObjectAccelerator : MonoBehaviour, IPooledObject
{
    public float upForce = 1f;
    public float sideForce = 0.1f;

    //Use the IPooledObject Start to ensure it runs every time (not just when its created)
    public void OnObjectSpawn()
    {
        //Particle Mover (Random Directionjs
        float xForce = Random.Range(-sideForce, sideForce);
        float yForce = Random.Range(upForce / 2f, upForce);
        float zForce = Random.Range(-sideForce, sideForce);

        Vector3 force = new Vector3(xForce, yForce, zForce);

        GetComponent<Rigidbody>().velocity = force;
    }
}
