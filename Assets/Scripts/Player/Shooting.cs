using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Security.Cryptography;
using System;

public class Shooting : NetworkBehaviour
{
    [System.Serializable]
    public class WeaponSlot {
        //The weapon in this slot
        public Weapon weapon;
        //The current fire mode, used only if the weapon has a mode-swap key
        public int currentFiringMode = 0;
        //the current amount of ammo the weapon has
        public int currentAmmo  = 0;
        //the current cooldown on firing
        public float currentCooldown = 0;
    }

    //Where the player's eyes are
    public Transform eyes;
    //The current weapon the player is using
    public int currentWeapon = 0;
    //List of weapons the player has
    public List<WeaponSlot> playerWeapons = new List<WeaponSlot>();
    //Bool used to make sure firing doesn't occur at the same time
    public bool currentlyFiring = false;

    [SyncVar(hook = nameof(ActiveFireModeUpdated))]
    private Weapon.FireMode activeFireMode;

    private void ActiveFireModeUpdated(Weapon.FireMode oldFireMode, Weapon.FireMode newFireMode)
    {
        activeFireMode = newFireMode;

    }

    // Update is called once per frame
    [Client]
    void Update()
    {
        if (!hasAuthority)
            return;
        //Loop through any existing weapons to tick down cooldowns
        foreach(WeaponSlot w in playerWeapons)
        {
            if(w.currentCooldown > 0)
                w.currentCooldown -= Time.deltaTime;
        }

        //Only do weapon stuff if the player actually has a weapon and isn't firing
        if (playerWeapons.Count > 0 && !currentlyFiring)
        {
            //Check to make sure the player is on a weapon that exists
            //If not, switch them to their first weapon
            if(currentWeapon >= playerWeapons.Count)
            {
                currentWeapon = 0;
            }

            //If the weapon has a set key for swapping modes
            if(playerWeapons[currentWeapon].weapon.modeSwapKey != Weapon.FireKey.None)
            {
                SwapWeaponFire();
            } else
            {
                NormalWeaponFire();
            }

            //Reload
            if (Input.GetKeyDown(Keybinds.Reload))
            {
                StartCoroutine(Reload());
            }
        }

        //GRENADE STUFF
    }

    //Firing script for weapons that swap modes
    void SwapWeaponFire()
    {
        //If the mode swap key is pressed, swap modes
        if (GetButtonFired(playerWeapons[currentWeapon].weapon.modeSwapKey))
        {
            //Increment firing mode
            playerWeapons[currentWeapon].currentFiringMode++;
            //If firing mode goes over the number of firing modes the gun has, loop back to 0
            if(playerWeapons[currentWeapon].currentFiringMode >= playerWeapons[currentWeapon].weapon.fireModes.Count)
            {
                playerWeapons[currentWeapon].currentFiringMode = 0;
            }
        }

        //Quick reference for the current fire mode
        Weapon.FireMode current = playerWeapons[currentWeapon].weapon.fireModes[playerWeapons[currentWeapon].currentFiringMode];
        if (playerWeapons[currentWeapon].currentCooldown <= 0 && playerWeapons[currentWeapon].currentAmmo >= current.ammoUsedEachShot)
        {
            if (current.fireType == Weapon.FireType.Automatic)
            {
                if (GetButtonHeld(current.key))
                {
                    StartCoroutine(FireBullet(currentWeapon, current));
                    return;
                }
            }
            else
            {
                if (GetButtonFired(current.key))
                {
                    StartCoroutine(FireBullet(currentWeapon, current));
                    return;
                }
            }
        }
    }

    //Firing script for weapons that work normally
    void NormalWeaponFire()
    {
        foreach(Weapon.FireMode current in playerWeapons[currentWeapon].weapon.fireModes)
        {
            //activeFireMode = current;
            if (playerWeapons[currentWeapon].currentCooldown <= 0 && playerWeapons[currentWeapon].currentAmmo >= current.ammoUsedEachShot)
            {
                if (current.fireType == Weapon.FireType.Automatic)
                {
                    if (GetButtonHeld(current.key))
                    {
                        StartCoroutine(FireBullet(currentWeapon, current));
                        return;
                    }
                }
                else
                {
                    if (GetButtonFired(current.key))
                    {
                        StartCoroutine(FireBullet(currentWeapon, current));
                        return;
                    }
                }
            }
        }
    }

    //Reload function
    IEnumerator Reload()
    {
        //Set firing so you can't shoot while reloading
        currentlyFiring = true;

        //Improve once animations are implemented
        playerWeapons[currentWeapon].currentAmmo = playerWeapons[currentWeapon].weapon.ammoCount;

        //Disable firing when reloading is done
        currentlyFiring = false;
        yield return null;
    }
    
    IEnumerator FireBullet(int weaponSlot, Weapon.FireMode fireMode)
    {
        //We are currently firing
        currentlyFiring = true;
        //Subtract from the ammo
        playerWeapons[weaponSlot].currentAmmo -= fireMode.ammoUsedEachShot;
        activeFireMode = fireMode;
        for (int i = 0; i < fireMode.shotsFiredAtOnce; i++)
        {
            //Summon the bullet
            //GameObject b = Instantiate(fireMode.bulletPrefab, eyes.transform.position, eyes.transform.rotation);
            //Assign it its properties
            //b.GetComponent<Bullet>().Initialize(fireMode);
            //GetComponent<AudioSource>().PlayOneShot(fireMode.firingSound, .5f);
            //NetworkServer.Spawn(b);
            //SpawnBullet(fireMode, eyes.transform);
            //CmdSpawnBullet(fireMode, eyes.transform);
            
            CmdSpawnBullet();
            //Play the firing audio
            
            //Wait
            yield return new WaitForSeconds(60 / fireMode.fireRate);
        }
        //We are no longer firing
        currentlyFiring = false;
    }
    

    [Command]
    void CmdSpawnBullet()
    {
        RpcSpawnBullet();
    }

    [ClientRpc]
    void RpcSpawnBullet()
    {
        //Weapon.FireMode fireMode = new Weapon.FireMode();
        GameObject b = Instantiate(activeFireMode.bulletPrefab, eyes.transform.position, eyes.transform.rotation);
        //Assign it its properties
        b.GetComponent<Bullet>().Initialize(activeFireMode);
        //NetworkServer.Spawn(b);
    }

    //Boolean that checks if a weapon has single-fired
    bool GetButtonFired(Weapon.FireKey key)
    {
        switch (key)
        {
            case Weapon.FireKey.PrimaryFire: //Primary fire key
                return Input.GetKeyDown(Keybinds.PrimaryFire);
            case Weapon.FireKey.SecondaryFire: //Secondary fire key
                return Input.GetKeyDown(Keybinds.SecondaryFire);
            case Weapon.FireKey.GrenadeFire: //Grenade fire key
                return Input.GetKeyDown(Keybinds.GrenadeFire);
            case Weapon.FireKey.None: //None isn't used
            default: //Default if a bad value is put in
                return false;
        }
    }

    //Boolean that checks if a weapon has hold-fired
    bool GetButtonHeld(Weapon.FireKey key)
    {
        switch (key)
        {
            case Weapon.FireKey.PrimaryFire: //Primary fire key
                return Input.GetKey(Keybinds.PrimaryFire);
            case Weapon.FireKey.SecondaryFire: //Secondary fire key
                return Input.GetKey(Keybinds.SecondaryFire);
            case Weapon.FireKey.GrenadeFire: //Grenade fire key
                return Input.GetKey(Keybinds.GrenadeFire);
            case Weapon.FireKey.None: //None isn't used
            default: //Default if a bad value is put in
                return false;
        }
    }
}
