using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    public int maxBounces = 6;
    private int numBounces = 0;
    private Vector3 initialVelocity;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        if(numBounces == maxBounces)
        {
            //Debug.Log("Destroying bullet");
            Destroy(gameObject);
        }
        
    }

    public void ExitGlass()
    {
        rb.velocity = initialVelocity;
    }
    public void Vel(Vector3 vel)
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(vel);
        initialVelocity = vel;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Num bounces:" + numBounces);
        if (collision.transform.tag == "Enemy")
        {
            Destroy(gameObject);
            if(numBounces > 1)
            {
                collision.transform.GetComponent<EnemyTarget>().HitEnemy(true);
            }
            else
            {
                collision.transform.GetComponent<EnemyTarget>().HitEnemy(false);
            }
            
        }
        numBounces++;
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
