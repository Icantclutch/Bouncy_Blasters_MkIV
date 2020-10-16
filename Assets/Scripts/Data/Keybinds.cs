using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Keybinds
{
    //Weapon keys
    public static KeyCode PrimaryFire = KeyCode.Mouse0; //Left click primary fire
    public static KeyCode Zoom = KeyCode.Mouse1; //Rigth click zoom 
    public static KeyCode SecondaryFire = KeyCode.Mouse2; //Middle click secondary fire
    public static KeyCode GrenadeFire = KeyCode.G; //G grenade fire
    public static KeyCode Reload = KeyCode.R; //R Reload weapon

    //Movement
    public static KeyCode Jump = KeyCode.Space; //Space jump
    public static KeyCode Crouch = KeyCode.LeftControl; //Left control crouch
    public static string Horizontal = "Horizontal"; //Left/Right movement axis
    public static string Vertical = "Vertical"; //Forward/Bakc movement axis
    public static string MouseX = "MouseX"; //Mouse Horizontal
    public static string MouseY = "MouseY"; //Mouse Vertical
}
