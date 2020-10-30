using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CameraShaking : MonoBehaviour
{
	public List<CameraTween> TweensToDo = new List<CameraTween>();

	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;

	// How long the object should shake for.
	public float shakeDuration = 2f;

	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;

	//Getting Destinations
	public void AddCameraTween(Transform camPos, Transform camLook, float timeToMove, bool blinkStart)
    {
		TweensToDo.Add(new CameraTween(camPos, camLook, timeToMove, blinkStart));
    }

	Vector3 originalPos;

	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}

	void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	public void LookAtPosition(Vector3 goPosition, Vector3 lookPosition, float speed)
	{
		StartCoroutine(LookAtTransformCorutine(goPosition, lookPosition, speed));
	}

	public void LookAtPosition(Transform goPosition, Transform lookPosition, float speed)
	{
		LookAtPosition(goPosition.position, lookPosition.position, speed);
	}

	public IEnumerator LookAtTransformCorutine(Vector3 goPosition, Vector3 lookPosition, float speed)
	{
		while (camTransform.position != lookPosition)
		{
			camTransform.position = Vector3.Lerp(camTransform.position, goPosition, speed);
			Vector3 holder = Vector3.Lerp(camTransform.position, goPosition, speed);
			camTransform.LookAt(holder);
			transform.localRotation = Quaternion.LookRotation(transform.forward);
			yield return new WaitForEndOfFrame();
		}
	}

	void Update()
	{

		if (TweensToDo.Count > 0)
		{
			CameraTween extra = TweensToDo[0];
			camTransform.LookAt(extra.camLook);
			LookAtPosition(extra.camPos, extra.camLook, extra.timeToMove);
		}
		else
		{
			if (shakeDuration > 0)
			{
				camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

				shakeDuration -= Time.deltaTime * decreaseFactor;
			}
			else
			{
				shakeDuration = 0f;
				camTransform.localPosition = originalPos;
			}
		}
	}
}

