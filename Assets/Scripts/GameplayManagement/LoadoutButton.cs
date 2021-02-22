using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutButton : MonoBehaviour
{
    [SerializeField]
    private int loadout;

    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(getLoadout);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getLoadout()
    {
        gameObject.GetComponentInParent<Shooting>().Cmd_ChangeLoadout(loadout);
    }
}
