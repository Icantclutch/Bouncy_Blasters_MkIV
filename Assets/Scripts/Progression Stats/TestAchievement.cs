using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAchievement : MonoBehaviour
{
    public void FinishedMatch(PlayerData data)//todo use the get stat instead of the actual player data
    {
        
        if(data.playerElims >= 1)
        {
            AchievementSystem.Instance.UnlockAchievement(Common.AchievementLookUp.FIRST_ELIM);
        }
    }
}
