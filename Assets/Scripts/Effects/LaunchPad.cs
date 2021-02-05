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
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            //Get Near rigid bodies
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                var force = (transform.forward * (forcee / 5)) + (transform.up * forcee);
                rb.AddForce(force);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var force = transform.up * forcee;

            other.GetComponent<Rigidbody>().AddForce(force);
        }
    }
}
