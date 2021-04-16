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
        //max bounces
        public int maxBounces;


    }
    //The weapon that will replace the old weapon when switching loadouts
    [SyncVar]
    private int newWeapon;

    //Where the player's eyes are
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
    private PlayerMovement myMovement;

    [SyncVar]
    public Weapon.FireMode currentFireMode;

    [SerializeField]
    private float _rechargeHoldTime = 1.5f;

    [SerializeField]
    private GameObject _ReloadingFrame;

    private void Start()
    {
        myReference = GetComponent<PlayerReference>();
        myMovement = GetComponent<PlayerMovement>();
        newWeapon = -1;
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
            if (Input.GetKeyDown(Keybinds.Reload) && playerWeapons[currentWeapon].currentReserve > 0 &&
                playerWeapons[currentWeapon].currentAmmo != playerWeapons[currentWeapon].weapon.ammoCount)
            {
                StartCoroutine(Reload());
                
            }
            if (Input.GetKey(Keybinds.Recharge) && playerWeapons[currentWeapon].currentReserve < playerWeapons[currentWeapon].weapon.reserveAmmo && myMovement.grounded)
            {
                _rechargeHoldTime -= Time.deltaTime;
                
                if(_rechargeHoldTime <= 0 && !currentlyFiring)
                {
                    Debug.Log("Recharge");
                    StartCoroutine(Recharge());
                }
                else if(_rechargeHoldTime <= 0.5 && _rechargeHoldTime > 0)
                {
                    Debug.Log("Disabling Movement");
                    GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
                    myMovement.active = false;
                }
            } else
            {
                _rechargeHoldTime = 1.5f;
                //Debug.Log("Enabling Movement");
                myMovement.active = true;
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
            else if(playerWeapons[currentWeapon].currentCooldown <= 0 && playerWeapons[currentWeapon].currentAmmo == 0)
            {
                if(GetButtonFired(currentFireMode.key) || GetButtonHeld(currentFireMode.key))
                {
                    StartCoroutine(Reload());
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

    public void FullReload()
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
        _ReloadingFrame.SetActive(true);
        Cmd_ServerReload(currentFireMode.reloadSoundIndex);
        yield return new WaitForSeconds(2);

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
        _ReloadingFrame.SetActive(false);
        yield return null;
    }

    [Command]
    public void Cmd_ServerReload(int soundIndex)
    {
        GetComponent<PlayerAudioController>().RpcOnAllClients(soundIndex);
    }

    //Recharge function
    IEnumerator Recharge()
    {
        //Set firing so you can't shoot while recharging
        currentlyFiring = true;

        //Improve once animations are implemented
        //While loop to recharge ammo to max reserves
        while (playerWeapons[currentWeapon].currentReserve < playerWeapons[currentWeapon].weapon.reserveAmmo)
        {
            playerWeapons[currentWeapon].currentReserve++;
            yield return null;
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
            //GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
            
            //Subtract from the ammo
            playerWeapons[currentWeapon].currentAmmo -= currentFireMode.ammoUsedEachShot;
            //Fire bullet over server
            Cmd_ServerFireBullet(currentFireMode.bulletPrefabName, currentFireMode.bulletDamage, currentFireMode.maxBounces, currentFireMode.fireSpeed, currentFireMode.shotSoundIndex);
            //Wait
            yield return new WaitForSeconds(60 / currentFireMode.fireRate);
        }
        //We are no longer firing
        currentlyFiring = false;
    }

    //Server reference for firing bullets
    [Command]
    void Cmd_ServerFireBullet(string bullet, List<int> damage, int bounces, float fireSpeed, int soundIndex)
    {
        //Fetch Bullet Prefab from Network Manager
        GameObject bulletPrefab = NetworkManager.singleton.spawnPrefabs.Find(bu => bu.name.Equals(bullet));
        //Summon the bullet
        //Debug.Log(GetComponent<BlasterController>().currentBlaster);
        Transform barrel = GetComponentInChildren<BlasterController>().currentBlaster.transform.Find("Barrel");
        Ray ray = new Ray(eyes.transform.position, eyes.transform.forward);
        RaycastHit hit;
        Quaternion rotation;
        Vector3 nextReflection = Vector3.zero;
        if (Physics.Raycast(ray, out hit, 100)) {
            Vector3 direction = hit.point - barrel.position;
            rotation = Quaternion.LookRotation(direction);
            nextReflection = (hit.transform.CompareTag("NoBounce")) ? Vector3.zero : Vector3.Reflect(ray.direction, hit.normal);
        }
        else
        {
            Vector3 direction = (eyes.position + eyes.forward*100) - barrel.position;
            rotation = Quaternion.LookRotation(direction);
        }
        GameObject b = Instantiate(bulletPrefab, barrel.position, rotation);
        //Spawn on server
        NetworkServer.Spawn(b);
        //Get the player's id
        int playerID = Convert.ToInt32(GetComponent<NetworkIdentity>().netId);
        //Assign it its properties
        b.GetComponent<Bullet>().Initialize(damage, bounces, fireSpeed, playerID);
        if (b.GetComponent<TravelBullet>())
        {
            b.GetComponent<TravelBullet>().SetNextReflectionDirection(nextReflection);
        }
        //Play the firing audio
        //GetComponent<AudioSource>().PlayOneShot(fireMode.firingSound, .5f);
        GetComponent<PlayerAudioController>().RpcOnAllClients(soundIndex);

        Rpc_ShootingEffects();
    }

    [Command]
    void Cmd_SwapWeapon()
    {
        currentWeapon++;
        //loop around if at the end
        if (currentWeapon >= playerWeapons.Count)
            currentWeapon = 0;

        Rpc_UpdateWeaponModel(playerWeapons[currentWeapon].weapon.modelIndex);
    }
    [Command]
    void Cmd_UpdateWeaponModel(int index)
    {
        Rpc_UpdateWeaponModel(index);
    }
    [ClientRpc]
    void Rpc_UpdateWeaponModel(int index)
    {
        GetComponentInChildren<BlasterController>().SwapTo(index);
        if (index == 3)
        {
            GetComponent<PlayerAnimationController>().SetUsingPistol(true);
        }
        else
        {
            GetComponent<PlayerAnimationController>().SetUsingPistol(false);
        }
    }
    [ClientRpc]
    void Rpc_ShootingEffects()
    {
        GetComponentInChildren<BlasterController>().StartShootingEffect();
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

    //Function for selecting a new loadout
    [Command]
    public void Cmd_ChangeLoadout(int wep)
    {
        newWeapon = wep;
    }
    [ClientRpc]
    public void Rpc_GetNewLoadout()
    {
        if(newWeapon != -1)
        {
            playerWeapons[0].weapon = GameObject.FindGameObjectWithTag("Management").GetComponent<LoadoutManager>().loadouts[newWeapon];
            FullReload();
            int index = playerWeapons[currentWeapon].weapon.modelIndex;
            GetComponentInChildren<BlasterController>().SwapTo(index);
            if(index == 3)
            {
                GetComponent<PlayerAnimationController>().SetUsingPistol(true);
            }
            else
            {
                GetComponent<PlayerAnimationController>().SetUsingPistol(false);
            }
        }
        
    }
    public void GetNewLoadout()
    {
        if (newWeapon != -1)
        {
            playerWeapons[0].weapon = GameObject.FindGameObjectWithTag("Management").GetComponent<LoadoutManager>().loadouts[newWeapon];
            FullReload();
            Cmd_UpdateWeaponModel(playerWeapons[currentWeapon].weapon.modelIndex);
        }

    }
}
