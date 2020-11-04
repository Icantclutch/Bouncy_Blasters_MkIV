using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;



public class Wall_Movement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private float movementSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        //float verticalInput = Input.GetAxis("Vertical");
        //transform.position = transform.position + new Vector3(horizontalInput * movementSpeed * Time.deltaTime, movementSpeed * Time.deltaTime, 0);
        //if (transform.position != (-0.6970215, 10.36052, -23.93095)){
        //    transform.position = transform.position + new Vector3(horizontalInput * movementSpeed * Time.deltaTime, movementSpeed * Time.deltaTime, 0);
        //}
        Debug.Log(transform.position);
    }
}
