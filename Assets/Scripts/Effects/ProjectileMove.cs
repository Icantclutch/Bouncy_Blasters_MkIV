using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject explosionPrefab;
    public float speed;
    public float fireRate;
    private Vector3 SavedPos;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (speed != 0)
        {
            SavedPos = transform.position;
            transform.position += transform.forward * (speed * Time.deltaTime);
        } else
        {
            Debug.Log("No Speed");
        }
    }

    void OnCollisionEnter(Collision co)
    {
        if (co.gameObject.name == "WALL")
        {
            speed = 0;
            ContactPoint contact = co.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;
            Instantiate(explosionPrefab, pos, rot);

            Destroy(gameObject);
        }
    }
}
