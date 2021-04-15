using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class RaycastReflection : MonoBehaviour
{
    //this game object's Transform  
    private Transform goTransform;
    //the attached line renderer  
    private LineRenderer lineRenderer;
    //Parent shooting
    private Shooting shooting;
    //Parent Movement
    private PlayerMovement movement;

    //a ray  
    public Ray ray;
    //a RaycastHit variable, to gather informartion about the ray's collision  
    public RaycastHit hit;

    //reflection direction  
    private Vector3 inDirection;

    //the number of reflections  
    public int nReflections = 2;

    //layermask for reflections
    public LayerMask reflectable;

    //the number of points at the line renderer  
    private int nPoints;

    //Materials
    public Material redMat;
    public Material greenMat;


    void Awake()
    {
        //get the attached Transform component  
        goTransform = this.GetComponent<Transform>();
        //get the attached LineRenderer component  
        lineRenderer = this.GetComponent<LineRenderer>();
        //get the parent shooting
        shooting = this.GetComponentInParent<Shooting>();
        movement = this.GetComponentInParent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetKey(Keybinds.Zoom))
        {
            lineRenderer.enabled = true;
            movement.Aiming(true);
        }
        else
        {
            lineRenderer.enabled = false;
            movement.Aiming(false);
        }

        Transform barrel = null;
        //Update reflections based on player's gun
        if (shooting)
        {
            nReflections = shooting.currentFireMode.maxBounces;
            barrel = shooting.GetComponentInChildren<BlasterController>().currentBlaster.transform.Find("Barrel");
            Debug.Log(barrel);
        }
        else
        {
            nReflections = 4;
        }

        //cast a new ray forward, from the current attached game object position  
        ray = new Ray(goTransform.position, goTransform.forward);

        //represent the ray using a line that can only be viewed at the scene tab  
        Debug.DrawRay(goTransform.position, goTransform.forward * 100, Color.magenta);

        //set the number of points to be the same as the number of reflections  
        nPoints = nReflections;
        //make the lineRenderer have nPoints  
        lineRenderer.positionCount = nPoints;

        //Get bounce points
        List<Vector3> bouncePoints = new List<Vector3>();
        
        if (barrel)
        {
            //Set the first point of the line at the current barrel position  
            lineRenderer.SetPosition(0, barrel.position);

            bouncePoints.Add(barrel.position);
        }
        else
        {
            //Set the first point of the line at the current attached game object position  
            lineRenderer.SetPosition(0, goTransform.position);

            bouncePoints.Add(goTransform.position);
        }
        //Set the color to red
        lineRenderer.material = redMat;

        

        //Loop through reflections
        for (int i = 0; i <= nReflections; i++)
        {
            //Cast ray
            if (Physics.Raycast(ray, out hit, 100, reflectable))
            {
                //Add hit point to the list of line points
                bouncePoints.Add(hit.point);

                //If its an enemy, break
                if (hit.transform.CompareTag("Player"))
                {
                    lineRenderer.material = greenMat;
                    break;
                }

                //Generate the reflection
                Vector3 reflection = Vector3.Reflect(ray.direction, hit.normal);

                //If it hasn't been stopped, create a new ray
                ray = new Ray(hit.point, reflection);
            }
            else //If it didn't hit anything, end the ray
            {
                //Create the endpoint and add it to the lists
                Vector3 endPoint = ray.origin + (ray.direction * 100);
                bouncePoints.Add(endPoint);
                //End loop early
                break;
            }
        }

        //Assign points
        Vector3[] bounces = bouncePoints.ToArray();
        lineRenderer.positionCount = bounces.Length;
        lineRenderer.SetPositions(bounces);
    }
}
