using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Security.Cryptography;
using System;
using Steamworks;

public class Shooting : NetworkBehaviour
{
    [System.Serializable]
    public class WeaponSlot {
        //The weapon in this slot
        public Weapon weapon;
        //The current fire mode, used only if the weapon has a mode-swap key
        public int currentFiringMode = 0;
        //the current amount of ammo the weapon has
        public int currentAmmo = 0;
        //the current amount of ammo the weapon has in reserve
        public int currentReserve = 0;
        //the current cooldown on firing
        public float currentCooldown = 0;
    }

    //Where the player's eyes are
    [SyncVar]
    public Transform eyes;
    //The current weapon the player is using
    [SyncVar]
    public int currentWeapon = 0;
    //List of weapons the player has
    [SyncVar]
    public List<WeaponSlot> playerWeapons = new List<WeaponSlot>();
    //Bool used to make sure firing doesn't occur at the same time
    [SyncVar]
    public bool currentlyFiring = false;

    public bool active = false;
    //Player reference
    private PlayerReference myReference;

    [SyncVar]
    private Weapon.FireMode currentFireMode;


    private float _rechargeHoldTime = 1.5f;

    private void Start()
    {
        myReference = GetComponent<PlayerReference>();

        if (!hasAuthority)
            return;

        //Set ammo to max
        for(int i = 0; i < playerWeapons.Count; i++)
        {
            playerWeapons[i].currentAmmo = playerWeapons[i].weapon.ammoCount;
            playerWeapons[i].currentReserve = playerWeapons[i].weapon.reserveAmmo;
        }
    }

    // Update is called once per frame
    [Client]
    void Update()
    {
        if (!hasAuthority || !active)
            return;
        //currentProjectile = activeFireMode.bulletPrefab;

        //Switch held weapon
        if (Input.GetKeyDown(Keybinds.SwapWeapon))
        {
            Cmd_SwapWeapon();
        }

        //Loop through any existing weapons to tick down cooldowns
        foreach(WeaponSlot w in playerWeapons)
        {
            if(w.currentCooldown > 0)
                w.currentCooldown -= Time.deltaTime;

            if (w.currentCooldown < 0)
                w.currentCooldown = 0;
        }

        //Only do weapon stuff if the player actually has a weapon and isn't firing
        if (playerWeapons.Count > 0 && !currentlyFiring)
        {
            //If the weapon has a set key for swapping modes
            if(playerWeapons[currentWeapon].weapon.modeSwapKey != Weapon.FireKey.None)
            {
                SwapWeaponFire();
            } else
            {
                NormalWeaponFire();
            }

            //Reload if button is pressed and there is any reserve ammo
            if (Input.GetKeyDown(Keybinds.Reload) && playerWeapons[currentWeapon].currentReserve > 0)
            {
                StartCoroutine(Reload());

            }
            if (Input.GetKey(Keybinds.Reload))
            {
                _rechargeHoldTime -= Time.deltaTime;
                if(_rechargeHoldTime <= 0)
                {
                    StartCoroutine(Recharge());
                }
            }
            if (Input.GetKeyUp(Keybinds.Reload))
            {
                _rechargeHoldTime = 1.5f;
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
        currentFireMode = playerWeapons[currentWeapon].weapon.fireModes[playerWeapons[currentWeapon].currentFiringMode];
        if (playerWeapons[currentWeapon].currentCooldown <= 0 && playerWeapons[currentWeapon].currentAmmo >= currentFireMode.ammoUsedEachShot)
        {
            if (currentFireMode.fireType == Weapon.FireType.Automatic)
            {
                if (GetButtonHeld(currentFireMode.key))
                {
                    StartCoroutine(FireBullet(currentWeapon));
                    return;
                }
            }
            else
            {
                if (GetButtonFired(currentFireMode.key))
                {
                    StartCoroutine(FireBullet(currentWeapon));
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
            currentFireMode = current;
            if (playerWeapons[currentWeapon].currentCooldown <= 0 && playerWeapons[currentWeapon].currentAmmo >= currentFireMode.ammoUsedEachShot)
            {
                if (currentFireMode.fireType == Weapon.FireType.Automatic)
                {
                    if (GetButtonHeld(currentFireMode.key))
                    {
                        StartCoroutine(FireBullet(currentWeapon));
                        return;
                    }
                }
                else
                {
                    if (GetButtonFired(currentFireMode.key))
                    {
                        StartCoroutine(FireBullet(currentWeapon));
                        return;
                    }
                }
            }
        }
    }

    //Fully Reload and recharge for when the player respawns
    [ClientRpc]
    public void Rpc_FullReload()
    {
        for (int i = 0; i < playerWeapons.Count; i++)
        {
            playerWeapons[i].currentAmmo = playerWeapons[i].weapon.ammoCount;
            playerWeapons[i].currentReserve = playerWeapons[i].weapon.reserveAmmo;
        }
    }

    //Reload function
    IEnumerator Reload()
    {
        //Set firing so you can't shoot while reloading
        currentlyFiring = true;

        //Improve once animations are implemented
        //For loop repeating a number of times equal to the missing ammo.
        for(int i = playerWeapons[currentWeapon].currentAmmo; i < playerWeapons[currentWeapon].weapon.ammoCount; i++)
        {
            //If any reserve ammo is left, reload by one
            if(playerWeapons[currentWeapon].currentReserve > 0)
            {
                playerWeapons[currentWeapon].currentAmmo++;
                playerWeapons[currentWeapon].currentReserve--;
            } else //Break if no ammo left
            {
                break;
            }
        }

        //Disable firing when reloading is done
        currentlyFiring = false;
        yield return null;
    }

    //Recharge function
    IEnumerator Recharge()
    {
        //Set firing so you can't shoot while recharging
        currentlyFiring = true;

        //Improve once animations are implemented
        //While loop to recharge ammo to max reserves
        if (playerWeapons[currentWeapon].currentReserve < playerWeapons[currentWeapon].weapon.reserveAmmo)
        {
            playerWeapons[currentWeapon].currentReserve++;

        }

        //Disable firing when reloading is done
        currentlyFiring = false;
        yield return null;
    }

    IEnumerator FireBullet(int weaponSlot)
    {
        //We are currently firing
        currentlyFiring = true;
        //Fire each shot
        for (int i = 0; i < currentFireMode.shotsFiredAtOnce; i++)
        {
            //Play audio of firing
            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);

            //Subtract from the ammo
            playerWeapons[currentWeapon].currentAmmo -= currentFireMode.ammoUsedEachShot;
            //Fire bullet over server
            Cmd_ServerFireBullet(currentFireMode.bulletPrefabName, currentFireMode.bulletDamage, currentFireMode.maxBounces, currentFireMode.fireSpeed);
            //Wait
            yield return new WaitForSeconds(60 / currentFireMode.fireRate);
        }
        //We are no longer firing
        currentlyFiring = false;
    }

    //Server reference for firing bullets
    [Command]
    void Cmd_ServerFireBullet(string bullet, List<int> damage, int bounces, float fireSpeed)
    {
        //Fetch Bullet Prefab from Network Manager
        GameObject bulletPrefab = NetworkManager.singleton.spawnPrefabs.Find(bu => bu.name.Equals(bullet));
        //Summon the bullet
        GameObject b = Instantiate(bulletPrefab, eyes.transform.position, eyes.transform.rotation);
        //Spawn on server
        NetworkServer.Spawn(b);
        //Get the player's id
        int playerID = Convert.ToInt32(GetComponent<NetworkIdentity>().netId);
        //Assign it its properties
        b.GetComponent<Bullet>().Initialize(damage, bounces, fireSpeed, playerID);
        //Play the firing audio
        //GetComponent<AudioSource>().PlayOneShot(fireMode.firingSound, .5f);
    }

    [Command]
    void Cmd_SwapWeapon()
    {
        currentWeapon++;
        //loop around if at the end
        if (currentWeapon >= playerWeapons.Count)
            currentWeapon = 0;
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
