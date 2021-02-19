using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    public float timer;
    public float countdown;
    public float explosionRadius;
    public float upForce;
    public float sideForce;

    public bool launchedPrimed = true;
    GameObject target;

    //[SerializeField] GameObject exploParticle;

    private void Start()
    {
        countdown = timer;
    }

    private void Update()
    {
        //Timer if the launch pad isn't charged
        if (!launchedPrimed)
        {
            countdown -= Time.deltaTime;
            //Reset the launch state once the timer is reset
            if(countdown <= 0)
            {
                launchedPrimed = true;
                countdown = timer;

            }
        }
       
    }

    void Launch()
    {
        Rigidbody targetRB = target.GetComponent<Rigidbody>();
        targetRB.velocity = new Vector3(targetRB.velocity.x, 0f, targetRB.velocity.z);

        targetRB.GetComponent<PlayerMovement>().Launch();
        targetRB.AddForce(target.transform.forward.x * sideForce, upForce, target.transform.forward.z * sideForce);
        //Debug.Log("Launch Force: " + target.transform.forward.x * sideForce + ", " + upForce + ", " + target.transform.forward.z * sideForce);


        ////Old Code
        /*
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
        }*/
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (launchedPrimed)
            {
                target = other.gameObject;
                launchedPrimed = false;
                Launch();
            }
            
        }
    }
}
