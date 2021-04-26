using System.Collections;
using UnityEngine;
using Steamworks;

public class AchievementSystem : MonoBehaviour
{
    private static AchievementSystem _achievementSystem;
    public static AchievementSystem Instance
    {
        get
        {
            if (_achievementSystem == null)
            {
                if (FindObjectOfType<AchievementSystem>() != null)
                {
                    _achievementSystem = FindObjectOfType<AchievementSystem>();
                    return _achievementSystem;
                }
                else
                {
                    Debug.LogError("Something in the scene is looking for the Achievement System but there isn't one to be found");
                    return null;
                }
            }
            else
            {
                return _achievementSystem;
            }
        }
    }
    //our game ID  
    private CGameID m_GameID;
    //did we get stats from Steam?  
    private bool m_bRequestedStats;
    private bool m_bStatsValid;
    //should we store stats?  
    private bool m_bStoreStats;
    protected Callback<UserStatsReceived_t> m_UserStatsReceived;
    protected Callback<UserStatsStored_t> m_UserStatsStored;
    protected Callback<UserAchievementStored_t> m_UserAchievementStored;
    void OnEnable()
    {
        if (_achievementSystem == null)
        {
            _achievementSystem = this;
        }
        if (!SteamManager.Initialized)
        {
            return;
        }
        //get our game ID for callbacks  
        m_GameID = new CGameID(SteamUtils.GetAppID());
        m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
        m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
        m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);
        //these need to be reset to get stats upon Assembly reload in editor  
        m_bRequestedStats = false;
        m_bStatsValid = false;
        //request stats  
        StartCoroutine("GetStats");
    }
    IEnumerator GetStats()
    {
        while (!SteamManager.Initialized)
        {
            yield return new WaitForEndOfFrame();
        }
        bool bSuccess = false;
        if (!m_bRequestedStats)
        {
            m_bRequestedStats = true;
            //request stats  
            bSuccess = SteamUserStats.RequestCurrentStats();
            m_bRequestedStats = bSuccess;
        }
    }
    void StoreStats()
    {
        SteamUserStats.StoreStats();
    }
    public void UnlockAchievement(string achievementID)
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.SetAchievement(achievementID);
            StoreStats();
        }
    }
    public void UpdateStat(string statID, int newValue)
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.SetStat(statID, newValue);
            StoreStats();
        }
    }
    private void OnUserStatsReceived(UserStatsReceived_t pCallback)
    {
        {
            if (!SteamManager.Initialized)
                return;
            // we may get callbacks for other games' stats arriving, ignore them  
            if ((ulong)m_GameID == pCallback.m_nGameID)
            {
                if (EResult.k_EResultOK == pCallback.m_eResult)
                {
                    // Debug.Log("Received stats and achievements from Steam\n");  
                    m_bStatsValid = true;
                }
                else
                {
                    Debug.Log("RequestStats - failed, " + pCallback.m_eResult);
                }
            }
        }
    }
    private void OnUserStatsStored(UserStatsStored_t pCallback)
    {
        // we may get callbacks for other games' stats arriving, ignore them  
        if ((ulong)m_GameID == pCallback.m_nGameID)
        {
            if (EResult.k_EResultOK == pCallback.m_eResult)
            {
                //Debug.Log("StoreStats - success");  
            }
            else if (EResult.k_EResultInvalidParam == pCallback.m_eResult)
            {
                // One or more stats we set broke a constraint. They've been reverted,  
                // and we should re-iterate the values now to keep in sync.  
                Debug.Log("StoreStats - some failed to validate");
                // Fake up a callback here so that we re-load the values.  
                UserStatsReceived_t callback = new UserStatsReceived_t();
                callback.m_eResult = EResult.k_EResultOK;
                callback.m_nGameID = (ulong)m_GameID;
                OnUserStatsReceived(callback);
            }
            else
            {
                Debug.Log("StoreStats - failed, " + pCallback.m_eResult);
            }
        }
    }
    private void OnAchievementStored(UserAchievementStored_t pCallback)
    {
        // We may get callbacks for other games' stats arriving, ignore them  
        if ((ulong)m_GameID == pCallback.m_nGameID)
        {
            if (0 == pCallback.m_nMaxProgress)
            {
                // Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");  
            }
            else
            {
                Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
            }
        }
    }
}