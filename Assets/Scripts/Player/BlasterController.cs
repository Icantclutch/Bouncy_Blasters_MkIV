using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
public class BlasterController : MonoBehaviour
{

    public List<GameObject> blasters;
    public GameObject currentBlaster = null;
    public int currentBlasterIndex;

    [SerializeField]
    private float _shotingEffectTime = 1.5f;
    private float _shootingEffectTimer = 0;

    private void Start()
    {
        int i = 0;
        //Sets the first active blaster model as the currentBlaster
        foreach (GameObject blaster in blasters)
        {
            if(currentBlaster != null && currentBlaster != blaster)
            {
                blaster.SetActive(false);
            }
            else if (blaster.activeSelf)
            {
                currentBlaster = blaster;
                currentBlasterIndex = i;
            }
            ++i;
        }
    }

    private void Update()
    {
        //Keep the shooting effect enabled for a set duration
        if(_shootingEffectTimer > 0)
        {
            _shootingEffectTimer -= Time.deltaTime;
            //Disable the shooting effect after the set duration
            if(_shootingEffectTimer <= 0 && currentBlaster)
            {
                currentBlaster.GetComponentInChildren<Barrel>().shootingEffect.SetActive(false);
                /*foreach (Transform child in currentBlaster.transform)
                {
                    if (child.tag == "Barrel")
                    {
                        child.Find("Blaster Firing-VFX Graph").gameObject.SetActive(false);
                        break;
                    }
                }*/
            }
        }
    }

    /// <summary>
    /// Enables the shooting particle effect and sets the duration it should be active for
    /// </summary>
    public void StartShootingEffect()
    {
        if(_shootingEffectTimer <= 0)
        {
            currentBlaster.GetComponentInChildren<Barrel>().shootingEffect.SetActive(true);
            /*foreach (Transform child in currentBlaster.transform)
            {
                if (child.tag == "Barrel")
                {
                    child.Find("Blaster Firing-VFX Graph").gameObject.SetActive(true);
                    break;
                }
            }*/
        }
        _shootingEffectTimer = _shotingEffectTime;
        
    }

    /// <summary>
    /// Swaps weapon model based on game object name
    /// (can be unreliable)
    /// </summary>
    public bool SwapTo(string blasterName)
    {
        int i = 0;
        //Loop throught the list of blaster models
        foreach(GameObject blaster in blasters)
        {
            if (blaster.name.Contains(blasterName))
            {
                if(blaster != currentBlaster)
                {
                    //Disable previous blaster model
                    if(currentBlaster != null)
                    {
                        currentBlaster.SetActive(false);
                    }

                    blaster.SetActive(true);
                    currentBlaster = blaster;
                    currentBlasterIndex = i;
                    return true;
                }
                else
                {
                    return true;
                }
            }
            ++i;
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
    public bool SwapTo(int blasterIndex)
    {
        
            if (blasterIndex < blasters.Count && blasterIndex >= 0)
            {
                if (blasters[blasterIndex] != currentBlaster)
                {
                    //Disable previous blaster model
                    if (currentBlaster != null)
                    {
                        currentBlaster.SetActive(false);
                    }

                    blasters[blasterIndex].SetActive(true);
                    currentBlaster = blasters[blasterIndex];
                currentBlasterIndex = blasterIndex;
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
