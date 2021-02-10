using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
using System;

public class TravelBullet : RaycastBullet
{
    public class RayInfo
    {
        public RaycastHit rayHit;
        public Ray rayPass;
        public RayInfo(RaycastHit rayHit1, Ray rayPass1)
        {
            rayPass = rayPass1;
            rayHit = rayHit1;
        }
    }
    [SerializeField]
    protected List<RayInfo> bulletInfos = new List<RayInfo>();

    //Next direction the bullet should bounce
    protected Vector3 nextDir;

    //For when the bullet needs to stop
    bool stopBullet = false;

    //Info for the second bounce
    bool floor = false;
    Vector3 secondBounce = new Vector3();

    [Server]
    public override void Initialize(List<int> damage, int bounces, float fireSpeed, int playerId)
    {
        base.Initialize(damage, bounces, fireSpeed, playerId);
        laserDestroyA = transform.position;
        nextDir = transform.forward;
        Vel(transform.forward, myShot.speed);
    }

    [Server]
    public override void Update()
    {
        if (!stopBullet)
        {
            //Re-check the velocity
            Vel(transform.forward, myShot.speed);

            //Increase the lerp,
            destroyLerp += (Time.deltaTime * bulletSpeed * 100) / (Vector3.Distance(laserDestroyA, laserDestroyB));
            if (destroyLerp > 1)
                destroyLerp = 1;
            
            //Set the position to the relative position
            transform.position = Vector3.Lerp(laserDestroyA, laserDestroyB, destroyLerp);

            if (transform.position == laserDestroyB)
            {
                //Calls from bulletInfo that stores a class RayInfo
                //RayInfo holds the Ray and the Rayhit from the collision
                if (bulletInfos.Count >= 1)
                {
                    //Retrieve the RayInfo
                    RayInfo newHit = bulletInfos[0];
                    RaycastHit rayHit = newHit.rayHit;
                    Ray rayPass = newHit.rayPass;
                    bulletInfos.RemoveAt(0);

                    Vector3 reflection = Vector3.Reflect(rayPass.direction, rayHit.normal);
                    Quaternion rot = Quaternion.FromToRotation(Vector3.up, rayHit.normal);
                    Vector3 pos = rayHit.point;
                    Instantiate(bulletCollisionEffect, pos, rot);
                    Instantiate(bulletDirtEffect, pos, Quaternion.FromToRotation(Vector3.up, reflection));
                }

                //Disable floor penalty if at the second bounce point
                if (transform.position == secondBounce)
                {
                    floor = false;
                }

                //Continue if there are still 2 positions
                if (nextDir != Vector3.positiveInfinity)
                {
                    //Reset lerping
                    laserDestroyA = laserDestroyB;
                    laserDestroyB = Vector3.positiveInfinity;
                    destroyLerp = 0;
                    transform.forward = nextDir;

                    //Up the number of bounces
                    myShot.numBounces += 1;
                }
                else
                {
                    //Destroy the bullet
                    DestroyBullet();
                }
            }
        }
    }

    //Velocity needed to be overhauled along with update, in essence its now designed to constantly update
    [Server]
    public override void Vel(Vector3 vel, float speed)
    {
        bulletSpeed = speed;

        //Create ray that transfers through loops and raycasthit
        Ray ray = new Ray(transform.position, vel);
        RaycastHit hit;

        //Cast ray
        if (Physics.Raycast(ray, out hit, 100, reflectable) && hit.point != laserDestroyB)
        {

            destroyLerp = (destroyLerp * Vector3.Distance(laserDestroyA, laserDestroyB)) / Vector3.Distance(laserDestroyA, hit.point);

            //Add hit point to the list of line points
            laserDestroyB = hit.point;
            
            //Set second bounce if this is the second bounce
            if (myShot.numBounces == 1)
                secondBounce = hit.point;

            //If its first bounce is off the floor, set floor true
            if (hit.transform.CompareTag("Floor") && myShot.numBounces == 0)
            {
                floor = true;
            }

            //If it hits a NoBounce object, end the bouncing; otherwise, generate the reflection
            nextDir = (hit.transform.CompareTag("NoBounce")) ? Vector3.positiveInfinity : Vector3.Reflect(ray.direction, hit.normal);
        }
        else //If it didn't hit anything, end the ray
        {
            //Set the endpoint and set the nextdir to positiveinfinity
            laserDestroyB = ray.origin + (ray.direction * 100);
            nextDir = Vector3.positiveInfinity;

            //Add effects (shouldn't need since there is no collision)
            //Rayhits.Add(new RayInfo(hit, ray));
        }
    }

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform != NetworkIdentity.spawned[Convert.ToUInt32(myShot.playerID)].transform || myShot.numBounces > 1)
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
        stopBullet = true;
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
