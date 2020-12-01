using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WallMover : MonoBehaviour
{
    public float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform child in transform)
        {
            float step = speed * Time.deltaTime; // calculate distance to move
            child.position = Vector3.MoveTowards(child.position, child.position + new Vector3(0,50,0), step);
        }
    }
}
