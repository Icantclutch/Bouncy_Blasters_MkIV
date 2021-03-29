using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpacitySetting : MonoBehaviour
{
    public Slider opacitySlider;
    public RawImage minimap;
    public Image minimapMask;
    float opaque = 0.0f;
  


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
            opaque = opacitySlider.value;
            minimap.color = new Color(255.0f, 255.0f, 255.0f, opaque);
        
    }

  
    //public void changeOpacity()
    //{
    //    //if(opacitySlider.value == 100)
    //    //{
    //    //    minimapCamera.backgroundColor = new Color(18.0f, 16.0f, 16.0f, 1);
    //    //}
    //    //else if (opacitySlider.value == 10)
    //    //{

    //    //}

    //}
}
