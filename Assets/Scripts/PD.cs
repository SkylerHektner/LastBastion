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

// PLAYER DATA
public class PD
{
    // DATA
    public PlayerDataField<int> PlayerWealth = new PlayerDataField<int>();
    public PlayerDataField<bool> Limbo = new PlayerDataField<bool>();
    public PlayerDataField<string> ExitedScene = new PlayerDataField<string>();
    public PlayerDataField<int> StoredLimboLevelIndex = new PlayerDataField<int>();
    public PlayerUpgradeUnlockMap UpgradeUnlockMap = new PlayerUpgradeUnlockMap();
    public PlayerLevelCompletionMap LevelCompletionMap = new PlayerLevelCompletionMap();

    // UPGRADES
    public enum UpgradeFlags
    {
        ChainLightning,
        ChainLightningStunDuration,
        ChainLightningStaticOverload,    // not implemented
        ChainLightningLightningRod,
        Typhoon,
        TyphoonExtendedBBQ,              // not implemented
        TyphoonFlameSaw,
        TyphoonRoaringFlames,
        Anomaly,
        AnomalyRicochetSaws,
        AnomalyStasisCoating,            // not implemented
        AnomalySingularity,
        Sawmageddon,
        SawmageddonDuration,
        SawmageddonProjectiles,
        SawmageddonComboKiller,          // not implemented
        BaseOvershield,
        SoulCollector,                   // not implemented
        BaseHP1,
        BaseHP2,
        BaseHP3,
        Turrets1,                        // not implemented
        Turrets2,                        // not implemented
        Turrets3,                        // not implemented
        Turrets4,
    }

    // EVENTS
    [NonSerialized] public UnityEvent<UpgradeFlags, bool> UpgradeFlagChangedEvent = new UnityEvent<UpgradeFlags, bool>();

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
    [MenuItem( "Debug/Add100Candy" )]
    public static void AddMoney()
    {
        PD.Instance.PlayerWealth.Set( PD.Instance.PlayerWealth.Get() + 100 );
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
        foreach( UpgradeFlags flag in Enum.GetValues( typeof( UpgradeFlags ) ) )
        {
            _instance.UpgradeUnlockMap.SetUnlock( flag, true );
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
    private Dictionary<PD.UpgradeFlags, bool> unlock_map = new Dictionary<PD.UpgradeFlags, bool>();

    [SerializeField] List<string> serialized_unlock_flags;

    private static Dictionary<string, PD.UpgradeFlags> valid_enum_strings;

    public bool GetUnlock( PD.UpgradeFlags flag )
    {
        return unlock_map[flag];
    }

    public void SetUnlock( PD.UpgradeFlags flag, bool value )
    {
        unlock_map[flag] = value;
        PD.Instance?.SetDirty();
        PD.Instance?.UpgradeFlagChangedEvent.Invoke( flag, value );
    }

    public PlayerUpgradeUnlockMap()
    {
        foreach( PD.UpgradeFlags flag in Enum.GetValues( typeof( PD.UpgradeFlags ) ) )
        {
            unlock_map.Add( flag, false );
        }
    }

    public void OnBeforeSerialize()
    {
        serialized_unlock_flags = new List<string>();
        foreach( var kvp in unlock_map )
        {
            if( kvp.Value )
                serialized_unlock_flags.Add( kvp.Key.ToString() );
        }
    }

    public void OnAfterDeserialize()
    {
        // populate valid_strings lookup map if not created yet
        if( valid_enum_strings == null )
        {
            valid_enum_strings = new Dictionary<string, PD.UpgradeFlags>();
            foreach( PD.UpgradeFlags flag in Enum.GetValues( typeof( PD.UpgradeFlags ) ) )
            {
                valid_enum_strings.Add( flag.ToString(), flag );
            }
        }

        // add an entry for every possible unlock flag
        foreach( PD.UpgradeFlags flag in Enum.GetValues( typeof( PD.UpgradeFlags ) ) )
        {
            if( !unlock_map.ContainsKey( flag ) )
                unlock_map.Add( flag, false );
        }

        // set valid entries
        if( serialized_unlock_flags != null )
        {
            foreach( string key in serialized_unlock_flags )
            {
                PD.UpgradeFlags out_flag;
                if( valid_enum_strings.TryGetValue( key, out out_flag ) )
                {
                    unlock_map[out_flag] = true;
                }
                else
                {
                    UnityEngine.Debug.LogWarning( "Unable to find player data unlock flag. Ignoring: " + key );
                }
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

    public bool GetLevelCompletion(string level_identifier)
    {
        return GetLevelCompletionData( level_identifier ).Complete;
    }

    public void SetLevelCompletion( string level_identifier, bool completion )
    {
        GetLevelCompletionData( level_identifier ).Complete = completion;
        PD.Instance?.SetDirty();
    }

    public bool GetWaveCompletion(string level_identifier, int wave)
    {
        return GetLevelCompletionData( level_identifier ).CompletedWaves.Contains( wave );
    }

    public void SetWaveCompletion(string level_identifier, int wave, bool complete)
    {
        LevelCompletionData data = GetLevelCompletionData( level_identifier );
        if(complete && !data.CompletedWaves.Contains(wave))
        {
            data.CompletedWaves.Add( wave );
            PD.Instance?.SetDirty();
        }
        else if (!complete && data.CompletedWaves.Contains(wave))
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
        foreach(LevelCompletionData data in level_map.Values)
        {
            serialized_level_completion_data.Add( data );
        }
    }

    public void OnAfterDeserialize()
    {
        foreach(LevelCompletionData data in serialized_level_completion_data)
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