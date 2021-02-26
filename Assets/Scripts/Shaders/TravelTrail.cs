using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TravelTrail : MonoBehaviour
{
    public TrailRenderer TR;
    public TravelBullet TB;
    public Color earlyColor;
    public Color lateColor;
    public Color lerpColor;

    // Start is called before the first frame update
    void Start()
    {
        TR = GetComponent<TrailRenderer>();
        TB = GetComponentInParent<TravelBullet>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log((float)TB.myShot.numBounces / TB.myShot.maxBounces);
        lerpColor = Color.Lerp(earlyColor, lateColor, (float)TB.myShot.numBounces / TB.myShot.maxBounces);

        TR.startColor = lerpColor;
        TR.endColor = Color.white;
    }
}
