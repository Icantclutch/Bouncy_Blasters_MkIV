using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableButton : MonoBehaviour
{
    private float duration = 2;
    public void DisableButtonForDuration(float seconds = 2)
    {
        duration = seconds;
        StartCoroutine(Disable());
    }

    IEnumerator Disable()
    {
        GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(duration);
        GetComponent<Button>().interactable = true;
    }

}
