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
        lerpColor = Color.Lerp(earlyColor, lateColor, (float)TB.myShot.numBounces / TB.myShot.maxBounces);

        TR.colorGradient.SetKeys(
            new GradientColorKey[] {new GradientColorKey(lerpColor,0), new GradientColorKey(Color.white, 1)},
            new GradientAlphaKey[] {new GradientAlphaKey(1,0), new GradientAlphaKey(1, 1)}
            );
    }
}
