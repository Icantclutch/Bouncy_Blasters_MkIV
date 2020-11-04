using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class HitInteraction : NetworkBehaviour
{
    public abstract void Hit(Bullet.Shot shot);

    //The team that the player is on
    //Team 0 is free-for-all, all others can't hit each members of their team
    [SerializeField]
    [SyncVar]
    private int teamNumber;

    [Server]
    public void AssignTeam(int teamNum)
    {
        teamNumber = teamNum;
    }

    //Returns the player's team number
    public int GetTeam()
    {
        return teamNumber;
    }

    //Checks to see if a team 1 can hit team 2
    public static bool CheckTeamConflict(int team1, int team2)
    {
        //If they're from the same team
        if(team1 == team2)
        {
            //Both are team 0, so they can hit one another
            if(team2 == 0)
            {
                return true;
            } else //Both are same team, can't hit each other
            {
                return false;
            }
        } else //From opposing teams
        {
            return true;
        }
    }
}
