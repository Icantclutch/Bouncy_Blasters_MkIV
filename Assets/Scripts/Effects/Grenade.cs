using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float timer = 5;
    public float countdown;
    public float explosionRadius = 3f;
    public float force = 1000f;
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
            Explode();
        }
    }

    void Explode()
    {
        //When Phillip is done with explosion particle
        //GameObject spawnedPart = Instantiate(exploParticle, transform.position, transform.rotation);
        // Destroy(spawnedPart, 1);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            //Get Near rigid bodies
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, explosionRadius);
            }
        }
        hasExploded = true;
        Destroy(gameObject);
    }
}
