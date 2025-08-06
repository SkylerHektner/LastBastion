using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using static Steamworks.InventoryItem;

public class SteamManager
{
    public uint appId = 3876840;

    public UnityEvent<Steamworks.Data.LeaderboardEntry[]> RefreshedLeaderboardEntriesEvent = new UnityEvent<Steamworks.Data.LeaderboardEntry[]>();

    // TODO Assign once we have a DLC setup
    public uint cosmeticsDLCAppId = 3892740;

    private Steamworks.Data.Leaderboard highestSurivalWaveLeaderboard;

    private const float statPersistenceCooldown = 30.0f;
    private float currentStatPersistenceCooldown = statPersistenceCooldown;

    private const float leaderboardRefreshCooldown = 30.0f;
    public float currentLeaderboardRefreshCooldown = 0.0f;
    private Task<Steamworks.Data.Leaderboard?> leaderboardRequest = null;
    private Steamworks.Data.Leaderboard? leaderboardData;
    private Task<Steamworks.Data.LeaderboardEntry[]> leaderboardEntriesRequest = null;

    public SteamManager()
    {
        try
        {
            Steamworks.SteamClient.Init(appId, false);
            Debug.Log("Steamworks initialized succesfully");
            leaderboardRequest = Steamworks.SteamUserStats.FindLeaderboardAsync("HighestSurvivalWaveLeaderboard");
        }
        catch (System.Exception e)
        {
            Debug.Log($"Failed to initialize steamworks {e}");
        }
    }

    public void Update(float deltaTime)
    {
        Steamworks.SteamClient.RunCallbacks();

        currentStatPersistenceCooldown -= deltaTime;
        if (currentStatPersistenceCooldown <= 0.0f)
        {
            currentStatPersistenceCooldown = statPersistenceCooldown;
            UpdateAndStoreStats();
        }

        // handle the initial request of the leaderboard
        if (leaderboardRequest != null)
        {
            if (leaderboardRequest.IsCompletedSuccessfully && leaderboardRequest.Result.HasValue)
            {
                Debug.Log("Succesfully fetched leaderboard for survival. Now fetching entries");
                leaderboardData = leaderboardRequest.Result;
                leaderboardRequest = null;
            }
            else if (leaderboardRequest.IsFaulted)
            {
                Debug.LogError("Unable to fetch survival leaderboard");
            }
        }

        // if we have a leaderboard and it's time to refresh its data, begin that process
        if (leaderboardData.HasValue && currentLeaderboardRefreshCooldown <= 0.0f)
        {
            currentLeaderboardRefreshCooldown = leaderboardRefreshCooldown;
            leaderboardEntriesRequest = leaderboardData.Value.GetScoresAsync(20);
            Debug.Log("Requesting leaderboard scores");
        }
        else
        {
            currentLeaderboardRefreshCooldown -= deltaTime;
        }

        // handle in progress leaderboard entries request
        if (leaderboardEntriesRequest != null)
        {
            if (leaderboardEntriesRequest.IsCompletedSuccessfully)
            {
                Debug.Log("Succesfully fetched entries for survival leaderboard.");
                RefreshedLeaderboardEntriesEvent.Invoke(leaderboardEntriesRequest.Result);

                // fetch a new set of data
                leaderboardEntriesRequest = null;
            }
            else if (leaderboardEntriesRequest.IsFaulted)
            {
                Debug.LogError("Unable to fetch survival leaderboard entries");
            }
        }
    }


    public bool HasCosmeticsDLC()
    {
        return Steamworks.SteamApps.IsDlcInstalled(cosmeticsDLCAppId);
    }

    public void TryUploadSurvivalScoreToLeaderboard()
    {
        if (leaderboardData.HasValue)
        {
            int score = PD.Instance.HighestSurvivalWave.Get();
            Debug.Log($"Submitting survival score {score} to leaderboard");
            leaderboardData.Value.SubmitScoreAsync(score);
        }    
    }

    // STATS
    private void UpdateAndStoreStats()
    {
        Steamworks.SteamUserStats.SetStat("NumKilledEnemies", PD.Instance.NumKilledEnemies.Get());
        Steamworks.SteamUserStats.SetStat("NumCrystalsUsed", PD.Instance.NumCrystalsUsed.Get());
        Steamworks.SteamUserStats.SetStat("NumTurretKills", PD.Instance.NumTurretKills.Get());
        Steamworks.SteamUserStats.SetStat("NumTimesSawOnFire", PD.Instance.NumTimesSawOnFire.Get());
        Steamworks.SteamUserStats.SetStat("NumEnemiesKilledByTyphoon", PD.Instance.NumEnemiesKilledByTyphoon.Get());
        Steamworks.SteamUserStats.SetStat("NumZappedEnemiesKilled", PD.Instance.NumZappedEnemiesKilled.Get());
        Steamworks.SteamUserStats.SetStat("HighestZappedEnemiesWithSingleChainLightning", PD.Instance.HighestZappedEnemiesWithSingleChainLightning.Get());
        Steamworks.SteamUserStats.SetStat("HighestAnomalySawUnleash", PD.Instance.HighestAnomalySawUnleash.Get());
        Steamworks.SteamUserStats.SetStat("TotalNumberOfAnomalySawUnleash", PD.Instance.TotalNumberOfAnomalySawUnleash.Get());
        Steamworks.SteamUserStats.SetStat("HighestEnemyDeathTollFromSawmageddonShot", PD.Instance.HighestEnemyDeathTollFromSawmageddonShot.Get());
        Steamworks.SteamUserStats.SetStat("HighestSurvivalWave", PD.Instance.HighestSurvivalWave.Get());
        Steamworks.SteamUserStats.SetStat("TotalWavesCompleted", PD.Instance.TotalWavesCompleted.Get());
        Steamworks.SteamUserStats.SetStat("TotalWealthEarned", PD.Instance.TotalWealthEarned.Get());
        Steamworks.SteamUserStats.SetStat("TotalFailures", PD.Instance.TotalFailures.Get());

        if (!Steamworks.SteamUserStats.StoreStats())
        {
            Debug.LogError("Unable to store stats to steam");
        }
    }
}
