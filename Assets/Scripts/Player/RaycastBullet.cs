using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RaycastBullet : Bullet
{
    //Line renderer
    private LineRenderer lineRenderer;

    //Reflectable layermask
    public LayerMask reflectable;

    public override void Initialize(Weapon.FireMode myFireMode)
    {
        lineRenderer = GetComponent<LineRenderer>();
        base.Initialize(myFireMode);
    }

    public override void Update()
    {
        
    }

    public override void Vel(Vector3 vel, float speed)
    {
        //Create the array of vec3 points in the line and add the starting point
        List<Vector3> bouncePoints = new List<Vector3>();
        bouncePoints.Add(transform.position);
        Debug.Log(bouncePoints.Count);

        //Create ray that transfers through loops and raycasthit
        Ray ray = new Ray(transform.position, vel);
        RaycastHit hit;

        //Loop through reflections
        for(int i = 0; i < myShot.maxBounces; i++)
        {
            //Cast ray
            if(Physics.Raycast(ray, out hit, 100, reflectable))
            {
                //Add hit point to the list of line points
                bouncePoints.Add(hit.point);
                //Check to see if it hit something
                if (hit.transform.GetComponent<HitInteraction>())
                {
                    //Send hit message
                    hit.transform.SendMessage("Hit", myShot);

                    //If its an enemy, break
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        break;
                    }
                }
                //Increase reflection count
                myShot.numBounces++;
                //Generate the reflection
                Vector3 reflection = Vector3.Reflect(ray.direction, hit.normal);
                //If it hasn't been stopped, create a new ray
                ray = new Ray(hit.point, reflection);
            } else //If it didn't hit anything, end the ray
            {
                //Create the endpoint and add it to the lists
                Vector3 endPoint = ray.origin + (ray.direction * 100);
                bouncePoints.Add(endPoint);
                //End loop early
                break;
            }
        }
        //Apply the line to the linerenderer
        Vector3[] arrayVecs = bouncePoints.ToArray();
        lineRenderer.positionCount = arrayVecs.Length;
        lineRenderer.SetPositions(arrayVecs);

        StartCoroutine(FancyDestroy(speed));
    }

    IEnumerator FancyDestroy(float speed)
    {
        //loop through each position
        while(lineRenderer.positionCount > 1)
        {
            //Get start, end, and direction
            Vector3 start = lineRenderer.GetPosition(0);
            Vector3 end = lineRenderer.GetPosition(1);

            //Create lerp
            float lerp = 0;
            //when start != end
            while (lineRenderer.GetPosition(0) != lineRenderer.GetPosition(1))
            {
                //Set the position of the first point
                lineRenderer.SetPosition(0, Vector3.Lerp(start, end, lerp));
                //Increase the lerp,
                lerp += Time.deltaTime * speed/Vector3.Distance(start, end);
                //Wait
                yield return null;
            }

            //Pull points
            Vector3[] newArray = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(newArray);
            newArray = newArray.Skip(1).ToArray();
            //Set new length
            lineRenderer.positionCount -= 1;
            //Set new positions
            lineRenderer.SetPositions(newArray);

            //Wait
            yield return null;
        }

        //Destroy the bullet
        DestroyBullet();
    }
}
