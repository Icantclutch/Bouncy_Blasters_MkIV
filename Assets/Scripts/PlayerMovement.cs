using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float xAxis;
    private float zAxis;
    private Rigidbody rb;

    public int speed = 5;

    // Start is called before the first frame update
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");
        if (xAxis > 0)
        {
            rb.AddForce(transform.right * speed);
        }
        else if (xAxis < 0)
        {
            rb.AddForce(transform.right * -speed);
        }

        if(zAxis > 0)
        {
            rb.AddForce(transform.forward * speed);
        }
        else if (zAxis < 0)
        {
            rb.AddForce(transform.forward * -speed);
        }
       
    }
}
