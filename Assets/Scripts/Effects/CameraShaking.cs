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
	public Transform camHolderTransform;

	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;

	//Used for rendering camera movement completion
	bool done = true;

	//Reference to an origin
	Vector3 originalPos;


	//Called from other scripts (will be used for the menu system or cinematics)
	public void AddCameraTween(Transform camPos, Transform camLook, float timeToMove)
    {
		TweensToDo.Add(new CameraTween(camPos, camLook, timeToMove));
    }

	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
		if (camHolderTransform == null)
		{
			camHolderTransform = camTransform.parent.transform;
		}
	}
	void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	//Runs the Ienumtator (Changes DONE on completion)
	public IEnumerator LookAtTransformCorutine(Vector3 goPosition, Vector3 lookPosition, float speed)
	{
		var currentPos = camHolderTransform.position;
		var t = 0f;
		while (t < 1)
		{
			t += Time.deltaTime / speed;
			Vector3 Holder1 = Vector3.Lerp(currentPos, goPosition, t);
			camHolderTransform.position = Holder1;

			Vector3 holder = Vector3.Lerp(camTransform.position, lookPosition, t);
			camTransform.LookAt(holder);
			yield return new WaitForEndOfFrame();
		}
		done = true;
		//Changing DONE allows the camera to continue moving
	}

	void Update()
	{
		if (TweensToDo.Count > 0)
		{
			if (done == true)
			{
				CameraTween extra = TweensToDo[0];
				//camTransform.LookAt(extra.camLook);
				done = false;
				StartCoroutine(LookAtTransformCorutine(extra.camPos.position, extra.camLook.position, extra.timeToMove));
				TweensToDo.Remove(extra);
				shakeDuration = 0f;
			}
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

