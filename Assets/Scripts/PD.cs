using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using UnityEditor;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine.Events;

// UNLOCK FLAGS
//      When adding new Unlock Flags remember to 
//          1) Update the UnlockFlagDependencyMap
//          2) Update the UnlockFlagCategoryMap
public enum UnlockFlag
{
    ChainLightning = 0,
    ChainLightningStunDuration = 1,
    ChainLightningStaticOverload = 2,
    ChainLightningLightningRod = 3,
    Typhoon = 4,
    TyphoonExtendedBBQ = 5,
    TyphoonFlameSaw = 6,
    TyphoonRoaringFlames = 7,
    Anomaly = 8,
    AnomalyRicochetSaws = 9,
    AnomalyStasisCoating = 10,
    AnomalySingularity = 11,
    Sawmageddon = 12,
    SawmageddonDuration = 13,
    SawmageddonProjectiles = 14,
    SawmageddonComboKiller = 15,
    BaseOvershield = 16,
    BaseHP1 = 17,
    BaseHP2 = 18,
    BaseHP3 = 19,
    Turrets = 20,
    TurretsPowerSurge = 21,
    TurretsCollateralDamage = 22,
    TurretsTimedPaylod = 23,

    // CURSE FLAGS
    EnemyMovementSpeedCurse = 24,
    EnemySpawnSpeedCurse = 25,
    SawRadiusCurse = 26,
    SawMovementSpeedCurse = 27,
    CrystalDropChanceCurse = 28,
    AbilityDurationCurse = 29,
    SkeletonUpgradeCurse = 30,
    BolterUpgradeCurse = 31,
    ShieldSkeletonUpgradeCurse = 32,
    PumpkinUpgradeCurse = 33,
    CarrierUpgradeCurse = 34,
    MudSlingerUpgradeCurse = 35,
    ShrikeUpgradeCurse = 36,
    SummonerUpgradeCurse = 37,
    EnemyMovementSpeedCurse2 = 38,
    EnemySpawnSpeedCurse2 = 39,
    SawRadiusCurse2 = 40,
    SawMovementSpeedCurse2 = 41,
    CrystalDropChanceCurse2 = 42,
    AbilityDurationCurse2 = 43,
    GhostUpgradeCurse = 44,
    BlackholeUpgradeCurse = 45,
    BouncerUpgradeCurse = 46,
    BomberUpgradeCurse = 47,

    // COSMETIC FLAGS
    Default_LaunchArrow = 48,
    Default_SawTrail = 49,
    Default_SawSkin = 50,
    // premium
    NeonLuna_SawSkin = 51,
    NeonSol_SawSkin = 52,
    Compact_SawSkin = 53,
    Origami_SawSkin = 54,
    SweetToothSlicer_SawSkin = 55,
    Shuriken_SawSkin = 56,
    Hearts_SawTrail = 57,
    Candies_SawTrail = 58,
    Feathers_SawTrail = 59,
    Pumpkins_SawTrail = 60,
    Eyeballs_SawTrail = 61,
    PaperPlane_LaunchArrow = 62,
    CandyCorn_LaunchArrow = 63,
    ThrowingKnife_LaunchArrow = 64,
    PointFinger_LaunchArrow = 65,
    Sword_LaunchArrow = 66,
    // bonus
    Amoeba_SawSkin = 67,
    Cookie_SawSkin = 68,
    FrostedSprinkle_SawSkin = 69,
    Ghostly_SawSkin = 70,
    Glacier_SawSkin = 71,
    Magma_SawSkin = 72,
    Golden_SawSkin = 73,
    Pineapple_SawSkin = 74,
    RainbowPinwheeel_SawSkin = 75,
    Slime_SawSkin = 76,
    SpiderWeb_SawTrail = 77,
    BubbleBlue_SawTrail = 78,
    BubbleOrange_SawTrail = 79,
    BubbleGreen_SawTrail = 80,
    BubblePurple_SawTrail = 81,
    Wooden_LaunchArrow = 82,
    Rainbow_LaunchArrow = 83,
    Ghostly_LaunchArrow = 84,
    Simple_LaunchArrow = 85,
    Golden_LaunchArrow = 86,
    Stars_SawTrail = 87,
    Cleaver_SawSkin = 88,


}

public enum UnlockFlagCategory
{
    Upgrade,
    Curse,
    Cosmetic,
}

// PLAYER DATA
public class PD
{
    // DATA
    public PlayerDataField<int> PlayerWealth = new PlayerDataField<int>( 0 );
    public PlayerUpgradeUnlockMap UnlockMap = new PlayerUpgradeUnlockMap();
    public PlayerLevelCompletionMap LevelCompletionMap = new PlayerLevelCompletionMap();
    public PDList<string> PlayerChallengeCompletionList = new PDList<string>();
    // limbo data
    public LimboResumeInformation CampaignLimboResumeInformation = new LimboResumeInformation();
    public LimboResumeInformation SurvivalLimboResumeInformation = new LimboResumeInformation();
    // settings data
    public PlayerDataField<float> StoredMusicVolume = new PlayerDataField<float>( 0.0f );
    public PlayerDataField<float> StoredSFXVolume = new PlayerDataField<float>( 0.0f );
    public PlayerDataField<int> GameBegun = new PlayerDataField<int>( 0 );
    // player stats data
    public LazyPlayerDataField<int> NumKilledEnemies = new LazyPlayerDataField<int>();
    public LazyPlayerDataField<int> NumCrystalsUsed = new LazyPlayerDataField<int>();
    public LazyPlayerDataField<int> NumTurretKills = new LazyPlayerDataField<int>();
    public LazyPlayerDataField<int> NumTimesSawOnFire = new LazyPlayerDataField<int>();
    public LazyPlayerDataField<int> NumEnemiesKilledByTyphoon = new LazyPlayerDataField<int>(); // test
    public LazyPlayerDataField<int> NumZappedEnemiesKilled = new LazyPlayerDataField<int>();
    public LazyPlayerDataField<int> HighestZappedEnemiesWithSingleChainLightning = new LazyPlayerDataField<int>(); // test
    public LazyPlayerDataField<int> HighestAnomalySawUnleash = new LazyPlayerDataField<int>();
    public LazyPlayerDataField<int> TotalNumberOfAnomalySawUnleash = new LazyPlayerDataField<int>();
    public LazyPlayerDataField<int> HighestEnemyDeathTollFromSawmageddonShot = new LazyPlayerDataField<int>();
    public LazyPlayerDataField<int> HighestSurvivalWave = new LazyPlayerDataField<int>();
    public LazyPlayerDataField<int> TotalWavesCompleted = new LazyPlayerDataField<int>();
    public LazyPlayerDataField<int> TotalWealthEarned = new LazyPlayerDataField<int>();
    public LazyPlayerDataField<int> TotalFailures = new LazyPlayerDataField<int>();
    public LazyPlayerDataField<int> NumEnemiesKilledByBoomerExplosions = new LazyPlayerDataField<int>(); // test
    public LazyPlayerDataField<int> NumTimesSawBouncedOffWall = new LazyPlayerDataField<int>(); // test
    public LazyPlayerDataField<int> NumTimesCoveredInMud = new LazyPlayerDataField<int>(); // test
    public LazyPlayerDataField<int> NumTimesMudRemovedWithFire = new LazyPlayerDataField<int>(); // test
    public LazyPlayerDataField<int> TotalHealthRecoveredFromSawmageddon = new LazyPlayerDataField<int>(); // test
    // achievements
    public PDList<string> EarnedAchievementList = new PDList<string>();
    public PlayerDataField<int> AchievementPoints = new PlayerDataField<int>( 0 );
    // encountered enemies
    public PDList<EnemyEnum> EncounteredEnemyList = new PDList<EnemyEnum>();
    // cosmetics
    public PlayerDataField<UnlockFlag> EquippedLaunchArrow = new PlayerDataField<UnlockFlag>( UnlockFlag.Default_LaunchArrow );
    public PlayerDataField<UnlockFlag> EquippedSawTrail = new PlayerDataField<UnlockFlag>( UnlockFlag.Default_SawTrail );
    public PlayerDataField<UnlockFlag> EquippedSawSkin = new PlayerDataField<UnlockFlag>( UnlockFlag.Default_SawSkin );

    // a dictionary containing information about the dependencies of each unlock flag
    public readonly Dictionary<UnlockFlag, List<UnlockFlag>> UnlockFlagDependencyMap = new Dictionary<UnlockFlag, List<UnlockFlag>>
    {
        // boons
        { UnlockFlag.ChainLightning, new List<UnlockFlag> { } },
        { UnlockFlag.ChainLightningStunDuration, new List<UnlockFlag> { UnlockFlag.ChainLightning } },
        { UnlockFlag.ChainLightningLightningRod, new List<UnlockFlag> { UnlockFlag.ChainLightning, UnlockFlag.ChainLightningStunDuration } },
        { UnlockFlag.ChainLightningStaticOverload, new List<UnlockFlag> { UnlockFlag.ChainLightning, UnlockFlag.ChainLightningStunDuration, UnlockFlag.ChainLightningLightningRod } },
        { UnlockFlag.Typhoon, new List<UnlockFlag> { } },
        { UnlockFlag.TyphoonFlameSaw, new List<UnlockFlag> { UnlockFlag.Typhoon } },
        { UnlockFlag.TyphoonRoaringFlames, new List<UnlockFlag> { UnlockFlag.Typhoon, UnlockFlag.TyphoonFlameSaw } },
        { UnlockFlag.TyphoonExtendedBBQ, new List<UnlockFlag> { UnlockFlag.Typhoon, UnlockFlag.TyphoonFlameSaw, UnlockFlag.TyphoonRoaringFlames } },
        { UnlockFlag.Anomaly, new List<UnlockFlag> { } },
        { UnlockFlag.AnomalyRicochetSaws, new List<UnlockFlag> { UnlockFlag.Anomaly } },
        { UnlockFlag.AnomalyStasisCoating, new List<UnlockFlag> { UnlockFlag.Anomaly, UnlockFlag.AnomalyRicochetSaws } },
        { UnlockFlag.AnomalySingularity, new List<UnlockFlag> { UnlockFlag.Anomaly, UnlockFlag.AnomalyRicochetSaws, UnlockFlag.AnomalyStasisCoating } },
        { UnlockFlag.Sawmageddon, new List<UnlockFlag> { } },
        { UnlockFlag.SawmageddonDuration, new List<UnlockFlag> { UnlockFlag.Sawmageddon } },
        { UnlockFlag.SawmageddonProjectiles, new List<UnlockFlag> { UnlockFlag.Sawmageddon, UnlockFlag.SawmageddonDuration } },
        { UnlockFlag.SawmageddonComboKiller, new List<UnlockFlag> { UnlockFlag.Sawmageddon, UnlockFlag.SawmageddonDuration, UnlockFlag.SawmageddonProjectiles } },
        { UnlockFlag.BaseHP1, new List<UnlockFlag> { } },
        { UnlockFlag.BaseHP2, new List<UnlockFlag> { UnlockFlag.BaseHP1 } },
        { UnlockFlag.BaseHP3, new List<UnlockFlag> { UnlockFlag.BaseHP1, UnlockFlag.BaseHP2 } },
        { UnlockFlag.BaseOvershield, new List<UnlockFlag> { UnlockFlag.BaseHP1, UnlockFlag.BaseHP2, UnlockFlag.BaseHP3 } },
        { UnlockFlag.Turrets, new List<UnlockFlag> { } },
        { UnlockFlag.TurretsPowerSurge, new List<UnlockFlag> { UnlockFlag.Turrets, UnlockFlag.ChainLightning } },
        { UnlockFlag.TurretsCollateralDamage, new List<UnlockFlag> { UnlockFlag.Turrets, UnlockFlag.Sawmageddon } },
        { UnlockFlag.TurretsTimedPaylod, new List<UnlockFlag> { UnlockFlag.Turrets, UnlockFlag.Anomaly } },

        // curse
        { UnlockFlag.EnemyMovementSpeedCurse, new List<UnlockFlag> { } },
        { UnlockFlag.EnemySpawnSpeedCurse, new List<UnlockFlag> { } },
        { UnlockFlag.SawRadiusCurse, new List<UnlockFlag> { } },
        { UnlockFlag.SawMovementSpeedCurse, new List<UnlockFlag> { } },
        { UnlockFlag.CrystalDropChanceCurse, new List<UnlockFlag> { } },
        { UnlockFlag.AbilityDurationCurse, new List<UnlockFlag> { } },
        { UnlockFlag.EnemyMovementSpeedCurse2, new List<UnlockFlag> { UnlockFlag.EnemyMovementSpeedCurse } },
        { UnlockFlag.EnemySpawnSpeedCurse2, new List<UnlockFlag> { UnlockFlag.EnemySpawnSpeedCurse } },
        { UnlockFlag.SawRadiusCurse2, new List<UnlockFlag> { UnlockFlag.SawRadiusCurse } },
        { UnlockFlag.SawMovementSpeedCurse2, new List<UnlockFlag> { UnlockFlag.SawMovementSpeedCurse } },
        { UnlockFlag.CrystalDropChanceCurse2, new List<UnlockFlag> { UnlockFlag.CrystalDropChanceCurse } },
        { UnlockFlag.AbilityDurationCurse2, new List<UnlockFlag> { UnlockFlag.AbilityDurationCurse } },

        { UnlockFlag.SkeletonUpgradeCurse, new List<UnlockFlag> { } },
        { UnlockFlag.BolterUpgradeCurse, new List<UnlockFlag> { } },
        { UnlockFlag.ShieldSkeletonUpgradeCurse, new List<UnlockFlag> { } },
        { UnlockFlag.PumpkinUpgradeCurse, new List<UnlockFlag> { } },
        { UnlockFlag.CarrierUpgradeCurse, new List<UnlockFlag> { } },
        { UnlockFlag.MudSlingerUpgradeCurse, new List<UnlockFlag> { } },
        { UnlockFlag.ShrikeUpgradeCurse, new List<UnlockFlag> { } },
        { UnlockFlag.SummonerUpgradeCurse, new List<UnlockFlag> { } },
        { UnlockFlag.GhostUpgradeCurse, new List<UnlockFlag> { } },
        { UnlockFlag.BlackholeUpgradeCurse, new List<UnlockFlag> { } },
        { UnlockFlag.BouncerUpgradeCurse, new List<UnlockFlag> { } },
        { UnlockFlag.BomberUpgradeCurse, new List<UnlockFlag> { } },
    };

    // a dictionary containing information about whether an unlock flag is a curse
    public readonly Dictionary<UnlockFlag, UnlockFlagCategory> UnlockFlagCategoryMap = new Dictionary<UnlockFlag, UnlockFlagCategory>
    {
        // boon
        { UnlockFlag.ChainLightning, UnlockFlagCategory.Upgrade },
        { UnlockFlag.ChainLightningStunDuration, UnlockFlagCategory.Upgrade },
        { UnlockFlag.ChainLightningLightningRod, UnlockFlagCategory.Upgrade },
        { UnlockFlag.ChainLightningStaticOverload, UnlockFlagCategory.Upgrade },
        { UnlockFlag.Typhoon, UnlockFlagCategory.Upgrade },
        { UnlockFlag.TyphoonFlameSaw, UnlockFlagCategory.Upgrade },
        { UnlockFlag.TyphoonRoaringFlames, UnlockFlagCategory.Upgrade },
        { UnlockFlag.TyphoonExtendedBBQ, UnlockFlagCategory.Upgrade },
        { UnlockFlag.Anomaly, UnlockFlagCategory.Upgrade },
        { UnlockFlag.AnomalyRicochetSaws, UnlockFlagCategory.Upgrade },
        { UnlockFlag.AnomalyStasisCoating, UnlockFlagCategory.Upgrade },
        { UnlockFlag.AnomalySingularity, UnlockFlagCategory.Upgrade },
        { UnlockFlag.Sawmageddon, UnlockFlagCategory.Upgrade },
        { UnlockFlag.SawmageddonDuration, UnlockFlagCategory.Upgrade },
        { UnlockFlag.SawmageddonProjectiles, UnlockFlagCategory.Upgrade },
        { UnlockFlag.SawmageddonComboKiller, UnlockFlagCategory.Upgrade },
        { UnlockFlag.BaseHP1, UnlockFlagCategory.Upgrade },
        { UnlockFlag.BaseHP2, UnlockFlagCategory.Upgrade },
        { UnlockFlag.BaseHP3, UnlockFlagCategory.Upgrade },
        { UnlockFlag.BaseOvershield, UnlockFlagCategory.Upgrade },
        { UnlockFlag.Turrets, UnlockFlagCategory.Upgrade },
        { UnlockFlag.TurretsPowerSurge, UnlockFlagCategory.Upgrade },
        { UnlockFlag.TurretsCollateralDamage, UnlockFlagCategory.Upgrade },
        { UnlockFlag.TurretsTimedPaylod, UnlockFlagCategory.Upgrade },

        // curse
        { UnlockFlag.EnemyMovementSpeedCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.EnemySpawnSpeedCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.SawRadiusCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.SawMovementSpeedCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.CrystalDropChanceCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.AbilityDurationCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.EnemyMovementSpeedCurse2, UnlockFlagCategory.Curse },
        { UnlockFlag.EnemySpawnSpeedCurse2, UnlockFlagCategory.Curse },
        { UnlockFlag.SawRadiusCurse2, UnlockFlagCategory.Curse },
        { UnlockFlag.SawMovementSpeedCurse2, UnlockFlagCategory.Curse },
        { UnlockFlag.CrystalDropChanceCurse2, UnlockFlagCategory.Curse },
        { UnlockFlag.AbilityDurationCurse2, UnlockFlagCategory.Curse },

        { UnlockFlag.SkeletonUpgradeCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.BolterUpgradeCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.ShieldSkeletonUpgradeCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.PumpkinUpgradeCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.CarrierUpgradeCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.MudSlingerUpgradeCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.ShrikeUpgradeCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.SummonerUpgradeCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.GhostUpgradeCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.BlackholeUpgradeCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.BouncerUpgradeCurse, UnlockFlagCategory.Curse },
        { UnlockFlag.BomberUpgradeCurse, UnlockFlagCategory.Curse },

        // cosmetic
        { UnlockFlag.Default_LaunchArrow, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Default_SawTrail, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Default_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.NeonLuna_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.NeonSol_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Compact_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Origami_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.SweetToothSlicer_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Shuriken_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Hearts_SawTrail, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Candies_SawTrail, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Feathers_SawTrail, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Pumpkins_SawTrail, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Eyeballs_SawTrail, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.PaperPlane_LaunchArrow, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.CandyCorn_LaunchArrow, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.ThrowingKnife_LaunchArrow, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.PointFinger_LaunchArrow, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Sword_LaunchArrow, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Amoeba_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Cookie_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.FrostedSprinkle_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Ghostly_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Glacier_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Magma_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Golden_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Pineapple_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.RainbowPinwheeel_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Slime_SawSkin, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.SpiderWeb_SawTrail, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.BubbleBlue_SawTrail, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.BubbleOrange_SawTrail, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.BubbleGreen_SawTrail, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.BubblePurple_SawTrail, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Wooden_LaunchArrow, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Rainbow_LaunchArrow, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Ghostly_LaunchArrow, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Simple_LaunchArrow, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Golden_LaunchArrow, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Stars_SawTrail, UnlockFlagCategory.Cosmetic },
        { UnlockFlag.Cleaver_SawSkin, UnlockFlagCategory.Cosmetic },

    };

    // EVENTS
    [NonSerialized] public UnityEvent<UnlockFlag, bool> UnlockFlagChangedEvent = new UnityEvent<UnlockFlag, bool>();

    // STATICS
    public static PD Instance
    {
        get
        {
            if( _instance == null )
            {
                string path = GetPath();
                if( File.Exists( path ) )
                {
                    string data = File.ReadAllText( GetPath() );
                    _instance = JsonUtility.FromJson<PD>( data );
                }
                else
                {
                    _instance = new PD();
                }
            }
            return _instance;
        }
    }
    private static PD _instance;
    private static string GetPath()
    {
        return Path.Combine( Application.persistentDataPath, "PlayerData.txt" );
    }
#if UNITY_EDITOR
    [MenuItem( "Debug/OpenPersistentDataPath" )]
    public static void OpenPersistentDataPath()
    {
        Process.Start( Application.persistentDataPath );
    }
    [MenuItem( "Debug/Add1000Candy" )]
    public static void AddMoney()
    {
        PD.Instance.PlayerWealth.Set( PD.Instance.PlayerWealth.Get() + 1000 );
        PD.Instance.TotalWealthEarned.Set( PD.Instance.TotalWealthEarned.Get() + 1000 );
    }
    [MenuItem( "Debug/UnlockEverything" )]
    public static void UnlockEverything()
    {
        foreach( UnlockFlag flag in Enum.GetValues( typeof( UnlockFlag ) ) )
        {
            if( PD.Instance.UnlockFlagCategoryMap[flag] == UnlockFlagCategory.Upgrade )
            {
                _instance.UnlockMap.Set( flag, true, false );
                _instance.UnlockMap.Set( flag, true, true );
            }
        }
        PD.Instance.LevelCompletionMap.SetLevelCompletion( "Level1", true );
        PD.Instance.LevelCompletionMap.SetLevelCompletion( "Level2", true );
        PD.Instance.LevelCompletionMap.SetLevelCompletion( "Level3", true );
        PD.Instance.LevelCompletionMap.SetLevelCompletion( "Level4", true );
        PD.Instance.LevelCompletionMap.SetLevelCompletion( "Level5", true );
        PD.Instance.LevelCompletionMap.SetLevelCompletion( "Level6", true );
        PD.Instance.LevelCompletionMap.SetLevelCompletion( "Level7", true );
        PD.Instance.LevelCompletionMap.SetLevelCompletion( "Level8", true );
        PD.Instance.LevelCompletionMap.SetLevelCompletion( "Level9", true );
    }
    [MenuItem( "Debug/EnableAllCurses" )]
    public static void EnableAllCurses()
    {
        foreach( UnlockFlag flag in Enum.GetValues( typeof( UnlockFlag ) ) )
        {
            if( PD.Instance.UnlockFlagCategoryMap[flag] == UnlockFlagCategory.Curse )
            {
                _instance.UnlockMap.Set( flag, true, false );
                _instance.UnlockMap.Set( flag, true, true );
            }
        }
    }
    [MenuItem( "Debug/DisableAllCurses" )]
    public static void DisableAllCurses()
    {
        foreach( UnlockFlag flag in Enum.GetValues( typeof( UnlockFlag ) ) )
        {
            if( PD.Instance.UnlockFlagCategoryMap[flag] == UnlockFlagCategory.Curse )
            {
                _instance.UnlockMap.Set( flag, false, false );
                _instance.UnlockMap.Set( flag, false, true );
            }
        }
    }
    [MenuItem( "Debug/UnlockAllCosmetics" )]
    public static void UnlockAllCosmetics()
    {
        foreach( UnlockFlag flag in Enum.GetValues( typeof( UnlockFlag ) ) )
        {
            if( PD.Instance.UnlockFlagCategoryMap[flag] == UnlockFlagCategory.Cosmetic )
            {
                _instance.UnlockMap.Set( flag, true, false );
                _instance.UnlockMap.Set( flag, true, true );
            }
        }
    }
    [MenuItem( "Debug/EndTheSuffering" )]
    public static void KillMyself()
    {
        BaseHP.Instance?.ReduceHP( 9001 );
        BaseHP.Instance?.ReduceHP( 9001 );
        BaseHP.Instance?.ReduceHP( 9001 );
        BaseHP.Instance?.ReduceHP( 9001 );
        BaseHP.Instance?.ReduceHP( 9001 );
        BaseHP.Instance?.ReduceHP( 9001 );
    }

    [MenuItem( "Debug/Achievements/Add100AP" )]
    private static void Add1000AP()
    {
        PD.Instance.AchievementPoints.Set( PD.Instance.AchievementPoints.Get() + 1000 );
    }

    [MenuItem( "Debug/Achievements/Remove100AP" )]
    private static void Remove1000AP()
    {
        PD.Instance.AchievementPoints.Set( PD.Instance.AchievementPoints.Get() - 1000 );
    }
#endif
    // this is intentionally compiled into release builds as we use it to reset player progress
#if UNITY_EDITOR
    [MenuItem( "Debug/DeleteAllPlayerData/NoSeriouslyDeleteItAll" )]
#endif
    public static void DeleteAllPlayerData()
    {
        File.Delete( GetPath() );
        var UnlockFlagChangedEvent = _instance.UnlockFlagChangedEvent;
        _instance = new PD();
        _instance.UnlockFlagChangedEvent = UnlockFlagChangedEvent;
        foreach( UnlockFlag flag in Enum.GetValues( typeof( UnlockFlag ) ) )
        {
            _instance.UnlockFlagChangedEvent.Invoke( flag, false );
        }
        _instance.SetDirty();
    }

    // NON STATIC
    private bool dirty = false;
    private bool lazy_dirty = false; // lazy dirty only saves between waves - used for non critical data like player stats

    // called once per frame in the Spectator and Gameplay Manager
    public void Tick()
    {
        SaveData();
    }

    // called on wave advancement in the Gameplay Manager
    public void LazyTick()
    {
        LazySaveData();
    }

    public void SetDirty()
    {
        dirty = true;
    }

    public void SetLazyDirty()
    {
        lazy_dirty = true;
    }

    private void SaveData()
    {
        if( !dirty )
            return;
        SaveToDisk();
    }

    private void LazySaveData()
    {
        if( !lazy_dirty )
            return;
        SaveToDisk();
    }

    private void SaveToDisk()
    {
        string data = JsonUtility.ToJson( this, true );
        File.WriteAllText( GetPath(), data );
        lazy_dirty = false;
        dirty = false;
    }

    public void Start()
    {
# if UNITY_EDITOR
        // this is really just here for debug purposes
        foreach( UnlockFlag flag in Enum.GetValues( typeof( UnlockFlag ) ) )
        {
            if( !UnlockFlagCategoryMap.ContainsKey( flag ) )
            {
                UnityEngine.Debug.LogError( $"UnlockFlagCurseMap missing entry for Unlock Flag {flag}" );
            }
            else if( !UnlockFlagDependencyMap.ContainsKey( flag ) && UnlockFlagCategoryMap[flag] != UnlockFlagCategory.Cosmetic )
            {
                UnityEngine.Debug.LogError( $"UnlockFlagDependencyMap missing entry for Unlock Flag {flag}" );
            }
        }
# endif
    }
}

[System.Serializable]
public class PlayerDataField<T>
{
    [SerializeField] T value;
    public T Get()
    {
        return value;
    }
    public void Set( T _value )
    {
        value = _value;
        PD.Instance?.SetDirty();
    }

    public PlayerDataField( T initial_value )
    {
        value = initial_value;
    }
}

[System.Serializable]
public class LazyPlayerDataField<T>
{
    [SerializeField] T value;
    public T Get()
    {
        return value;
    }
    public void Set( T _value )
    {
        value = _value;
        PD.Instance?.SetLazyDirty();
    }
}

[System.Serializable]
public class PlayerUpgradeUnlockMap : ISerializationCallbackReceiver
{
    private Dictionary<UnlockFlag, bool> campaign_unlock_map = new Dictionary<UnlockFlag, bool>();
    private Dictionary<UnlockFlag, bool> survival_unlock_map = new Dictionary<UnlockFlag, bool>();

    [SerializeField] List<string> serialized_campaign_unlock_flags;
    [SerializeField] List<string> serialized_survival_unlock_flags;

    private static Dictionary<string, UnlockFlag> valid_enum_strings;

    public bool Get( UnlockFlag flag )
    {
        UnityEngine.Debug.Assert( GameplayManager.Instance != null );
        return GameplayManager.Instance.Survival ? survival_unlock_map[flag] : campaign_unlock_map[flag];
    }

    public bool Get( UnlockFlag flag, bool survival )
    {
        return survival ? survival_unlock_map[flag] : campaign_unlock_map[flag];
    }

    public void Set( UnlockFlag flag, bool value, bool survival )
    {
        if( survival )
        {
            survival_unlock_map[flag] = value;
        }
        else
        {
            campaign_unlock_map[flag] = value;
        }

        PD.Instance?.SetDirty();
        PD.Instance?.UnlockFlagChangedEvent.Invoke( flag, value );
    }

    public PlayerUpgradeUnlockMap()
    {
        foreach( UnlockFlag flag in Enum.GetValues( typeof( UnlockFlag ) ) )
        {
            campaign_unlock_map.Add( flag, false );
            survival_unlock_map.Add( flag, false );
        }

        // default cosmetics are always unlocked
        campaign_unlock_map[UnlockFlag.Default_LaunchArrow] = true;
        campaign_unlock_map[UnlockFlag.Default_SawSkin] = true;
        campaign_unlock_map[UnlockFlag.Default_SawTrail] = true;
        survival_unlock_map[UnlockFlag.Default_LaunchArrow] = true;
        survival_unlock_map[UnlockFlag.Default_SawSkin] = true;
        survival_unlock_map[UnlockFlag.Default_SawTrail] = true;
    }

    public void OnBeforeSerialize()
    {
        serialized_campaign_unlock_flags = new List<string>();
        foreach( var kvp in campaign_unlock_map )
        {
            if( kvp.Value )
                serialized_campaign_unlock_flags.Add( kvp.Key.ToString() );
        }

        serialized_survival_unlock_flags = new List<string>();
        foreach( var kvp in survival_unlock_map )
        {
            if( kvp.Value )
                serialized_survival_unlock_flags.Add( kvp.Key.ToString() );
        }
    }

    public void OnAfterDeserialize()
    {
        // populate valid_strings lookup map if not created yet
        if( valid_enum_strings == null )
        {
            valid_enum_strings = new Dictionary<string, UnlockFlag>();
            foreach( UnlockFlag flag in Enum.GetValues( typeof( UnlockFlag ) ) )
            {
                valid_enum_strings.Add( flag.ToString(), flag );
            }
        }

        // add an entry for every possible unlock flag
        foreach( UnlockFlag flag in Enum.GetValues( typeof( UnlockFlag ) ) )
        {
            if( !campaign_unlock_map.ContainsKey( flag ) )
                campaign_unlock_map.Add( flag, false );

            if( !survival_unlock_map.ContainsKey( flag ) )
                survival_unlock_map.Add( flag, false );
        }

        // set valid entries
        if( serialized_campaign_unlock_flags != null )
        {
            foreach( string key in serialized_campaign_unlock_flags )
            {
                UnlockFlag out_flag;
                if( valid_enum_strings.TryGetValue( key, out out_flag ) )
                {
                    campaign_unlock_map[out_flag] = true;
                }
#if UNITY_EDITOR
                else
                {
                    UnityEngine.Debug.LogWarning( "Unable to find player data campaign unlock flag. Ignoring: " + key );
                }
#endif
            }
        }

        if( serialized_survival_unlock_flags != null )
        {
            foreach( string key in serialized_survival_unlock_flags )
            {
                UnlockFlag out_flag;
                if( valid_enum_strings.TryGetValue( key, out out_flag ) )
                {
                    survival_unlock_map[out_flag] = true;
                }
#if UNITY_EDITOR
                else
                {
                    UnityEngine.Debug.LogWarning( "Unable to find player data survival unlock flag. Ignoring: " + key );
                }
#endif
            }
        }
    }
}

[System.Serializable]
public class LevelCompletionData
{
    public string LevelIdentifier;
    public bool Complete = false;
    public List<int> CompletedWaves = new List<int>();
}

[System.Serializable]
public class PlayerLevelCompletionMap : ISerializationCallbackReceiver
{
    private Dictionary<string, LevelCompletionData> level_map = new Dictionary<string, LevelCompletionData>();
    [SerializeField] private List<LevelCompletionData> serialized_level_completion_data = new List<LevelCompletionData>();

    public bool GetLevelCompletion( string level_identifier )
    {
        return GetLevelCompletionData( level_identifier ).Complete;
    }

    public void SetLevelCompletion( string level_identifier, bool completion )
    {
        GetLevelCompletionData( level_identifier ).Complete = completion;
        PD.Instance?.SetDirty();
    }

    public bool GetWaveCompletion( string level_identifier, int wave )
    {
        return GetLevelCompletionData( level_identifier ).CompletedWaves.Contains( wave );
    }

    public void SetWaveCompletion( string level_identifier, int wave, bool complete )
    {
        LevelCompletionData data = GetLevelCompletionData( level_identifier );
        if( complete && !data.CompletedWaves.Contains( wave ) )
        {
            data.CompletedWaves.Add( wave );
            PD.Instance?.SetDirty();
        }
        else if( !complete && data.CompletedWaves.Contains( wave ) )
        {
            data.CompletedWaves.Remove( wave );
            PD.Instance?.SetDirty();
        }
    }

    private LevelCompletionData GetLevelCompletionData( string level_identifier )
    {
        LevelCompletionData data;
        if( level_map.TryGetValue( level_identifier, out data ) )
        {
            return data;
        }
        else
        {
            data = new LevelCompletionData();
            data.LevelIdentifier = level_identifier;
            level_map.Add( level_identifier, data );
            PD.Instance?.SetDirty();
            return data;
        }
    }

    public void OnBeforeSerialize()
    {
        serialized_level_completion_data.Clear();
        foreach( LevelCompletionData data in level_map.Values )
        {
            serialized_level_completion_data.Add( data );
        }
    }

    public void OnAfterDeserialize()
    {
        foreach( LevelCompletionData data in serialized_level_completion_data )
        {
            level_map.Add( data.LevelIdentifier, data );
        }
        serialized_level_completion_data.Clear();
    }
}

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach( KeyValuePair<TKey, TValue> pair in this )
        {
            keys.Add( pair.Key );
            values.Add( pair.Value );
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        if( keys.Count != values.Count )
            throw new System.Exception( string.Format( "there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable." ) );

        for( int i = 0; i < keys.Count; i++ )
            this.Add( keys[i], values[i] );
    }
}

// only for use with primitive types
[Serializable]
public class PDList<T> : IList<T>
{
    [SerializeField]
    private List<T> list = new List<T>();

    public int Count => list.Count;

    public bool IsReadOnly => false;

    public T this[int index] { get => list[index]; set => list[index] = value; }

    public int IndexOf( T item )
    {
        return list.IndexOf( item );
    }

    public void Insert( int index, T item )
    {
        list.Insert( index, item );
        PD.Instance?.SetDirty();
    }

    public void RemoveAt( int index )
    {
        list.RemoveAt( index );
        PD.Instance?.SetDirty();
    }

    public void Add( T item )
    {
        list.Add( item );
        PD.Instance?.SetDirty();
    }

    public void Clear()
    {
        list.Clear();
        PD.Instance?.SetDirty();
    }

    public bool Contains( T item )
    {
        return list.Contains( item );
    }

    public void CopyTo( T[] array, int arrayIndex )
    {
        list.CopyTo( array, arrayIndex );
    }

    public bool Remove( T item )
    {
        bool result = list.Remove( item );
        if( result )
            PD.Instance?.SetDirty();
        return result;
    }

    public IEnumerator<T> GetEnumerator()
    {
        PD.Instance?.SetDirty(); // sucks but we don't know if they've modified anything while iterating
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        PD.Instance?.SetDirty(); // sucks but we don't know if they've modified anything while iterating
        return list.GetEnumerator();
    }
}

[System.Serializable]
public class LimboResumeInformation
{

    [SerializeField] private bool active = false;
    [SerializeField] private string scene_name;
    [SerializeField] private int level_index;
    [SerializeField] private int wave;
    [SerializeField] private float health;
    [SerializeField] private List<UnlockFlag> survival_unlocks = new List<UnlockFlag>();
    [SerializeField] private int survival_spawn_cadence_index = 0;

    public bool Active { get { return active; } }
    public string SceneName { get { return scene_name; } }
    public int LevelIndex { get { return level_index; } }
    public int Wave { get { return wave; } }
    public float Health { get { return health; } }
    public IReadOnlyList<UnlockFlag> SurvivalUnlocks { get { return survival_unlocks; } }
    public int SurvivalSpawnCadenceIndex { get { return survival_spawn_cadence_index; } }

    public void SetInfo( bool active, string level_name = "", int levelIndex = 0, int wave = 0, float health = 0, List<UnlockFlag> survival_unlocks = null, int survival_spawn_cadence_index = 0 )
    {
        this.active = active;
        this.scene_name = level_name;
        this.level_index = levelIndex;
        this.wave = wave;
        this.health = health;
        this.survival_unlocks = survival_unlocks;
        this.survival_spawn_cadence_index = survival_spawn_cadence_index;
        PD.Instance.SetDirty();
    }

    public void Clear()
    {
        active = false;
        scene_name = "";
        level_index = 0;
        wave = 0;
        health = 0.0f;
        survival_unlocks = null;
        survival_spawn_cadence_index = 0;
        PD.Instance.SetDirty();
    }
}