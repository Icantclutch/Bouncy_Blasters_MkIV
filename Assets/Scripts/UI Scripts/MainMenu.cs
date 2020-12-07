using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Update()
    {
        /*GameObject networkManager = GameObject.Find("NetworkManager");
        if (networkManager)
        {
            Destroy(networkManager);
        }*/
        
    }
    public void Quit()
    {
        Application.Quit();
    }
}
