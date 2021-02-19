using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public class MouseLook2 : NetworkBehaviour
{
    [SerializeField] private GameObject UI = null;

    public Transform eyes;
    public float sens = 5;
    private float temp = 5;
    private Quaternion charTargetRot;
    private Quaternion camTargetRot;

    private bool pause = false;

    [SerializeField]
    private float doubleClickTimer = 0.25f;
    [SerializeField]
    private bool beginTimer;



    // Start is called before the first frame update
    void Start()
    {
        doubleClickTimer = 0.25f;
        charTargetRot = gameObject.transform.localRotation;
        camTargetRot = eyes.localRotation;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        temp = sens;
        /*if (hasAuthority) {
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
        }*/
    }

    public void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //eyes.gameObject.SetActive(false);
        for (int i = 0; i < eyes.childCount; i++)
        {
            eyes.GetChild(i).gameObject.SetActive(false);
        }
        UI.SetActive(false);
    }
    public void OnEnable()
    {
        if (hasAuthority)
        {
            //eyes.gameObject.SetActive(true);
            for (int i = 0; i < eyes.childCount; i++)
            {
                eyes.GetChild(i).gameObject.SetActive(true);
            }
            UI.SetActive(true);
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

            //Debug.Log(yRot);
            //Debug.Log(xRot);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            pause = !pause;
            Cursor.visible = pause;
            
        }

        //Functionality for allowing the player to double press zoom to snap the reticle to the horizon
        ResetToHorizon();
        
    }

    public void changeSensitivity(float sensitivity)
    {
        sens = temp;
        sens+=sensitivity;
    }

    private void ResetToHorizon()
    {
        //Begins the timer window for pressing the zoom button a second time
        if (beginTimer)
        {
            doubleClickTimer -= Time.deltaTime;
        }
        //Reseting the timer if the window expires
        if(doubleClickTimer <= 0)
        {
            beginTimer = false;
            doubleClickTimer = 0.25f;
            
        }
        //Starting the timer for a second zoom press
        if (Input.GetKeyDown(Keybinds.Zoom) && doubleClickTimer == 0.25f)
        {
            beginTimer = true;
        }
        //Handling a second zoom press within the alloted time window
        else if (Input.GetKeyDown(Keybinds.Zoom) && doubleClickTimer < 0.25f)
        {
            //Change camera to look at horizon
            charTargetRot *= Quaternion.Euler(0f, 0f, 0f);
            camTargetRot = Quaternion.Euler(0f, 0f, 0f);

            gameObject.transform.localRotation = charTargetRot;
            eyes.transform.localRotation = camTargetRot;

        }

    }
}
