using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;
using System;

//Bullet that uses a constant raycast
//Constantly updates path
public class TravelBullet : RaycastBullet
{
    [System.Serializable]
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
    //Next direction the bullet should bounce
    protected Vector3 forcedDir = Vector3.zero;

    //For when the bullet needs to stop
    [SyncVar]
    bool stopBullet = false;

    //Info for the second bounce
    [SyncVar]
    bool floor = false;
    [SyncVar]
    Vector3 secondBounce = new Vector3();

    [Server]
    public override void Initialize(List<int> damage, int bounces, float fireSpeed, int playerId)
    {
        bulletInfos = new List<RayInfo>();
        base.Initialize(damage, bounces, fireSpeed, playerId);
        laserDestroyA = transform.position;
        nextDir = transform.forward;
        laserDestroyB = nextDir * 10000;
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
                //Retrieve the RayInfo
                RayInfo newHit = bulletInfos[myShot.numBounces];
                RaycastHit rayHit = newHit.rayHit;
                Ray rayPass = newHit.rayPass;

                Vector3 reflection = Vector3.Reflect(rayPass.direction, rayHit.normal);
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, rayHit.normal);
                Vector3 pos = rayHit.point;
                Instantiate(bulletCollisionEffect, pos, rot);

                if (newHit.rayHit.transform != null)
                {
                    if (newHit.rayHit.transform != NetworkIdentity.spawned[Convert.ToUInt32(myShot.playerID)].transform || myShot.numBounces > 0)
                    {
                        //If the hit is on a player and floor is active, reduce the bounce count
                        if (newHit.rayHit.transform.CompareTag("Player") && floor)
                        {
                            myShot.numBounces = 0;
                        }

                        //Check to see if it hit something
                        if (newHit.rayHit.transform.GetComponent<HitInteraction>())
                        {
                            //Send hit message
                            newHit.rayHit.transform.SendMessage("Hit", myShot, SendMessageOptions.DontRequireReceiver);

                            //If its an enemy, break
                            if (newHit.rayHit.transform.CompareTag("Player"))
                            {
                                base.DestroyBullet();
                            }
                        }
                    }
                }

                if (!newHit.rayHit.transform.CompareTag("Player"))
                {
                    if (bulletDirtEffect != null)
                    {
                        Instantiate(bulletDirtEffect, pos, Quaternion.FromToRotation(Vector3.up, reflection));
                    }
                }

                //Disable floor penalty if at the second bounce point
                if (transform.position == secondBounce)
                {
                    floor = false;
                }

                //Continue if there are still 2 positions
                if (nextDir != Vector3.zero && myShot.numBounces < myShot.maxBounces)
                {
                    if(forcedDir != Vector3.zero)
                    {
                        nextDir = forcedDir;
                        forcedDir = Vector3.zero;
                    }
                    //Reset lerping
                    laserDestroyA = laserDestroyB;
                    laserDestroyB = Vector3.zero;
                    destroyLerp = 0;
                    transform.forward = nextDir;
                    Vel(transform.forward, myShot.speed);

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

        float rayLength = 100;// - Vector3.Distance(laserDestroyA, transform.position);

        //Cast ray
        bool result = Physics.Raycast(ray, out hit, rayLength, reflectable);

        //Update bulletInfos
        if (bulletInfos.Count != myShot.numBounces + 1)
            bulletInfos.Add(new RayInfo(hit, ray));
        else
            bulletInfos[myShot.numBounces] = new RayInfo(hit, ray);

        //Check result
        if (result)
        {
            if (hit.transform.gameObject.tag == "OverchargeArea")
                return;
            if (hit.point == laserDestroyB)
                return;

            //Recalculate the lerp
            destroyLerp = Vector3.Distance(laserDestroyA, transform.position) / Vector3.Distance(laserDestroyA, laserDestroyB);
            destroyLerp = (destroyLerp * Vector3.Distance(laserDestroyA, laserDestroyB)) / Vector3.Distance(laserDestroyA, hit.point);

            //Update laserdetroyb
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
            nextDir = (hit.transform.CompareTag("NoBounce")) ? Vector3.zero : Vector3.Reflect(ray.direction, hit.normal);
        }
        else //If it didn't hit anything, end the ray
        {
            //If endpoint hasn't been set, set it to 100 away
            laserDestroyB = laserDestroyA + (ray.direction * rayLength);
            nextDir = Vector3.zero;
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
        GetComponentInChildren<TrailRenderer>().emitting = false;
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

    public void SetNextReflectionDirection(Vector3 nextReflection)
    {
        forcedDir = nextReflection;
    }
}
