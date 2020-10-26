using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bullet : MonoBehaviour
{
    [System.Serializable]
    public class Shot
    {
        //how much raw damage the shot does
        public List<int> damage;
        //how many bounces the shot has had so far
        public int maxBounces;
        //how many times the shot has bounced
        [Tooltip("Numner of times the laser has bounced")]
        public int numBounces = 0;
        //Player sourced from
        public PlayerReference playerSource;
    }

    //Rigidbody
    private Rigidbody rb;
    //Shot info
    public Shot myShot;

    // Initialize the bullet
    public virtual void Initialize(Weapon.FireMode myFireMode, PlayerReference playerSource)
    {
        //Initialize shot
        myShot = new Shot();
        //Set values
        myShot.damage = myFireMode.bulletDamage;
        myShot.maxBounces = myFireMode.maxBounces;
        //Assign player
        myShot.playerSource = playerSource;

        //Set bullet speed
        Vel(transform.forward, myFireMode.fireSpeed * 100);

        //Destroy(gameObject, 3f); //REMOVE AFTER DEBUGGING
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if(myShot.numBounces == myShot.maxBounces)
        {
            DestroyBullet();
        }
    }

    public virtual void Vel(Vector3 vel, float speed)
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(vel * speed);
    }

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

        //Increase the bounces if the bullet still lives
        myShot.numBounces++;
    }

    public void DestroyBullet(float time = 0)
    {
        Debug.Log("Whats Triggering Me");
        //Destroy(gameObject, time);
    }
}
