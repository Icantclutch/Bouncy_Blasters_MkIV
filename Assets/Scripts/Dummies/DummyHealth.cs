using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DummyHealth : MonoBehaviour
{

    private Slider Bar;
    public GameObject linkedEnemy;
    private Dummies curHealth;
    // Start is called before the first frame update
    void Start()
    {
        //get the Dummies script component, the slider this script is attatched to, and values of the Dummies script
        curHealth = linkedEnemy.GetComponent<Dummies>();
        Bar = GetComponent<Slider>();
        Bar.maxValue = curHealth.getMax();
        Bar.value = curHealth.getCharge();

    }

    // Update is called once per frame
    void Update()
    {
        //set the slider's value to the charge from the Dummies script
        Bar.value = curHealth.getCharge();
        
    }
}
