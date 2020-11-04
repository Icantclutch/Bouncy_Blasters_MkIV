﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MouseLook2 : NetworkBehaviour
{
    [SerializeField] private GameObject UI = null;

    public Transform eyes;
    public int sens = 1;
    private Quaternion charTargetRot;
    private Quaternion camTargetRot;

    private bool pause = false;



    // Start is called before the first frame update
    void Start()
    {
        charTargetRot = gameObject.transform.localRotation;
        camTargetRot = eyes.localRotation;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (hasAuthority) {
            //eyes.gameObject.SetActive(true);
            for (int i = 0; i < eyes.childCount; i++)
            {
                eyes.GetChild(i).gameObject.SetActive(true);
            }
            UI.SetActive(true);
        }
        else {
            //eyes.gameObject.SetActive(false);
            for (int i = 0; i < eyes.childCount; i++)
            {
                eyes.GetChild(i).gameObject.SetActive(false);
            }
            UI.SetActive(false);
        }
    }

    // Update is called once per frame
    [Client]
    void Update()
    {
        if (!hasAuthority)
            return;

        if (!pause)
        {
            Cursor.lockState = CursorLockMode.Locked;
            float yRot = Input.GetAxis(Keybinds.MouseY) * sens;
            float xRot = Input.GetAxis(Keybinds.MouseX) * sens;
            charTargetRot *= Quaternion.Euler(0f, xRot, 0f);
            camTargetRot *= Quaternion.Euler(-yRot, 0f, 0f);
            
            camTargetRot.x = Mathf.Clamp(camTargetRot.x, -.7f, .7f);
            camTargetRot.w = Mathf.Clamp(camTargetRot.w, .6f, .7f);
            gameObject.transform.localRotation = charTargetRot;
            eyes.transform.localRotation = camTargetRot;


        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = pause;
            
        }

        //gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, charTargetRot, Time.deltaTime);
        //cam.localRotation = Quaternion.Slerp(cam.localRotation, camTargetRot, Time.deltaTime);
    }
}
