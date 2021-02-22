using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;

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
    private AudioClip _deathClip = null;
    [SerializeField]
    private float _respawnDelay = 10;

    [SerializeField]
    private GameObject deathEffect;

    //reference to ther scrips
    private PlayerReference myReference;

    //Variables for shield visibility
    [SerializeField]
    private MeshRenderer _shield;
    [SerializeField]
    private float _shieldVisibilityTime = 2f;
    private float _timer = 0;

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
        Rpc_DeathSounds();
        GetComponent<Shooting>().RPC_GetNewLoadout();
        GetComponent<Shooting>().Rpc_FullReload();
        //Teleport the player
        Rpc_TeleportPlayer();
        currentCharge = 0;
    }

    [ClientRpc]
    private void Rpc_DeathSounds()
    {
        //print("CREATING EFFECT " + transform.position);
        //if (deathEffect != null)
        //{
        //    GameObject b = Instantiate(deathEffect, transform.position, Quaternion.identity);
            //Spawn on server
        //    NetworkServer.Spawn(b);
        //}
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
            GetComponent<PlayerMovement>().active = true;
            GetComponent<PlayerMovement>().inRespawnRoom = true;
            GetComponent<PlayerMovement>().DisableSprint();
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
        GetComponent<PlayerMovement>().inRespawnRoom = false;
    }

    [Server]
    public override void Hit(Bullet.Shot shot)
    {
        //Get the source object
        int shotTeam = NetworkIdentity.spawned[Convert.ToUInt32(shot.playerID)].GetComponent<HitInteraction>().GetTeam();
        int myTeam = this.GetTeam();

        //Check if the source is not on your team
        if (CheckTeamConflict(shotTeam, myTeam))
        {
            Debug.Log("No Conflict");
        }

        //Deal damage
        currentCharge += shot.damage[shot.numBounces];

        //Play audio clips for hitting a shot and getting hit
        NetworkIdentity.spawned[Convert.ToUInt32(shot.playerID)].GetComponent<PlayerAudioController>().RpcOnPlayerClient(0);
        NetworkIdentity.spawned[Convert.ToUInt32(shot.playerID)].GetComponent<PlayerEffects>().CreateHitmarker();
        GetComponent<PlayerAudioController>().RpcOnPlayerClient(1);

        //NetworkIdentity.spawned[Convert.ToUInt32(shot.playerID)].GetComponent<PlayerEffects>().CreateKillFeed

        Rpc_ShowShield();

        if (currentCharge >= maxSuitCharge)
        {
            //Prevent adding score to team on self kill
            if (Convert.ToUInt32(shot.playerID) != GetComponent<NetworkIdentity>().netId)
            {
                NetworkIdentity.spawned[Convert.ToUInt32(shot.playerID)].GetComponent<PlayerData>().AddPlayerElim();
            }
            //Create a kill feed for everyone
            GetComponent<PlayerEffects>().CreateKillFeed(NetworkIdentity.spawned[Convert.ToUInt32(shot.playerID)].GetComponent<PlayerData>().playerName, GetComponent<PlayerData>().playerName);
            //Show a death message to player
            GetComponent<PlayerEffects>().ShowDeathDisplay();
            GetComponent<PlayerData>().AddPlayerDeaths();
        }
    }

    //Enable the shields mesh for all clients
    [ClientRpc]
    public void Rpc_ShowShield()
    {
        //GetComponent<MeshRenderer>().enabled = true;
        //_timer = _shieldVisibilityTime;
        StartCoroutine(ShowShield());
    }

    IEnumerator ShowShield()
    {
        _shield.enabled = true;
        yield return new WaitForSeconds(_shieldVisibilityTime);
        _shield.enabled = false;
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

    public void SetMaxCharge(int hp)
    {
        maxSuitCharge = hp;
    }

    public void SetRespawnDelay(float value)
    {
        _respawnDelay = value;
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

    
    private void ChangeLoadout()
    {
        myReference.playerShooting.RPC_GetNewLoadout();
    }
}
