using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//SYSTEM TO BE USED FOR PETS/COMPANIONS AROUND THE PLAYER HEAD
//NOT IN THE GAME YET
public class RotateToMouse : MonoBehaviour
{
    //Get Variables
    public Camera cam;
    public float maximumLength;

    private Ray rayMouse;
    private Vector3 pos;
    private Vector3 direction;
    private Quaternion rotation;

    // Update is called once per frame
    void Update()
    {
        if (cam != null)
        {
            //Create a ray cast with the mouse position on screen

            RaycastHit hit;
            var mousePos = Input.mousePosition;
            rayMouse = cam.ScreenPointToRay(mousePos);
            if (Physics.Raycast(rayMouse.origin, rayMouse.direction, out hit, maximumLength))
            {
                RotateToMouseDirection(gameObject, hit.point);
            } else
            {   
                var pos = rayMouse.GetPoint(maximumLength);
                RotateToMouseDirection(gameObject, pos);
            }
        } else
        {
            Debug.Log("No Camera");
        }
    }

    void RotateToMouseDirection(GameObject obj, Vector3 destination)
    {
        direction = destination - obj.transform.position;
        rotation = Quaternion.LookRotation(direction);
        //Change localRotation of the fireobject
        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
    }

    //Return the rotation
    public Quaternion GetRotation()
    {
        return rotation;
    }
}
