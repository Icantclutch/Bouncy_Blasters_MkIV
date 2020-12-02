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
    [SerializeField]
    private float _respawnDelay = 10;

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
        GetComponent<Shooting>().Rpc_FullReload();
        //Teleport the player
        Rpc_TeleportPlayer();
    }

    [ClientRpc]
    private void Rpc_DeathSounds()
    {
        GetComponent<AudioSource>().PlayOneShot(_deathClip, .25f);
    }

    [TargetRpc]
    private void Rpc_TeleportPlayer()
    {
        //TODO Call to game controller to teleport player to designated spawn point
        Debug.Log("Player has died, Teleporting to respawn room");
        if (PlayerSpawnSystem.SpawnPlayer(gameObject, false))
        {
            GetComponent<Shooting>().active = false;
            StartCoroutine(RespawnPlayer());
        }
        else
        {
            PlayerSpawnSystem.SpawnPlayer(gameObject);
        }
    }
    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(_respawnDelay);
        PlayerSpawnSystem.SpawnPlayer(gameObject);
        GetComponent<Shooting>().active = true;
    }

    [Server]
    public override void Hit(Bullet.Shot shot)
    {
        //print("HIT " + shot);
        //Get the source object
        int shotTeam = NetworkIdentity.spawned[Convert.ToUInt32(shot.playerID)].GetComponent<HitInteraction>().GetTeam();
        int myTeam = this.GetTeam();

        //Check if the source is not on your team
        if (CheckTeamConflict(shotTeam, myTeam))// && Convert.ToUInt32(shot.playerID) != GetComponent<NetworkIdentity>().netId)
        {
            //Deal damage
            currentCharge += shot.damage[shot.numBounces];

            //Play audio clips for hitting a shot and getting hit
            NetworkIdentity.spawned[Convert.ToUInt32(shot.playerID)].GetComponent<PlayerAudioController>().RpcOnPlayerClient(0);
            GetComponent<PlayerAudioController>().RpcOnPlayerClient(1);

            if (currentCharge >= maxSuitCharge)
            {
                //Prevent adding score to team on self kill
                if (Convert.ToUInt32(shot.playerID) != GetComponent<NetworkIdentity>().netId)
                {
                    NetworkIdentity.spawned[Convert.ToUInt32(shot.playerID)].GetComponent<PlayerData>().AddPlayerElim();
                }
               
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
