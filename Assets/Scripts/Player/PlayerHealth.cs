using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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

    //The team that the player is on
    [SerializeField]
    [SyncVar]
    private int playerTeamNumber;

    //reference to ther scrips
    private PlayerReference myReference;

    // Start is called before the first frame update
    void Start()
    {
        myReference = GetComponent<PlayerReference>();
        currentCharge = 0;
    }

    [Server]
    void Update()
    {
        //charge check to see if the player has reached the damage threshold for being teleported.
        if (currentCharge >= maxSuitCharge)
        {
            TeleportPlayer();
        }
        
    }

    [Server]
    private void TeleportPlayer()
    {
        currentCharge = 0;
        //TODO Call to game controller to teleport player to designated spawn point
        Debug.Log("Player has died, Teleporting to respawn room (not implemented)");
        PlayerSpawnSystem.SpawnPlayer(gameObject);
    }

    [Server]
    public void AssignTeam(int teamNum) 
    {
        playerTeamNumber = teamNum;
    }

    public int GetTeam()
    {
        return playerTeamNumber;
    }
    public override void Hit(Bullet.Shot shot)
    {
        //Deal damage
        currentCharge += shot.damage[shot.numBounces];
        //Debug Message
        Rpc_DebugOutput(shot.damage[shot.numBounces]);
    }

    //Debug to be removed
    [ClientRpc]
    public void Rpc_DebugOutput(int damage)
    {
        Debug.Log("Player was dealt damage: " + damage + ", and now has health of: " + currentCharge);
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
