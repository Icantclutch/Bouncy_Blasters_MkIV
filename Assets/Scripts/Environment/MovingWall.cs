using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MovingWall : NetworkBehaviour
{
    [SyncVar]
    float clock;
<<<<<<< HEAD
    //How fast the wall completes a cycle
=======
>>>>>>> parent of 5d237103 (Wall Moving)
    [SyncVar]
    public float SpeedMultiplier;

    public AnimationCurve MovementCycle;

    [SyncVar, Range(0,1)]
    public float startValue = 0f;
    [SyncVar]
    Vector3 startPos;
    [SyncVar]
    public Vector3 fullyUp;
    [SyncVar]
    public Vector3 fullyDown;

    [Server]
    private void Start()
    {
        clock = startValue;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< HEAD
        //Tick clock
=======
>>>>>>> parent of 5d237103 (Wall Moving)
        if (isServer)
        {
            clock += Time.deltaTime;
        }

<<<<<<< HEAD
        //Lerp transform between up and down position
=======
>>>>>>> parent of 5d237103 (Wall Moving)
        transform.position = startPos + Vector3.Lerp(fullyDown, fullyUp, MovementCycle.Evaluate((clock * SpeedMultiplier) % 1f));
    }
}
