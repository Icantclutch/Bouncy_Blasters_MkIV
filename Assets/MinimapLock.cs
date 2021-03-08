using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapLock : MonoBehaviour
{
    public GameObject cam;

    // Start is called before the first frame update
    void Start()
    {
        //Camera cam = gameObject.GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
    }
}
