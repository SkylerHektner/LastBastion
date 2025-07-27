using UnityEngine;
using static Steamworks.InventoryItem;

public class SteamManager
{
    public uint appId = 3876840;

    // TODO Assign once we have a DLC setup
    public uint cosmeticsDLCAppId = 0;

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

    public void Update()
    {
        Steamworks.SteamClient.RunCallbacks();
    }

    public bool HasCosmeticsDLC()
    {
        return Steamworks.SteamApps.IsDlcInstalled(cosmeticsDLCAppId);
    }

    // STATS
    public void IncrementNumKilledEnemies()
    {
        Steamworks.Data.Stat stat = new Steamworks.Data.Stat("NumKilledEnemies");
        stat.Set(stat.GetInt() + 1);
        Debug.Log(stat.GetInt());
    }

    public void IncrementNumCrystalsUsed()
    {
        Steamworks.Data.Stat stat = new Steamworks.Data.Stat("NumCrystalsUsed");
        stat.Set(stat.GetInt() + 1);
    }

    public void IncrementNumTurretKills()
    {
        Steamworks.Data.Stat stat = new Steamworks.Data.Stat("NumTurretKills");
        stat.Set(stat.GetInt() + 1);
    }

    public void IncrementNumTimesSawOnFire()
    {
        Steamworks.Data.Stat stat = new Steamworks.Data.Stat("NumTimesSawOnFire");
        stat.Set(stat.GetInt() + 1);
    }

    public void IncrementNumEnemiesKilledByTyphoon()
    {
        Steamworks.Data.Stat stat = new Steamworks.Data.Stat("NumEnemiesKilledByTyphoon");
        stat.Set(stat.GetInt() + 1);
    }

    public void IncrementNumZappedEnemiesKilled()
    {
        Steamworks.Data.Stat stat = new Steamworks.Data.Stat("NumZappedEnemiesKilled");
        stat.Set(stat.GetInt() + 1);
    }

    public void TrySetHighestZappedEnemiesWithSingleChainLightning(int candidateValue)
    {
        Steamworks.Data.Stat stat = new Steamworks.Data.Stat("HighestZappedEnemiesWithSingleChainLightning");
        if (candidateValue > stat.GetInt())
        {
            stat.Set(candidateValue);
        }
    }

    public void TrySetHighestAnomalySawUnleash(int candidateValue)
    {
        Steamworks.Data.Stat stat = new Steamworks.Data.Stat("HighestAnomalySawUnleash");
        if (candidateValue > stat.GetInt())
        {
            stat.Set(candidateValue);
        }
    }

    public void IncrementTotalNumberOfAnomalySawUnleash(int amount)
    {
        Steamworks.Data.Stat stat = new Steamworks.Data.Stat("TotalNumberOfAnomalySawUnleash");
        stat.Set(stat.GetInt() + amount);
    }

    public void TrySetHighestEnemyDeathTollFromSawmageddonShot(int candidateValue)
    {
        Steamworks.Data.Stat stat = new Steamworks.Data.Stat("HighestEnemyDeathTollFromSawmageddonShot");
        if (candidateValue > stat.GetInt())
        {
            stat.Set(candidateValue);
        }
    }

    public void TrySetHighestSurvivalWave(int candidateValue)
    {
        Steamworks.Data.Stat stat = new Steamworks.Data.Stat("HighestSurvivalWave");
        if (candidateValue > stat.GetInt())
        {
            stat.Set(candidateValue);
        }
    }

    public void IncrementTotalWavesCompleted()
    {
        Steamworks.Data.Stat stat = new Steamworks.Data.Stat("TotalWavesCompleted");
        stat.Set(stat.GetInt() + 1);
    }

    public void IncrementTotalWealthEarned(int amount)
    {
        Steamworks.Data.Stat stat = new Steamworks.Data.Stat("TotalWealthEarned");
        stat.Set(stat.GetInt() + amount);
    }

    public void IncrementTotalFailures()
    {
        Steamworks.Data.Stat stat = new Steamworks.Data.Stat("TotalFailures");
        stat.Set(stat.GetInt() + 1);
    }
}
