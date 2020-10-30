using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectiles : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject firePoint;
    public List<GameObject> vfx = new List<GameObject>();
    public RotateToMouse rotateToMouse;
    public Camera CamToShake;
    private CameraShaking ShakeScript;

    private GameObject effectToSpawn;
    private float timeToFire = 0;
    void Start()
    {
        ShakeScript = CamToShake.GetComponent<CameraShaking>();
        effectToSpawn = vfx[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / effectToSpawn.GetComponent<ProjectileMove>().fireRate;
            SpawnVFX();
        }
    }

    void SpawnVFX()
    {

        GameObject vfx;
        if (firePoint != null)
        {
            vfx = Instantiate(effectToSpawn, firePoint.transform.position, Quaternion.identity);
            if (rotateToMouse != null)
            {
                vfx.transform.localRotation = rotateToMouse.GetRotation();
            }

            ShakeScript.shakeDuration = 0.3f;
            //ShakeScript

        } else
        {
            Debug.Log("No Fire Point");
        }
    }
}
