using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollisionSetting : MonoBehaviour
{
    public GameObject wallInstance;
    public Ray myRay;
    private RaycastReflection rayScript;

    GameObject aimRay = GameObject.Find("RaycastReflection");

    
    void detectCollision(Collider col)
    {
        
    }
    
}
