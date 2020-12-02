using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTween : MonoBehaviour
{
	public Transform camPos;
	public Transform camLook;
	public float timeToMove = 1f;


    public CameraTween(Transform camPos, Transform camLook, float timeToMove)
    {
        this.camPos = camPos;
        this.camLook = camLook;
        this.timeToMove = timeToMove;
    }
}
