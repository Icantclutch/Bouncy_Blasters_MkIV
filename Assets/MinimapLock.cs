using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapLock : MonoBehaviour
{
    public Camera minimapCamera;

    // Start is called before the first frame update
    void Start()
    {
        //Camera cam = gameObject.GetComponent<Camera>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion setRotation = Quaternion.Euler(90, -90, 0);
        minimapCamera.transform.rotation = setRotation;
    }
}
