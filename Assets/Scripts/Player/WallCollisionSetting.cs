using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollisionSetting : MonoBehaviour
{
    public GameObject wallInstance;
<<<<<<< HEAD
    public GameObject normal;
    public GameObject aim;
    //public RaycastHit getHit;
    public GameObject scriptObject;
    private RaycastReflection rayScript;
    bool ishit;
    private void Start()
=======
    //public Ray myRay;
    //private RaycastReflection rayScript;

    //GameObject aimRay = GameObject.Find("RaycastReflection");

    
    void detectCollision(Collider col)
>>>>>>> parent of 64a14082 (Merge branch 'main' of https://github.com/Icantclutch/Bouncy_Blasters_MkIV into main)
    {
        rayScript = scriptObject.GetComponent<RaycastReflection>();
    }
<<<<<<< HEAD

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
                Debug.Log("Wall error");
            }
        }


    }



=======
    
>>>>>>> parent of 64a14082 (Merge branch 'main' of https://github.com/Icantclutch/Bouncy_Blasters_MkIV into main)
}
