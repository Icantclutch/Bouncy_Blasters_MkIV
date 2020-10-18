using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Ocsilating : MonoBehaviour
{
    public float speed = 5f;
    
    public Vector3 offset = new Vector3(0, 0, -5);
    private Vector3 startPos;
    private Vector3 targetPos;
    private bool moveAway = true;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + offset;
        moveAway = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveAway)
        {
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

            if ((transform.position - targetPos).magnitude < 0.1)
            {
                moveAway = !moveAway;
            }
        }
        else
        {
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, startPos, step);

            if ((transform.position - startPos).magnitude < 0.1)
            {
                moveAway = !moveAway;
            }
        }
        
        
    }
}
