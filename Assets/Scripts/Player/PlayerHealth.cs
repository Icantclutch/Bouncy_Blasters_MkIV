using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    //The max charge of the player. This variable should not change in execution
    [SerializeField]
    private int maxSuitCharge = 100;

    //The current charge of the player. This variable will change based on the players charge
    [SerializeField]
    private int currentCharge;

    //The team that the player is on
    [SerializeField]
    private int playerTeamNumber;

    // Start is called before the first frame update
    void Start()
    {
        currentCharge = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //charge check to see if the player has reached the damage threshold for being teleported.
        if (currentCharge > maxSuitCharge)
        {
            TeleportPlayer();
        }
        
    }

    //Public 
    public void DealDamage(int damage)
    {
        currentCharge += damage;
        Debug.Log("Player was dealt damage: " + damage + ", and now has health of: " + currentCharge);
    }

    private void TeleportPlayer()
    {
        currentCharge = 0;
        //TODO Call to game controller to teleport player to designated spawn point
        Debug.Log("Player has died, Teleporting to respawn room (not implemented)");
    }

    public void AssignTeam(int teamNum) 
    {
        playerTeamNumber = teamNum;
    } 
}
