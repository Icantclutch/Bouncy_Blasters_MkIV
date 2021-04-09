using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking.Types;

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

    private bool _isDead = false;

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


    private bool _RunningDeath = false;
    private Vector3 _LookToPos = new Vector3(0, 0, 0);
    

    private LobbyManager _lobbyManager;

    public GameObject MainCamera;
    public GameObject SceneRespawnCam;


    //Camera that is Null if not dead
    private GameObject ToUseCam;


    // Start is called before the first frame update
    void Start()
    {
        myReference = GetComponent<PlayerReference>();
        SetCharge(0);
        _isDead = false;
    }

    public IEnumerator LookAtTransformCorutine(Vector3 goPosition, Vector3 lookPosition, float speed)
    {
        _RunningDeath = true;
        var currentPos = ToUseCam.transform.position;
        var t = 0f;
        while (t < 1)
        {
            if (ToUseCam != null)
            {
                t += Time.deltaTime / speed;
                Vector3 Holder1 = Vector3.Lerp(currentPos, goPosition, t);
                ToUseCam.transform.position = Holder1;

                Vector3 holder = Vector3.Lerp(ToUseCam.transform.position, lookPosition, t);
                ToUseCam.transform.LookAt(holder);
                yield return new WaitForEndOfFrame();
            }
        }
        _RunningDeath = false;
        //Changing DONE allows the camera to continue moving
    }

    [Server]
    void Update()
    {
        if (ToUseCam != null) {
            //ToUseCam.transform.Translate(0, Time.deltaTime * 1.5f, 0, Space.World);
            ToUseCam.GetComponent<Camera>().enabled = true;
            MainCamera.GetComponent<Camera>().enabled = false;
        } else {
            MainCamera.GetComponent<Camera>().enabled = true;
        }
    if (!_lobbyManager) {
        _lobbyManager = GameObject.FindGameObjectWithTag("Management").GetComponent<LobbyManager>();
     }
            if (GetComponent<PlayerData>())
            if (GetTeam() != GetComponent<PlayerData>().team)
                AssignTeam(GetComponent<PlayerData>().team);

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
        //Rpc_DeathSounds();
        GetComponent<PlayerAudioController>().RpcOnAllClients(7);
        GetComponent<Shooting>().Rpc_GetNewLoadout();
        GetComponent<Shooting>().Rpc_FullReload();
        //Teleport the player
        Rpc_TeleportPlayer();
        GetComponent<PlayerAudioController>().RpcOnAllClients(7);
        currentCharge = 0;
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
        Vector3 savedPos = transform.position; //+ new Vector3(0, 10, 0);
        if (PlayerSpawnSystem.SpawnPlayer(gameObject, false))
        {
            ToUseCam = Instantiate(SceneRespawnCam, savedPos, MainCamera.transform.rotation); //Quaternion.Euler(90, 0, 0)
            Destroy(ToUseCam, _respawnDelay);
            if (_RunningDeath == false)
            {
                StartCoroutine(LookAtTransformCorutine(savedPos + new Vector3(0, 5, 0), _LookToPos, _respawnDelay));
            }
            GetComponent<Shooting>().active = false;
            GetComponent<PlayerMovement>().active = true;
            GetComponent<PlayerMovement>().inRespawnRoom = true;
            GetComponent<PlayerMovement>().DisableSprint();
            StartCoroutine(RespawnPlayer());
        }
        else
        {
            Destroy(ToUseCam);
        }
    }
    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(_respawnDelay-2);
        CmdRespawnEffects();
        yield return new WaitForSeconds(2);
        PlayerSpawnSystem.SpawnPlayer(gameObject);
        GetComponent<Shooting>().active = true;
        GetComponent<PlayerMovement>().inRespawnRoom = false;
        _isDead = false;
        SetIsDead(_isDead);
    }

    [Command]
    private void CmdRespawnEffects()
    {
        GetComponent<PlayerAudioController>().RpcOnAllClients(8);
    }

    [Server]
    public override void Hit(Bullet.Shot shot)
    {
        if (!_isDead)
        {
            //Get the source object
            NetworkIdentity shooterIdentity = NetworkIdentity.spawned[Convert.ToUInt32(shot.playerID)];
            int shotTeam = shooterIdentity.GetComponent<HitInteraction>().GetTeam();
            int myTeam = this.GetTeam();

            //Check if the source is not on your team
            if (CheckTeamConflict(shotTeam, myTeam) || Convert.ToUInt32(shot.playerID) == GetComponent<NetworkIdentity>().netId)
            {
                //Deal damage
                int ShotDmg = shot.damage[shot.numBounces];
                currentCharge += ShotDmg;
                GetComponent<PlayerEffects>().DamageTakenText(shot.damage[shot.numBounces]);

                //Play audio clips for hitting a shot and getting hit
                shooterIdentity.GetComponent<PlayerAudioController>().RpcOnPlayerClient(0);
                shooterIdentity.GetComponent<PlayerEffects>().CreateHitmarker(ShotDmg);
                GetComponent<PlayerAudioController>().RpcOnPlayerClient(1);

                Rpc_ShowShield();

                if (currentCharge >= maxSuitCharge)
                {
                    _LookToPos = shooterIdentity.GetComponent<Transform>().position;
                    _isDead = true;
                    //Prevent adding score to team on self kill
                    if (Convert.ToUInt32(shot.playerID) != GetComponent<NetworkIdentity>().netId)
                    {
                        shooterIdentity.GetComponent<PlayerData>().AddPlayerElim();
                        //Create a kill feed for everyone
                        foreach (PlayerData Data in _lobbyManager.players)
                        {
                            Data.GetComponent<PlayerEffects>().CreateKillFeed(shooterIdentity.GetComponent<PlayerData>().playerName, GetComponent<PlayerData>().playerName);
                        }
                    }
                    //Displays a kill marker to the player who killed you
                    shooterIdentity.GetComponent<PlayerEffects>().CreateKillmarker();
                    //Show a death message to player
                    GetComponent<PlayerEffects>().ShowDeathDisplay();
                    GetComponent<PlayerData>().AddPlayerDeaths();
                }
            }
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

    [Command]
    private void SetIsDead(bool isDead)
    {
        _isDead = isDead;
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
        myReference.playerShooting.Rpc_GetNewLoadout();
    }
}
