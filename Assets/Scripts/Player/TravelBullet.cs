using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
using System;

public class TravelBullet : RaycastBullet {

    [SerializeField]
    protected List<Vector3> raycastPositions = new List<Vector3>();

    //Info for the second bounce
    bool floor = false;
    Vector3 secondBounce = new Vector3();

    [Server]
    public override void Initialize(List<int> damage, int bounces, float fireSpeed, int playerId)
    {
        base.Initialize(damage, bounces, fireSpeed, playerId);
    }

    [Server]
    public override void Update()
    {
        if (raycastPositions.Count >= 2)
        {
            //Increase the lerp,
            destroyLerp += (Time.deltaTime * bulletSpeed * 100) / (Vector3.Distance(laserDestroyA, laserDestroyB));
            if (destroyLerp > 1)
                destroyLerp = 1;
            //Set the position of the first point
            transform.position = Vector3.Lerp(laserDestroyA, laserDestroyB, destroyLerp);

            if (transform.position == laserDestroyB)
            {
                ///PUT THE COLLISION STUFF HERE MARK

                ///ABOVE HERE

                //Remove the first point
                raycastPositions.RemoveAt(0);

                //Disable floor penalty if at the second bounce point
                if(transform.position == secondBounce)
                {
                    floor = false;
                }

                //Continue if there are still 2 positions
                if (raycastPositions.Count >= 2)
                {
                    //Reset lerping
                    laserDestroyA = raycastPositions[0];
                    laserDestroyB = raycastPositions[1];
                    destroyLerp = 0;
                }
            }
        }
        else
        {
            //Destroy the bullet
            DestroyBullet();
        }
    }

    [Server]
    public override void Vel(Vector3 vel, float speed)
    {
        bulletSpeed = speed;
        //Create the array of vec3 points in the line and add the starting point
        List<Vector3> bouncePoints = new List<Vector3>();
        bouncePoints.Add(transform.position);

        //Create ray that transfers through loops and raycasthit
        Ray ray = new Ray(transform.position, vel);
        RaycastHit hit;

        //Loop through reflections
        for (int i = 0; i <= myShot.maxBounces; i++)
        {
            //Cast ray
            if (Physics.Raycast(ray, out hit, 100, reflectable))
            {
                //Add hit point to the list of line points
                bouncePoints.Add(hit.point);

                //Set second bounce if this is the second bounce
                if (i == 1)
                    secondBounce = hit.point;

                //If its first bounce is off the floor, set floor true
                if (hit.transform.CompareTag("Floor") && i == 0)
                {
                    floor = true;
                }

                //Generate the reflection
                Vector3 reflection = Vector3.Reflect(ray.direction, hit.normal);
                ray = new Ray(hit.point, reflection);
            }
            else //If it didn't hit anything, end the ray
            {
                //Create the endpoint and add it to the lists
                Vector3 endPoint = ray.origin + (ray.direction * 100);
                bouncePoints.Add(endPoint);
                //End loop early
                break;
            }
        }

        //Assign points
        raycastPositions = bouncePoints;

        //Set up movement
        laserDestroyA = raycastPositions[0];
        laserDestroyB = raycastPositions[1];
        destroyLerp = 0;
    }

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform != NetworkIdentity.spawned[Convert.ToUInt32(myShot.playerID)].transform)
        {
            //If the hit is on a player and floor is active, reduce the bounce count
            if (other.transform.CompareTag("Player") && floor)
            {
                myShot.numBounces = 0;
            }

            //Check to see if it hit something
            if (other.transform.GetComponent<HitInteraction>())
            {
                //Send hit message
                other.transform.SendMessage("Hit", myShot, SendMessageOptions.DontRequireReceiver);


                //If its an enemy, break
                if (other.transform.CompareTag("Player"))
                {
                    DestroyBullet();
                }
            }
        }
    }

    [Server]
    public override void DestroyBullet()
    {
        StartCoroutine(SlowBulletDeath());
    }

    IEnumerator SlowBulletDeath()
    {
        GetComponentInChildren<ParticleSystem>().Stop();
        Rpc_DisableParticles();

        while (GetComponentInChildren<ParticleSystem>().IsAlive())
        {
            yield return null;
        }

        base.DestroyBullet();
    }

    [ClientRpc]
    protected void Rpc_DisableParticles()
    {
        GetComponentInChildren<ParticleSystem>().Stop();
    }
}
