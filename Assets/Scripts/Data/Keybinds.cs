using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Keybinds
{
    //Weapon keys
    public static KeyCode _primaryFire = KeyCode.Mouse0; //Left click primary fire
    public static KeyCode Zoom = KeyCode.Mouse1; //Rigth click zoom 
    public static KeyCode SecondaryFire = KeyCode.Mouse2; //Middle click secondary fire
    public static KeyCode GrenadeFire = KeyCode.G; //G grenade fire
    public static KeyCode Reload = KeyCode.R; //R Reload weapon
    public static KeyCode Recharge = KeyCode.F; //F Recharge ammo
    public static KeyCode SwapWeapon = KeyCode.Q; //Q Swap weapon

    //Movement
    public static KeyCode Jump = KeyCode.Space; //Space jump
    public static KeyCode Crouch = KeyCode.LeftControl; //Left control crouch
    public static KeyCode Sprint = KeyCode.LeftShift;
    public static string Horizontal = "Horizontal"; //Left/Right movement axis
    public static string Vertical = "Vertical"; //Forward/Back movement axis
    public static string MouseX = "Mouse X"; //Mouse Horizontal
    public static string MouseY = "Mouse Y"; //Mouse Vertical


    //ABXY controls
    public static string controlJump = "A Button";
    public static string controlRecharge = "B Button";
    public static string controlReload = "X Button";
    public static string controlSwap = "Y Button";
    public static string controlSprint = "LS Button";
    public static string controlReset = "RS Button";

    //start button to see menu
    public static string controlPause = "Start Button";

    //select to see map
    public static string controlMap = "Select Button";

    public static float triggerVal = Input.GetAxisRaw("Right Trigger");
    public static bool PrimaryFire(bool hold)
    {
        bool triggerFiring = false;
        float newVal = Input.GetAxisRaw("Right Trigger");

        if (Mathf.Round(Input.GetAxisRaw("Right Trigger")) < 0)
        {
            triggerFiring = true;
            //triggerVal = newVal;
        }
       

        Debug.Log(triggerVal);
        Debug.Log(newVal);
        Debug.Log(triggerFiring);
        return triggerFiring || Input.GetKeyDown(_primaryFire);
    }


    /*
   public static bool PrimaryFire(bool hold)
    {
        return Input.GetButtonDown("Right Trigger") || Input.GetKeyDown(_primaryFire);
    }


    /*
    public static bool Reload(bool press)
    {
        Debug.Log("reloading");
        return ((press) ? Input.GetKey(_reload) : Input.GetKeyDown(_reload) || Input.GetButton("Reload"));
    }

    */

}


