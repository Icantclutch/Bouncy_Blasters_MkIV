using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutMenuButtons : MonoBehaviour
{
    [SerializeField]
    private List<Weapon> _weaponList = null;

    [SerializeField]
    private Button _weaponButtonPrefab = null;

    [SerializeField]
    private List<Slider> _statSliderList = null;

    [SerializeField]
    private List<Text> _statValueList = null;

    // Start is called before the first frame update
    void Start()
    {
        PopulateLoadoutScreen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PopulateLoadoutScreen()
    {
        for(int i = 0; i < _weaponList.Count; i++)
        {
            Button b = Instantiate(_weaponButtonPrefab, gameObject.transform);
            b.GetComponent<LoadoutWeaponButton>().wep = _weaponList[i];
            b.GetComponent<LoadoutWeaponButton>().SetupButton();
        }
    }

    public void UpdateLoadoutStats(List<int> stats)
    {
  
        for(int i = 0; i < stats.Count; i++)
        {
            _statSliderList[i].value = stats[i];
            _statValueList[i].text = stats[i].ToString();
        }
     
    }

   
}
