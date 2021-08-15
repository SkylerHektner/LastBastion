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
//          2) Update the UnlockFlagCurseMap
public enum UnlockFlags
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
}

// PLAYER DATA
public class PD
{
    // DATA
    public PlayerDataField<int> PlayerWealth = new PlayerDataField<int>();
    public PlayerUpgradeUnlockMap UnlockMap = new PlayerUpgradeUnlockMap();
    public PlayerLevelCompletionMap LevelCompletionMap = new PlayerLevelCompletionMap();
    public PDList<string> PlayerChallengeCompletionList = new PDList<string>();
    // limbo data
    public PlayerDataField<bool> Limbo = new PlayerDataField<bool>();
    public PlayerDataField<string> ExitedScene = new PlayerDataField<string>();
    public PlayerDataField<int> StoredLimboLevelIndex = new PlayerDataField<int>();
    public PlayerDataField<int> StoredLimboCurrentWave = new PlayerDataField<int>();
    public PlayerDataField<int> StoredLimboSurvivalIndex = new PlayerDataField<int>();
    public PlayerDataField<float> StoredMusicVolume = new PlayerDataField<float>();
    public PlayerDataField<float> StoredSFXVolume = new PlayerDataField<float>();
    public PlayerDataField<int> GameBegun = new PlayerDataField<int>();



    // a dictionary containing information about the dependencies of each unlock flag
    public readonly Dictionary<UnlockFlags, List<UnlockFlags>> UnlockFlagDependencyMap = new Dictionary<UnlockFlags, List<UnlockFlags>>
    {
        // boons
        { UnlockFlags.ChainLightning, new List<UnlockFlags> { } },
        { UnlockFlags.ChainLightningStunDuration, new List<UnlockFlags> { UnlockFlags.ChainLightning } },
        { UnlockFlags.ChainLightningLightningRod, new List<UnlockFlags> { UnlockFlags.ChainLightning, UnlockFlags.ChainLightningStunDuration } },
        { UnlockFlags.ChainLightningStaticOverload, new List<UnlockFlags> { UnlockFlags.ChainLightning, UnlockFlags.ChainLightningStunDuration, UnlockFlags.ChainLightningLightningRod } },
        { UnlockFlags.Typhoon, new List<UnlockFlags> { } },
        { UnlockFlags.TyphoonFlameSaw, new List<UnlockFlags> { UnlockFlags.Typhoon } },
        { UnlockFlags.TyphoonRoaringFlames, new List<UnlockFlags> { UnlockFlags.Typhoon, UnlockFlags.TyphoonFlameSaw } },
        { UnlockFlags.TyphoonExtendedBBQ, new List<UnlockFlags> { UnlockFlags.Typhoon, UnlockFlags.TyphoonFlameSaw, UnlockFlags.TyphoonRoaringFlames } },
        { UnlockFlags.Anomaly, new List<UnlockFlags> { } },
        { UnlockFlags.AnomalyRicochetSaws, new List<UnlockFlags> { UnlockFlags.Anomaly } },
        { UnlockFlags.AnomalyStasisCoating, new List<UnlockFlags> { UnlockFlags.Anomaly, UnlockFlags.AnomalyRicochetSaws } },
        { UnlockFlags.AnomalySingularity, new List<UnlockFlags> { UnlockFlags.Anomaly, UnlockFlags.AnomalyRicochetSaws, UnlockFlags.AnomalyStasisCoating } },
        { UnlockFlags.Sawmageddon, new List<UnlockFlags> { } },
        { UnlockFlags.SawmageddonDuration, new List<UnlockFlags> { UnlockFlags.Sawmageddon } },
        { UnlockFlags.SawmageddonProjectiles, new List<UnlockFlags> { UnlockFlags.Sawmageddon, UnlockFlags.SawmageddonDuration } },
        { UnlockFlags.SawmageddonComboKiller, new List<UnlockFlags> { UnlockFlags.Sawmageddon, UnlockFlags.SawmageddonDuration, UnlockFlags.SawmageddonProjectiles } },
        { UnlockFlags.BaseHP1, new List<UnlockFlags> { } },
        { UnlockFlags.BaseHP2, new List<UnlockFlags> { UnlockFlags.BaseHP1 } },
        { UnlockFlags.BaseHP3, new List<UnlockFlags> { UnlockFlags.BaseHP1, UnlockFlags.BaseHP2 } },
        { UnlockFlags.BaseOvershield, new List<UnlockFlags> { UnlockFlags.BaseHP1, UnlockFlags.BaseHP2, UnlockFlags.BaseHP3 } },
        { UnlockFlags.Turrets, new List<UnlockFlags> { } },
        { UnlockFlags.TurretsPowerSurge, new List<UnlockFlags> { UnlockFlags.Turrets, UnlockFlags.ChainLightning } },
        { UnlockFlags.TurretsCollateralDamage, new List<UnlockFlags> { UnlockFlags.Turrets, UnlockFlags.Sawmageddon } },
        { UnlockFlags.TurretsTimedPaylod, new List<UnlockFlags> { UnlockFlags.Turrets, UnlockFlags.Anomaly } },

        // curse
        { UnlockFlags.EnemyMovementSpeedCurse, new List<UnlockFlags> { } },
        { UnlockFlags.EnemySpawnSpeedCurse, new List<UnlockFlags> { } },
        { UnlockFlags.SawRadiusCurse, new List<UnlockFlags> { } },
        { UnlockFlags.SawMovementSpeedCurse, new List<UnlockFlags> { } },
        { UnlockFlags.CrystalDropChanceCurse, new List<UnlockFlags> { } },
        { UnlockFlags.AbilityDurationCurse, new List<UnlockFlags> { } },
        { UnlockFlags.EnemyMovementSpeedCurse2, new List<UnlockFlags> { UnlockFlags.EnemyMovementSpeedCurse } },
        { UnlockFlags.EnemySpawnSpeedCurse2, new List<UnlockFlags> { UnlockFlags.EnemySpawnSpeedCurse } },
        { UnlockFlags.SawRadiusCurse2, new List<UnlockFlags> { UnlockFlags.SawRadiusCurse } },
        { UnlockFlags.SawMovementSpeedCurse2, new List<UnlockFlags> { UnlockFlags.SawMovementSpeedCurse } },
        { UnlockFlags.CrystalDropChanceCurse2, new List<UnlockFlags> { UnlockFlags.CrystalDropChanceCurse } },
        { UnlockFlags.AbilityDurationCurse2, new List<UnlockFlags> { UnlockFlags.AbilityDurationCurse } },

        { UnlockFlags.SkeletonUpgradeCurse, new List<UnlockFlags> { } },
        { UnlockFlags.BolterUpgradeCurse, new List<UnlockFlags> { } },
        { UnlockFlags.ShieldSkeletonUpgradeCurse, new List<UnlockFlags> { } },
        { UnlockFlags.PumpkinUpgradeCurse, new List<UnlockFlags> { } },
        { UnlockFlags.CarrierUpgradeCurse, new List<UnlockFlags> { } },
        { UnlockFlags.MudSlingerUpgradeCurse, new List<UnlockFlags> { } },
        { UnlockFlags.ShrikeUpgradeCurse, new List<UnlockFlags> { } },
        { UnlockFlags.SummonerUpgradeCurse, new List<UnlockFlags> { } },
        { UnlockFlags.GhostUpgradeCurse, new List<UnlockFlags> { } },
        { UnlockFlags.BlackholeUpgradeCurse, new List<UnlockFlags> { } },
        { UnlockFlags.BouncerUpgradeCurse, new List<UnlockFlags> { } },
        { UnlockFlags.BomberUpgradeCurse, new List<UnlockFlags> { } },
    };

    // a dictionary containing information about whether an unlock flag is a curse
    public readonly Dictionary<UnlockFlags, bool> UnlockFlagCurseMap = new Dictionary<UnlockFlags, bool>
    {
        // boon
        { UnlockFlags.ChainLightning, false },
        { UnlockFlags.ChainLightningStunDuration, false },
        { UnlockFlags.ChainLightningLightningRod, false },
        { UnlockFlags.ChainLightningStaticOverload, false },
        { UnlockFlags.Typhoon, false },
        { UnlockFlags.TyphoonFlameSaw, false },
        { UnlockFlags.TyphoonRoaringFlames, false },
        { UnlockFlags.TyphoonExtendedBBQ, false },
        { UnlockFlags.Anomaly, false },
        { UnlockFlags.AnomalyRicochetSaws, false },
        { UnlockFlags.AnomalyStasisCoating, false },
        { UnlockFlags.AnomalySingularity, false },
        { UnlockFlags.Sawmageddon, false },
        { UnlockFlags.SawmageddonDuration, false },
        { UnlockFlags.SawmageddonProjectiles, false },
        { UnlockFlags.SawmageddonComboKiller, false },
        { UnlockFlags.BaseHP1, false },
        { UnlockFlags.BaseHP2, false },
        { UnlockFlags.BaseHP3, false },
        { UnlockFlags.BaseOvershield, false },
        { UnlockFlags.Turrets, false },
        { UnlockFlags.TurretsPowerSurge, false },
        { UnlockFlags.TurretsCollateralDamage, false },
        { UnlockFlags.TurretsTimedPaylod, false },

        // curse
        { UnlockFlags.EnemyMovementSpeedCurse, true },
        { UnlockFlags.EnemySpawnSpeedCurse, true },
        { UnlockFlags.SawRadiusCurse, true },
        { UnlockFlags.SawMovementSpeedCurse, true },
        { UnlockFlags.CrystalDropChanceCurse, true },
        { UnlockFlags.AbilityDurationCurse, true },
        { UnlockFlags.EnemyMovementSpeedCurse2, true },
        { UnlockFlags.EnemySpawnSpeedCurse2, true },
        { UnlockFlags.SawRadiusCurse2, true },
        { UnlockFlags.SawMovementSpeedCurse2, true },
        { UnlockFlags.CrystalDropChanceCurse2, true },
        { UnlockFlags.AbilityDurationCurse2, true },

        { UnlockFlags.SkeletonUpgradeCurse, true },
        { UnlockFlags.BolterUpgradeCurse, true },
        { UnlockFlags.ShieldSkeletonUpgradeCurse, true },
        { UnlockFlags.PumpkinUpgradeCurse, true },
        { UnlockFlags.CarrierUpgradeCurse, true },
        { UnlockFlags.MudSlingerUpgradeCurse, true },
        { UnlockFlags.ShrikeUpgradeCurse, true },
        { UnlockFlags.SummonerUpgradeCurse, true },
        { UnlockFlags.GhostUpgradeCurse, true },
        { UnlockFlags.BlackholeUpgradeCurse, true },
        { UnlockFlags.BouncerUpgradeCurse, true },
        { UnlockFlags.BomberUpgradeCurse, true },
    };

    // EVENTS
    [NonSerialized] public UnityEvent<UnlockFlags, bool> UpgradeFlagChangedEvent = new UnityEvent<UnlockFlags, bool>();

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
    }
    [MenuItem( "Debug/DeleteAllPlayerData/NoSeriouslyDeleteItAll" )]
    public static void DeleteAllPlayerData()
    {
        File.Delete( GetPath() );
        _instance = new PD();
        _instance.SetDirty();
    }
    [MenuItem( "Debug/UnlockEverything" )]
    public static void UnlockEverything()
    {
        foreach( UnlockFlags flag in Enum.GetValues( typeof( UnlockFlags ) ) )
        {
            if( !PD.Instance.UnlockFlagCurseMap[flag] )
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
    [MenuItem("Debug/EnableAllCurses")]
    public static void EnableAllCurses()
    {
        foreach( UnlockFlags flag in Enum.GetValues( typeof( UnlockFlags ) ) )
        {
            if( PD.Instance.UnlockFlagCurseMap[flag] )
            {
                _instance.UnlockMap.Set( flag, true, false );
                _instance.UnlockMap.Set( flag, true, true );
            }
        }
    }
    [MenuItem( "Debug/DisableAllCurses" )]
    public static void DisableAllCurses()
    {
        foreach( UnlockFlags flag in Enum.GetValues( typeof( UnlockFlags ) ) )
        {
            if( PD.Instance.UnlockFlagCurseMap[flag] )
            {
                _instance.UnlockMap.Set( flag, false, false );
                _instance.UnlockMap.Set( flag, false, true );
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
#endif

    // NON STATIC
    private bool dirty = false;
    ~PD()
    {
        SaveData();
    }

    // called once per frame in the Spectator
    public void Tick()
    {
        SaveData();
    }

    public void SetDirty()
    {
        dirty = true;
    }

    public void SaveData()
    {
        if( !dirty )
            return;

        string data = JsonUtility.ToJson( this, true );
        File.WriteAllText( GetPath(), data );
        dirty = false;
    }

    public void Start()
    {
# if UNITY_EDITOR
        // this is really just here for debug purposes
        foreach( UnlockFlags flag in Enum.GetValues( typeof( UnlockFlags ) ) )
        {
            if( !UnlockFlagDependencyMap.ContainsKey( flag ) )
            {
                UnityEngine.Debug.LogError( $"UnlockFlagDependencyMap missing entry for Unlock Flag {flag}" );
            }
            if( !UnlockFlagCurseMap.ContainsKey( flag ) )
            {
                UnityEngine.Debug.LogError( $"UnlockFlagCurseMap missing entry for Unlock Flag {flag}" );
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
}

[System.Serializable]
public class PlayerUpgradeUnlockMap : ISerializationCallbackReceiver
{
    private Dictionary<UnlockFlags, bool> campaign_unlock_map = new Dictionary<UnlockFlags, bool>();
    private Dictionary<UnlockFlags, bool> survival_unlock_map = new Dictionary<UnlockFlags, bool>();

    [SerializeField] List<string> serialized_campaign_unlock_flags;
    [SerializeField] List<string> serialized_survival_unlock_flags;

    private static Dictionary<string, UnlockFlags> valid_enum_strings;

    public bool Get( UnlockFlags flag )
    {
        UnityEngine.Debug.Assert( GameplayManager.Instance != null );
        return GameplayManager.Instance.Survival ? survival_unlock_map[flag] : campaign_unlock_map[flag];
    }

    public bool Get( UnlockFlags flag, bool survival )
    {
        return survival ? survival_unlock_map[flag] : campaign_unlock_map[flag];
    }

    public void Set( UnlockFlags flag, bool value, bool survival )
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
        PD.Instance?.UpgradeFlagChangedEvent.Invoke( flag, value );
    }

    public PlayerUpgradeUnlockMap()
    {
        foreach( UnlockFlags flag in Enum.GetValues( typeof( UnlockFlags ) ) )
        {
            campaign_unlock_map.Add( flag, false );
            survival_unlock_map.Add( flag, false );
        }
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
            valid_enum_strings = new Dictionary<string, UnlockFlags>();
            foreach( UnlockFlags flag in Enum.GetValues( typeof( UnlockFlags ) ) )
            {
                valid_enum_strings.Add( flag.ToString(), flag );
            }
        }

        // add an entry for every possible unlock flag
        foreach( UnlockFlags flag in Enum.GetValues( typeof( UnlockFlags ) ) )
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
                UnlockFlags out_flag;
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
                UnlockFlags out_flag;
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