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
    public static KeyCode controlJump = KeyCode.JoystickButton0;
    public static KeyCode controlCrouch = KeyCode.JoystickButton1;
    public static KeyCode controlReload = KeyCode.JoystickButton2;
    public static KeyCode controlSwap = KeyCode.JoystickButton3;

    //start button to see menu
    public static KeyCode controlStart = KeyCode.JoystickButton7;

    public static bool PrimaryFire(bool hold)
    {
       
        Debug.Log(Input.GetAxis("Right Trigger"));
        return (((hold) ? Input.GetKey(_primaryFire) : Input.GetKeyDown(_primaryFire)) || //inline if statement
        (Input.GetAxisRaw("Right Trigger") < 0));
    }
    /*
       public static bool Zoom()
       {

       }

       */
}


