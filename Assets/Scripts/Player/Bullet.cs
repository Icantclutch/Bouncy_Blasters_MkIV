﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class Bullet : NetworkBehaviour
{
    [System.Serializable]
    public struct Shot
    {
        //how much raw damage the shot does
        public List<int> damage;
        //how many bounces the shot has had so far
        public int maxBounces;
        //how many times the shot has bounced
        [Tooltip("Numner of times the laser has bounced")]
        public int numBounces;
        //The team of the player that fired the bullet
        public int playerID;
        //Speed of the bullet
        public float speed;
    }

    //Rigidbody
    private Rigidbody rb;
    //Shot info
    [SyncVar]
    public Shot myShot;

    [Server]
    // Initialize the bullet
    public virtual void Initialize(List<int> damage, int bounces, float fireSpeed, int playerId)
    {
        //Initialize shot
        myShot = new Shot();
        //Set values
        myShot.damage = damage;
        myShot.maxBounces = bounces;
        myShot.numBounces = 0;
        myShot.playerID = playerId;
        myShot.speed = fireSpeed;

        //Set bullet speed
        Vel(transform.forward, myShot.speed);
    }

    [Server]
    // Update is called once per frame
    public virtual void Update()
    {
        if(myShot.numBounces == myShot.maxBounces)
        {
            DestroyBullet();
        }
    }

    [Server]
    public virtual void Vel(Vector3 vel, float speed)
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(vel * speed);
    }

    [Server]
    private void OnCollisionEnter(Collision collision)
    {
        //Send message to hit object
        collision.transform.SendMessage("Hit", myShot, SendMessageOptions.DontRequireReceiver);

        //If its an enemy, destroy the bullet
        if (collision.transform.CompareTag("Enemy"))
        {
            DestroyBullet();
            return;
        }

        print("HIT WALL1");

        //Increase the bounces if the bullet still lives
        myShot.numBounces++;
    }

    [Server]
    public virtual void DestroyBullet()
    {
        NetworkServer.Destroy(gameObject);
    }
}
