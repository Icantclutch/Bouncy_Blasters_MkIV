using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChangeCollision : MonoBehaviour
{

    public Camera mapCam;
    float camPos;
    Vector3 position;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Map Changer Collider")
        {
            mapCam.transform.localPosition = new Vector3(0.0f, 10.0f, 0.0f);
           
        }
    }

}
