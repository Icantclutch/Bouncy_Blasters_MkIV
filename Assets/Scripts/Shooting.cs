using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform eyes;
    public GameObject bullet;
    public float speed = 18f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
            GameObject b = Instantiate(bullet, eyes.transform.position, eyes.transform.rotation);
            b.GetComponent<Bullet>().Vel(eyes.transform.forward * speed * 100);
        }
    }
}
