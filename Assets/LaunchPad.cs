using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    public float timer = 5;
    public float countdown;
    public float explosionRadius = 3f;
    public float forcee = 1000f;
    bool hasExploded = false;

    //[SerializeField] GameObject exploParticle;

    private void Start()
    {
        countdown = timer;
    }

    private void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0 && !hasExploded)
        {
            Launch();
            countdown = timer;
        }
    }

    void Launch()
    {
        //When Phillip is done with explosion particle
        //GameObject spawnedPart = Instantiate(exploParticle, transform.position, transform.rotation);
        // Destroy(spawnedPart, 1);
        print("JUMP PADDING");
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            //Get Near rigid bodies
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                //rb.AddExplosionForce(force, transform.position, explosionRadius);
                var force = (transform.forward * (forcee/5)) + (transform.up * forcee);
                rb.AddForce(force);
            }
        }
       // hasExploded = true;
        //Destroy(gameObject);
    }
}
