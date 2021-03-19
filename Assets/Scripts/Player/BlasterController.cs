using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterController : MonoBehaviour
{

    public List<GameObject> blasters;
    public GameObject currentBlaster = null;

    private void Start()
    {
        foreach (GameObject blaster in blasters)
        {
            if(currentBlaster != null)
            {
                blaster.SetActive(false);
            }
            else if (blaster.activeSelf)
            {
                currentBlaster = blaster;
            }
        }
    }
    /// <summary>
    /// Swaps weapon model based on game object name
    /// (can be unreliable)
    /// </summary>
    public bool swapTo(string blasterName)
    {
        foreach(GameObject blaster in blasters)
        {
            if (blaster.name.Contains(blasterName))
            {
                if(blaster != currentBlaster)
                {
                    if(currentBlaster != null)
                    {
                        currentBlaster.SetActive(false);
                    }

                    blaster.SetActive(true);
                    currentBlaster = blaster;
                    return true;
                }
                else
                {
                    return true;
                }
            }
        }

        return false;
    }
    /// <summary>
    /// Swaps weapon model based on index in List
    /// <para>
    /// 0: Auto blaster<br/>
    /// 1: Beam Rifle<br/>
    /// 2: Marksman Rifle<br/>
    /// 3: Pistol<br/>
    /// 4: Shotgun<br/>
    /// 5: SMG<br/>
    /// </para>
    /// </summary>
    public bool swapTo(int blasterIndex)
    {
        
            if (blasterIndex < blasters.Count && blasterIndex >= 0)
            {
                if (blasters[blasterIndex] != currentBlaster)
                {
                    if (currentBlaster != null)
                    {
                        currentBlaster.SetActive(false);
                    }

                    blasters[blasterIndex].SetActive(true);
                    currentBlaster = blasters[blasterIndex];
                    return true;
                }
                else
                {
                    return true;
                }
            }
        

        return false;
    }
}
