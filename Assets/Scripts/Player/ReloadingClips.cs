using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine.UI;
using UnityEngine;

public class ReloadingClips : MonoBehaviour
{
    //much of this is unimportant and was just there for testing. I labeled
    //the part that sets clips sizes and the reloading

    public float gunType;
    float clipSize;
    float originalSize;
    public Text clipDisplay;
    Text temp;
    string displayText;

    
    // Start is called before the first frame update
    void Start()
    {
        
        //rifle
        if (gunType == 0)
        {
            //set clip size for each gun
            clipSize = 30;
           
        }
        //sub machine 
        else if (gunType == 1)
        {
            clipSize = 20;
           
        }
        //shotgun
        else if (gunType == 2)
        {
            clipSize = 8;
            
        }
        //Marksman
        else if (gunType == 3)
        {
            clipSize = 10;
            
        }
        originalSize = clipSize;

    }

    // Update is called once per frame
    void Update()
    {
        //reload when pressing R
        if (Input.GetKeyDown(KeyCode.R))
        {
            clipSize = originalSize;
            Debug.Log("reloading" + clipSize);
        }

        shooting();
        
        displayText = clipSize.ToString();


    }


    public void shooting()
    {
       
        if (Input.GetKeyDown(KeyCode.Space) && clipSize > 0)
        {
            Debug.Log(clipSize);
            clipSize -= 1;

        
        }
    }



}
