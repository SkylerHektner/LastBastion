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
    public PDList<string> PlayerChallengeCompletionList = new PDList<string>();

    // UPGRADES
    public enum UpgradeFlags
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

    [MenuItem("Debug/EndTheSuffering")]
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
#if UNITY_EDITOR
                else
                {
                    UnityEngine.Debug.LogWarning( "Unable to find player data unlock flag. Ignoring: " + key );
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