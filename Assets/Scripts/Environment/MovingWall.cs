using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(NetworkIdentity))]
public class MovingWall : NetworkBehaviour
{
    //Clock used to track cycles
    [SyncVar]
    public float clock;
    [SyncVar]
    public float SpeedMultiplier;

    //The curve representing the y-axis movement of the wall
    public AnimationCurve MovementCycle;

    //THe value on the curve the wall starts at, in a range of 0-1
    [SyncVar, Range(0,1)]
    public float startValue = 0f;
    //Recorded starting position
    [SyncVar]
    Vector3 startPos;
    //The relative position when fully up (typically Vector3.zero)
    [SyncVar]
    public Vector3 fullyUp;
    //The relative position when fully down
    [SyncVar]
    public Vector3 fullyDown;

    [Server]
    private void Start()
    {
        clock = startValue/SpeedMultiplier;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPos + Vector3.Lerp(fullyDown, fullyUp, MovementCycle.Evaluate((clock * SpeedMultiplier) % 1f));
    }

    [Server]
    private void FixedUpdate()
    {
        clock += Time.fixedDeltaTime;
    }
}
