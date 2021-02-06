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

public class PlayerData
{
    // DATA
    public PlayerDataField<int> PlayerWealth = new PlayerDataField<int>();
    public PlayerDataField<bool> Limbo = new PlayerDataField<bool>();
    public PlayerDataField<string> ExitedScene = new PlayerDataField<string>();
    public PlayerUpgradeUnlockMap UpgradeUnlockMap = new PlayerUpgradeUnlockMap();

    // UPGRADES
    public enum UpgradeFlags
    {
        ChainLightning,                  // not implemented
        ChainLightningStunDuration,      // not implemented
        ChainLightningStaticOverload,    // not implemented
        ChainLightningLightningRod,      // not implemented
        Typhoon,                         // not implemented
        TyphoonExtendedBBQ,              // not implemented
        TyphoonFlameSaw,                 // not implemented
        TyphoonRoaringFlames,            // not implemented
        Anomaly,                         // not implemented
        AnomalyRicochetSaws,             // not implemented
        AnomalyStasisCoating,            // not implemented
        AnomalySingularity,              // not implemented
        Sawmageddon,                     // not implemented
        SawmageddonDuration,             // not implemented
        SawmageddonProjectiles,          // not implemented
        SawmageddonComboKiller,          // not implemented
        BaseOvershield,                  // not implemented
        SoulCollector,                   // not implemented
        BaseHP1,                         // not implemented
        BaseHP2,                         // not implemented
        BaseHP3,                         // not implemented
        Turrets1,                        // not implemented
        Turrets2,                        // not implemented
        Turrets3,                        // not implemented

    }

    // EVENTS
    public UnityEvent<UpgradeFlags, bool> UpgradeFlagChangedEvent = new UnityEvent<UpgradeFlags, bool>();

    // STATICS
    public static PlayerData Instance {
        get {
            if( _instance == null )
            {
                string path = GetPath();
                if( File.Exists( path ) )
                {
                    string data = File.ReadAllText( GetPath() );
                    _instance = JsonUtility.FromJson<PlayerData>( data );
                }
                else
                {
                    _instance = new PlayerData();
                }
            }
            return _instance;
        }
    }
    private static PlayerData _instance;
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
        PlayerData.Instance.PlayerWealth.Set( PlayerData.Instance.PlayerWealth.Get() + 100 );
    }
    [MenuItem( "Debug/DeleteAllPlayerData/NoSeriouslyDeleteItAll" )]
    public static void DeleteAllPlayerData()
    {
        _instance = new PlayerData();
        _instance.SetDirty();
    }
#endif

    // NON STATIC
    private bool dirty = false;
    ~PlayerData()
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
        PlayerData.Instance?.SetDirty();
    }
}

[System.Serializable]
public class PlayerUpgradeUnlockMap : ISerializationCallbackReceiver
{
    private Dictionary<PlayerData.UpgradeFlags, bool> unlock_map = new Dictionary<PlayerData.UpgradeFlags, bool>();

    [SerializeField] List<string> serialized_unlock_flags;

    private static Dictionary<string, PlayerData.UpgradeFlags> valid_enum_strings;

    public bool GetUnlock( PlayerData.UpgradeFlags flag )
    {
        return unlock_map[flag];
    }

    public void SetUnlock( PlayerData.UpgradeFlags flag, bool value )
    {
        PlayerData.Instance?.SetDirty();
        unlock_map[flag] = value;
        PlayerData.Instance?.UpgradeFlagChangedEvent.Invoke( flag, value );
    }

    public PlayerUpgradeUnlockMap()
    {
        foreach( PlayerData.UpgradeFlags flag in Enum.GetValues( typeof( PlayerData.UpgradeFlags ) ) )
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
            valid_enum_strings = new Dictionary<string, PlayerData.UpgradeFlags>();
            foreach( PlayerData.UpgradeFlags flag in Enum.GetValues( typeof( PlayerData.UpgradeFlags ) ) )
            {
                valid_enum_strings.Add( flag.ToString(), flag );
            }
        }

        // add an entry for every possible unlock flag
        foreach( PlayerData.UpgradeFlags flag in Enum.GetValues( typeof( PlayerData.UpgradeFlags ) ) )
        {
            if( !unlock_map.ContainsKey( flag ) )
                unlock_map.Add( flag, false );
        }

        // set valid entries
        if( serialized_unlock_flags != null )
        {
            foreach( string key in serialized_unlock_flags )
            {
                PlayerData.UpgradeFlags out_flag;
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