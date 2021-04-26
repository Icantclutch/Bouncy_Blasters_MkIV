using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollisionSetting : MonoBehaviour
{
    [SerializeField]
    private GameObject wallInstance;

    [SerializeField]
    private GameObject normalImage;

    [SerializeField]
    private GameObject aimImage;

    [SerializeField]
    private bool isHit = false;
    




   
    private void Start()
    {
        
    }

    private void Update()
    {
        if (isHit)
        {
            normalImage.SetActive(false);
            aimImage.SetActive(true);
            
        }
        else
        {
            normalImage.SetActive(true);
            aimImage.SetActive(false);
           
        }
        isHit = false;
    }

    public void ActivateWall()
    {
        isHit = true;
        Debug.Log("Hitting Wall");
    }


}
