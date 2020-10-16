using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    public AudioClip ricClip;
    public AudioClip hit;
    private AudioSource audSrc;
    // Start is called before the first frame update
    void Start()
    {
        audSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HitEnemy(bool ricochet)
    {
        Debug.Log("Hit");
        if (ricochet)
        {
            audSrc.PlayOneShot(ricClip);
            
        }
        else
        {
            audSrc.PlayOneShot(hit);

        }

    }

   
}
