using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using UnityEditor;

public class PlayerData
{
    // DATA
    public PlayerDataField<int> PlayerWealth = new PlayerDataField<int>();
    public PlayerDataField<bool> Limbo = new PlayerDataField<bool>();
    public PlayerDataField<string> ExitedScene = new PlayerDataField<string>();

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

        string data = JsonUtility.ToJson( this );
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
