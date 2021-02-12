using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempWallsScripts : MonoBehaviour
{
    [SerializeField]
    Transform[] walls;
    bool state;
    // Start is called before the first frame update
    void Start()
    {
        state = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            state = !state;
            foreach(Transform w in walls)
            {
                w.gameObject.SetActive(state);
                Debug.Log("Walls change: " + state);
            }
        }
    }


}
