using UnityEngine;
using static Steamworks.InventoryItem;

public class SteamManager
{
    public uint appId = 3876840;

    // TODO Assign once we have a DLC setup
    public uint cosmeticsDLCAppId = 3892740;

    private Steamworks.Data.Leaderboard highestSurivalWaveLeaderboard;

    private const float statPersistenceCooldown = 30.0f;
    private float currentStatPersistenceCooldown = statPersistenceCooldown;

    public SteamManager()
    {
        try
        {
            Steamworks.SteamClient.Init(appId, false);
            Debug.Log("Steamworks initialized succesfully");
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
    }

    public bool HasCosmeticsDLC()
    {
        return Steamworks.SteamApps.IsDlcInstalled(cosmeticsDLCAppId);
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

        var steamworks_val = Steamworks.SteamUserStats.GetStatInt("NumKilledEnemies");
        Debug.Log($"Steamworks Value {steamworks_val}");
        var pd_val = PD.Instance.NumKilledEnemies.Get();
        Debug.Log($"PD Value {pd_val}");

        if (!Steamworks.SteamUserStats.StoreStats())
        {
            Debug.LogError("Unable to store stats to steam");
        }
    }
}
