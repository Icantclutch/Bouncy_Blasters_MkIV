using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutWeaponButton : MonoBehaviour
{
    public Weapon wep;

    private GameObject[] _statBars;
    List<int> stats;

   

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetupButton()
    {
        GetComponent<Button>().onClick.AddListener(SetStats);
        GetComponentInChildren<Text>().text = wep.name;
     
        stats = new List<int>(5);
        //Initializing the stats List
        stats.Add(0);stats.Add(0);stats.Add(0);stats.Add(0);stats.Add(0);
    }

    private void SetStats()
    {
        //Fire Rate Stat
        stats[0] = ((int)wep.fireModes[0].fireRate);
        //Ammo Count for Battery Size
        stats[1] = (wep.ammoCount);
        //Raw Bullet Damage
        stats[2] = (wep.fireModes[0].bulletDamage[0]);

        //Max Bullet Damage
        int ricochets = wep.fireModes[0].bulletDamage.Count;
       

        stats[3] = (wep.fireModes[0].bulletDamage[ricochets - 1]);

        //number of Ricochets
        stats[4] = (ricochets - 1);

        Debug.Log("Press button");

        GetComponentInParent<LoadoutMenuButtons>().UpdateLoadoutStats(stats);
    }
}
