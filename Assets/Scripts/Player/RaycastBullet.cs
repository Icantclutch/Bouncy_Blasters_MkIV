﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class RaycastBullet : Bullet
{
    //Line renderer
    protected LineRenderer lineRenderer;

    //Speed
    [SyncVar]
    public float bulletSpeed;
    public GameObject bulletCollisionEffect;
    public GameObject bulletDirtEffect;

    //Reflectable layermask
    public LayerMask reflectable;

    //Laser positions
    protected Vector3 laserDestroyA;
    protected Vector3 laserDestroyB;
    protected float destroyLerp = 0;

    //Server-side initialization
    [Server]
    public override void Initialize(List<int> damage, int bounces, float fireSpeed, int playerId)
    {
        lineRenderer = GetComponent<LineRenderer>();
        Rpc_PlayerInit();
        base.Initialize(damage, bounces, fireSpeed, playerId);
    }

    //Client-side initialization
    [ClientRpc]
    private void Rpc_PlayerInit()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    
    [Server]
    public override void Update()
    {
        if (lineRenderer.positionCount >= 2)
        {
            //Increase the lerp,
            destroyLerp += (Time.deltaTime * bulletSpeed) / (Vector3.Distance(laserDestroyA, laserDestroyB));
            if (destroyLerp > 1)
                destroyLerp = 1;
            //Set the position of the first point
            lineRenderer.SetPosition(0, Vector3.Lerp(laserDestroyA, laserDestroyB, destroyLerp));

            if(lineRenderer.GetPosition(0) == laserDestroyB)
            {
                //Pull points
                Vector3[] newArray = new Vector3[lineRenderer.positionCount];
                lineRenderer.GetPositions(newArray);
                newArray = newArray.Skip(1).ToArray();
                //Set new length
                lineRenderer.positionCount -= 1;
                //Set new positions
                lineRenderer.SetPositions(newArray);
                if (lineRenderer.positionCount >= 2)
                {
                    //Reset lerping
                    laserDestroyA = lineRenderer.GetPosition(0);
                    laserDestroyB = lineRenderer.GetPosition(1);
                    destroyLerp = 0;
                }
            }

            //Update the Client
            Vector3[] temp = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(temp);
            Rpc_UpdateClientLines(temp);
        } else
        {
            //Destroy the bullet
            DestroyBullet();
        }
    }

    [Server]
    public override void Vel(Vector3 vel, float speed)
    {
        bulletSpeed = speed * 100;
        //Create the array of vec3 points in the line and add the starting point
        List<Vector3> bouncePoints = new List<Vector3>();
        bouncePoints.Add(transform.position);

        //Create ray that transfers through loops and raycasthit
        Ray ray = new Ray(transform.position, vel);
        RaycastHit hit;

        //Checks for floor bouncing
        bool floor = false;

        //Loop through reflections
        for(int i = 0; i <= myShot.maxBounces; i++)
        {
            //Cast ray
            if(Physics.Raycast(ray, out hit, 100, reflectable))
            {
                //Add hit point to the list of line points
                bouncePoints.Add(hit.point);
                speed = 0;

                //If the second hit is on a player and floor is active, reduce the bounce count
                if (hit.transform.CompareTag("Player") && floor)
                {
                    myShot.numBounces = 0;
                }
                else
                {
                    floor = false;
                }

                //Check to see if it hit something
                if (hit.transform.GetComponent<HitInteraction>())
                {
                    //Send hit message
                    hit.transform.SendMessage("Hit", myShot, SendMessageOptions.DontRequireReceiver);


                    //If its an enemy, break
                    if (hit.transform.CompareTag("Player"))
                    {
                        break;
                    }
                }

                //If its first bounce is off the floor, set floor true
                if (hit.transform.CompareTag("Floor") && i == 0)
                {
                    floor = true;
                }

                //Generate the reflection  (Moved to before effect)
                Vector3 reflection = Vector3.Reflect(ray.direction, hit.normal);

                if (hit.transform.CompareTag("Floor") || hit.transform.CompareTag("Wall"))
                {
                    Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    Vector3 pos = hit.point;
                    Instantiate(bulletCollisionEffect, pos, rot);
                    Instantiate(bulletDirtEffect, pos, Quaternion.FromToRotation(Vector3.up, reflection));
                    
                }

                //Increase reflection count
                myShot.numBounces++;
                //If it hasn't been stopped, create a new ray
                ray = new Ray(hit.point, reflection);
            } else //If it didn't hit anything, end the ray
            {
                //Create the endpoint and add it to the lists
                Vector3 endPoint = ray.origin + (ray.direction * 100);
                bouncePoints.Add(endPoint);
                //End loop early
                break;
            }
        }
        //Assign points
        Vector3[] bounces = bouncePoints.ToArray();
        lineRenderer.positionCount = bounces.Length;
        lineRenderer.SetPositions(bounces);

        //Set up vanishing
        laserDestroyA = lineRenderer.GetPosition(0);
        laserDestroyB = lineRenderer.GetPosition(1);
        destroyLerp = 0;

        //Apply the line to the linerenderer
        Rpc_UpdateClientLines(bounces);
    }

    //Update the line positions on the client side
    [ClientRpc]
    protected void Rpc_UpdateClientLines(Vector3[] vectors)
    {
        lineRenderer.positionCount = vectors.Length;
        lineRenderer.SetPositions(vectors);
    }
}
