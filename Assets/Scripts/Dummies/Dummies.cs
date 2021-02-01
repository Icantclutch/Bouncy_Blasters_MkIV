﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public class Dummies : MonoBehaviour
{

    private float currentCharge;
    private float maxCharge;
    public GameObject Dying;
    private float timer;
    private NavMeshAgent agent;
    public GameObject destination1;
    public GameObject destination2;
    private bool atDest2;


    // Start is called before the first frame update
    void Start()
    {
        atDest2 = true;
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        currentCharge = 0;
        maxCharge = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
        Death();
        dummyMove();
        
    }

    //get charge and getMax are so that the dummy health script can access the currentCharge and maxCharge
    public float getCharge()
    {
        return currentCharge;
    }
    public float getMax()
    {
        return maxCharge;
    }


    //detects type of shot the dummy is hit with
    public void Hit(Bullet.Shot shot)
    {
        
        //Deal damage based on shot type and amount of bounces
        currentCharge += shot.damage[shot.numBounces];
            

    }

    private void Death()
    {
        //if charge is over the max
        if (currentCharge >= maxCharge)
        {
            //disable the dummy object and start the timer
            Dying.SetActive(false);
            timer += Time.deltaTime;
        }

        
        //after 10 seconds the dummy will respawn, resetting the timer and charge
        if (timer >= 10)
        {
            timer = 0;
            currentCharge = 0;
            Dying.SetActive(true);

        }

    }


    private void dummyMove()
    {
        if (destination1 && destination2)
        {
            Debug.Log(atDest2);

            if (agent.remainingDistance < .5f && atDest2 == true)
            {
                atDest2 = false;
                agent.SetDestination(destination2.transform.position);

            }
            else if (agent.remainingDistance < .5f && atDest2 == false)
            {
                atDest2 = true;
                agent.SetDestination(destination1.transform.position);
            }
        }
    }

    

}

