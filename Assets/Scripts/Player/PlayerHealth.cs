using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerHealth : HitInteraction
{
    //The max charge of the player. This variable should not change in execution
    [SerializeField]
    [SyncVar]
    private int maxSuitCharge = 100;

    //The current charge of the player. This variable will change based on the players charge
    [SerializeField]
    [SyncVar]
    private int currentCharge;

    [SerializeField]
    private AudioClip _deathClip;

    //reference to ther scrips
    private PlayerReference myReference;

    // Start is called before the first frame update
    void Start()
    {
        myReference = GetComponent<PlayerReference>();
        SetCharge(0);
        AssignTeam(0);
    }

    [Server]
    void Update()
    {
        //charge check to see if the player has reached the damage threshold for being teleported.
        if (currentCharge >= maxSuitCharge)
        {
            Respawn();
        }
        
    }

    [Server]
    //Holds all the servserside calls for respawning the player, 
    private void Respawn()
    {
        currentCharge = 0;
        Rpc_DeathSounds();
        //Teleport the player
        Rpc_TeleportPlayer();
    }

    [ClientRpc]
    private void Rpc_DeathSounds()
    {
        GetComponent<AudioSource>().PlayOneShot(_deathClip, .5f);
        GetComponent<Shooting>().Rpc_FullReload();
    }

    [TargetRpc]
    private void Rpc_TeleportPlayer()
    {
        //TODO Call to game controller to teleport player to designated spawn point
        Debug.Log("Player has died, Teleporting to respawn room (not implemented)");
        PlayerSpawnSystem.SpawnPlayer(gameObject);
    }

    [Server]
    public override void Hit(Bullet.Shot shot)
    {
        //Get the source object
        int shotTeam = NetworkIdentity.spawned[Convert.ToUInt32(shot.playerID)].GetComponent<HitInteraction>().GetTeam();
        int myTeam = this.GetTeam();

        //Check if the source is not on your team
        if (CheckTeamConflict(shotTeam, myTeam))// && Convert.ToUInt32(shot.playerID) != GetComponent<NetworkIdentity>().netId)
        {
            //Deal damage
            currentCharge += shot.damage[shot.numBounces];
            if(currentCharge >= maxSuitCharge)
            {
                NetworkIdentity.spawned[Convert.ToUInt32(shot.playerID)].GetComponent<PlayerData>().AddPlayerElim();
                GetComponent<PlayerData>().AddPlayerDeaths();
            }
        }

    }

    //Sets the players charge to a certain value
    [Server]
    public void SetCharge(int value)
    {
        currentCharge = value;
    }

    public int GetCharge()
    {
        return currentCharge;
    }

    //Add and remove player from the SpawnSystem
    private void Awake()
    {
        PlayerSpawnSystem.AddPlayer(gameObject);
    }
    private void OnDestroy()
    {
        PlayerSpawnSystem.RemovePlayer(gameObject);
    }
}
