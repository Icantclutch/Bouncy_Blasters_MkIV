using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutMenuButtons : MonoBehaviour
{
    [SerializeField]
    private List<Weapon> weaponList;

    [SerializeField]
    private Button LoadoutPrefab;

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
        for(int i = 0; i < weaponList.Count; i++)
        {
            Button b = Instantiate(LoadoutPrefab, gameObject.transform);
            b.GetComponentInChildren<Text>().text = weaponList[i].name;
        }
    }
}
