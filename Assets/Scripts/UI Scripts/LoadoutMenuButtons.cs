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

    [SerializeField]
    private List<Sprite> _weaponIcons = null;

    [SerializeField]
    private Text _weaponTextDisplay = null;

    // Start is called before the first frame update
    void Start()
    {
        PopulateLoadoutScreen();
    }


    //1 Max 800
    //2 max 20
    //3 max 25
    //4 max 200
    //5 max 4
    // Update is called once per frame
    void Update()
    {

    }

    private void PopulateLoadoutScreen()
    {
        for (int i = 0; i < _weaponList.Count; i++)
        {
            Button b = Instantiate(_weaponButtonPrefab, gameObject.transform);
            b.GetComponent<LoadoutWeaponButton>().wep = _weaponList[i];
            b.GetComponent<Button>().image.sprite = _weaponIcons[i];
            b.GetComponent<LoadoutWeaponButton>().SetupButton();
        }
    }

    public void UpdateLoadoutStats(List<int> stats, string wepName)
    {
        _weaponTextDisplay.text = wepName;
        for (int i = 0; i < stats.Count; i++)
        {
            _statSliderList[i].value = stats[i];
            _statValueList[i].text = stats[i].ToString();
        }

    }
}