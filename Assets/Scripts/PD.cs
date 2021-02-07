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
    public PlayerUpgradeUnlockMap UpgradeUnlockMap = new PlayerUpgradeUnlockMap();

    // UPGRADES
    public enum UpgradeFlags
    {
        ChainLightning,                  
        ChainLightningStunDuration,      
        ChainLightningStaticOverload,    // not implemented
        ChainLightningLightningRod,      
        Typhoon,                         
        TyphoonExtendedBBQ,              // not implemented
        TyphoonFlameSaw,                 // not implemented
        TyphoonRoaringFlames,            // not implemented
        Anomaly,                         
        AnomalyRicochetSaws,             
        AnomalyStasisCoating,            // not implemented
        AnomalySingularity,              // not implemented
        Sawmageddon,                     
        SawmageddonDuration,             
        SawmageddonProjectiles,          
        SawmageddonComboKiller,          // not implemented
        BaseOvershield,                  // not implemented
        SoulCollector,                   // not implemented
        BaseHP1,                         
        BaseHP2,                         
        BaseHP3,                         
        Turrets1,                        // not implemented
        Turrets2,                        // not implemented
        Turrets3,                        // not implemented

    }

    // EVENTS
    [NonSerialized] public UnityEvent<UpgradeFlags, bool> UpgradeFlagChangedEvent = new UnityEvent<UpgradeFlags, bool>();

    // STATICS
    public static PD Instance {
        get {
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
        _instance = new PD();
        _instance.SetDirty();
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
        PD.Instance?.SetDirty();
        unlock_map[flag] = value;
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