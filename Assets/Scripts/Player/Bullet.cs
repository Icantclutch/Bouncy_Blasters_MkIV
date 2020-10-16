using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [System.Serializable]
    public class Shot
    {
        //how much damage the shot does
        public int damage;
        //how many bounces the shot has had so far
        public int maxBounces;
        //how many times the shot has bounced
        public int numBounces = 0;
    }

    //Rigidbody
    private Rigidbody rb;
    //Shot info
    public Shot myShot;

    // Initialize the bullet
    public void Initialize(Weapon.FireMode myFireMode)
    {
        //Initialize shot
        myShot = new Shot();
        //Set values
        myShot.damage = myFireMode.bulletDamage;
        myShot.maxBounces = myFireMode.maxBounces;

        //Set bullet speed
        Vel(transform.forward * myFireMode.fireSpeed * 100);

        Destroy(gameObject, 3f); //REMOVE AFTER DEBUGGING
    }

    // Update is called once per frame
    void Update()
    {
        if(myShot.numBounces == myShot.maxBounces)
        {
            DestroyBullet();
        }
        
    }

    public void Vel(Vector3 vel)
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(vel);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Send message to hit object
        collision.transform.SendMessage("Hit", myShot);

        //If its an enemy, destroy the bullet
        if (collision.transform.CompareTag("Enemy"))
        {
            DestroyBullet();
            return;
        }

        //Increase the bounces if the bullet still lives
        myShot.numBounces++;
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
