using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerEffects : NetworkBehaviour
{
    [SerializeField]
    private Canvas _canvas;
    [SerializeField]
    private GameObject _hitmarker;

    private float _growSize = 1.25f;

    [TargetRpc]
    public void CreateHitmarker()
    {
        GameObject Clone = Instantiate(_hitmarker, _canvas.transform);
        StartCoroutine(HitMarkerEffect(Clone, 255, 0, 0.5f));
    }

    public IEnumerator HitMarkerEffect(GameObject go, float start, float end, float lerpTime)
    {
        float _timeStartedLerping = Time.time;
        float _timeSinceLast = Time.time - _timeStartedLerping;
        float _completedPercent = _timeSinceLast / lerpTime;

        while (true)
        {
            _timeSinceLast = Time.time - _timeStartedLerping;
            _completedPercent = _timeSinceLast / lerpTime;

            //Set the scale
            float currSizeVal = Mathf.Lerp(1, _growSize, _completedPercent);
            go.transform.localScale.Scale(new Vector3(currSizeVal, currSizeVal, currSizeVal));

            //Set the Image alpha (fade)
            float currVal = Mathf.Lerp(start, end, _completedPercent);
            Image newImg = go.GetComponent<Image>();
            Color tempColor = newImg.color;
            tempColor.a = currVal;
            newImg.color = tempColor;
            if (_completedPercent >= 1) break;

            yield return null;
        }
        Destroy(go);
    }
}
