﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CreateAssetMenu( fileName = "SpawnGroup", menuName = "ScriptableObjects/SpawnGroup", order = 1 )]
[System.Serializable]
public class SpawnGroup : ScriptableObject
{
    [SerializeField] public SpawnDictionary SpawnMap;
    [SerializeField] public Layout layout;
    [SerializeField] public float cluster_density = 2.0f; // per meter squared

    [System.Serializable]
    public enum Layout
    {
        Cluster,
        Spread,
    }
}

#if UNITY_EDITOR
[CustomEditor( typeof( SpawnGroup ) )]
public class SpawnGroupEdtior : Editor
{
    public override void OnInspectorGUI()
    {
        SpawnGroup spawn_group = (SpawnGroup)target;
        var enemy_enum_values = Enum.GetValues( typeof( EnemyEnum ) ).Cast<EnemyEnum>();

        // no spawnmap exists
        if( spawn_group.SpawnMap == null )
        {
            spawn_group.SpawnMap = new SpawnDictionary();
            EditorUtility.SetDirty( target );
        }
        // new enemy types have been added
        else if( spawn_group.SpawnMap.Count < enemy_enum_values.Count() )
        {
            foreach( var e in enemy_enum_values )
                if( !spawn_group.SpawnMap.ContainsKey( e ) )
                    spawn_group.SpawnMap[e] = 0;
            EditorUtility.SetDirty( target );
        }
        // enemy types have been removed
        else if( spawn_group.SpawnMap.Count > enemy_enum_values.Count() )
        {
            var old_spawn_map = spawn_group.SpawnMap;
            spawn_group.SpawnMap = new SpawnDictionary();
            foreach(var e in enemy_enum_values )
            {
                spawn_group.SpawnMap[e] = old_spawn_map[e];
            }
            EditorUtility.SetDirty( target );
        }

        foreach( var e in Enum.GetValues( typeof( EnemyEnum ) ) )
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( e.ToString() + ": " + spawn_group.SpawnMap[(EnemyEnum)e].ToString() );
            if( GUILayout.Button( "+", GUILayout.Width( 25 ) ) )
            {
                spawn_group.SpawnMap[(EnemyEnum)e]++;
                EditorUtility.SetDirty( target );
            }
            else if( GUILayout.Button( "-", GUILayout.Width( 25 ) ) && spawn_group.SpawnMap[(EnemyEnum)e] >= 1 )
            {
                spawn_group.SpawnMap[(EnemyEnum)e]--;
                EditorUtility.SetDirty( target );
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        SpawnGroup.Layout layout = (SpawnGroup.Layout)EditorGUILayout.EnumPopup( "Layout: ", spawn_group.layout );
        if( layout != spawn_group.layout )
        {
            spawn_group.layout = layout;
            EditorUtility.SetDirty( target );
        }

        if( layout == SpawnGroup.Layout.Cluster )
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( "Monster Per Meter^2" );
            float cluster_density = EditorGUILayout.FloatField( spawn_group.cluster_density );
            if( cluster_density != spawn_group.cluster_density )
            {
                if( cluster_density <= 0.0f ) cluster_density = 0.1f;
                spawn_group.cluster_density = cluster_density;
                EditorUtility.SetDirty( target );
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif

[Serializable] public class SpawnDictionary : SerializableDictionary<EnemyEnum, int> { }

[Serializable]
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