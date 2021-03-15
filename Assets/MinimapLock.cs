using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapLock : MonoBehaviour
{
    [SerializeField]
    private Camera _minimapCamera;

    // Start is called before the first frame update
    void Start()
    {
        //Camera cam = gameObject.GetComponent<Camera>();
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Quaternion setRotation = Quaternion.Euler(90, -90, 0);
        _minimapCamera.transform.rotation = setRotation;
    }
}
