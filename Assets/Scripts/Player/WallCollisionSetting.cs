using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollisionSetting : MonoBehaviour
{
    public GameObject wallInstance;
    public GameObject normal;
    public GameObject aim;
    //public RaycastHit getHit;
    public GameObject scriptObject;
    private RaycastReflection rayScript;
    private void Start()
    {
        rayScript = scriptObject.GetComponent<RaycastReflection>();
    }

    private void Update()
    {
        if(rayScript.hit.collider.name == wallInstance.name)
        {
            if(normal.active == true)
            {
                normal.SetActive(false);
                aim.SetActive(true);
            }
            else
            {
                Debug.Log("laser error. blue wall should have by default set to active unless told otherwise");
            }
        }


    }



}
