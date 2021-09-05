using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoStatsItem : InfoItem
{
    public string Name;
    public string Description;
    public PlayerStat stat;

    public enum PlayerStat
    {
        NumKilledEnemies,
        NumCrystalsUsed,
        NumTurretKills,
        NumTimesSawOnFire,
        NumZappedEnemiesKilled,
        HighestAnomalySawUnleash,
        TotalNumberOfAnomalySawUnleash,
        HighestEnemyDeathTollFromSawmageddonShot,
        HighestSurvivalWave,
        TotalWavesCompleted,
        TotalWealthEarned,
        TotalFailures,
    }

    public override void Start()
    {
        AchievementLocked = false;
    }

    public override string GetInfoName()
    {
        return GetProgressAmount(); // I swapped these
    }

    public override string GetInfoDescription()
    {
        return Name; // I swapped these
    }
    public override string GetProgressAmount()
    {
        switch( stat )
        {
            case PlayerStat.NumKilledEnemies:
                return PD.Instance.NumKilledEnemies.Get().ToString();
            case PlayerStat.NumCrystalsUsed:
                return PD.Instance.NumCrystalsUsed.Get().ToString();
            case PlayerStat.NumTurretKills:
                return PD.Instance.NumTurretKills.Get().ToString();
            case PlayerStat.NumTimesSawOnFire:
                return PD.Instance.NumTimesSawOnFire.Get().ToString();
            case PlayerStat.NumZappedEnemiesKilled:
                return PD.Instance.NumZappedEnemiesKilled.Get().ToString();
            case PlayerStat.HighestAnomalySawUnleash:
                return PD.Instance.HighestAnomalySawUnleash.Get().ToString();
            case PlayerStat.TotalNumberOfAnomalySawUnleash:
                return PD.Instance.TotalNumberOfAnomalySawUnleash.Get().ToString();
            case PlayerStat.HighestEnemyDeathTollFromSawmageddonShot:
                return PD.Instance.HighestEnemyDeathTollFromSawmageddonShot.Get().ToString();
            case PlayerStat.HighestSurvivalWave:
                return PD.Instance.HighestSurvivalWave.Get().ToString();
            case PlayerStat.TotalWavesCompleted:
                return PD.Instance.TotalWavesCompleted.Get().ToString();
            case PlayerStat.TotalWealthEarned:
                return PD.Instance.TotalWealthEarned.Get().ToString();
            case PlayerStat.TotalFailures:
                return PD.Instance.TotalFailures.Get().ToString();
        }

        return "";
    }
}
