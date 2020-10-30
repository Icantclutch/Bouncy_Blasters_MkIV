using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTween : MonoBehaviour
{
	public Transform camPos;
	public Transform camLook;
	public float timeToMove = 1f;
	public bool blinkStart = false;

    public CameraTween(Transform camPos, Transform camLook, float timeToMove, bool blinkStart)
    {
        this.camPos = camPos;
        this.camLook = camLook;
        this.timeToMove = timeToMove;
        this.blinkStart = blinkStart;
    }
}
