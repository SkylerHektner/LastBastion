using System;
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
    [SerializeField] public float ClusterDensity = 2.0f; // per meter squared
    [SerializeField] public float SpawnStaggerMinTime = 0.02f;
    [SerializeField] public float SpawnStaggerMaxTime = 0.07f;

    [System.Serializable]
    public enum Layout
    {
        Cluster,
        Spread,
        Door,
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
            float cluster_density = EditorGUILayout.FloatField( spawn_group.ClusterDensity );
            if( cluster_density != spawn_group.ClusterDensity )
            {
                if( cluster_density <= 0.0f ) cluster_density = 0.1f;
                spawn_group.ClusterDensity = cluster_density;
                EditorUtility.SetDirty( target );
            }
            EditorGUILayout.EndHorizontal();
        }

        // spawn stagger times
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( "Spawn Stagger Min Time" );
            float spawn_stagger_min_time = EditorGUILayout.FloatField( spawn_group.SpawnStaggerMinTime );
            if( spawn_stagger_min_time != spawn_group.SpawnStaggerMinTime )
            {
                if( spawn_stagger_min_time <= 0.0f ) spawn_stagger_min_time = 0.0f;
                spawn_group.SpawnStaggerMinTime = spawn_stagger_min_time;
                EditorUtility.SetDirty( target );
            }
            EditorGUILayout.EndHorizontal();
        }
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( "Spawn Stagger Max Time" );
            float spawn_stagger_max_time = EditorGUILayout.FloatField( spawn_group.SpawnStaggerMaxTime );
            if( spawn_stagger_max_time != spawn_group.SpawnStaggerMaxTime )
            {
                if( spawn_stagger_max_time <= 0.0f ) spawn_stagger_max_time = 0.0f;
                spawn_group.SpawnStaggerMaxTime = spawn_stagger_max_time;
                EditorUtility.SetDirty( target );
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif

[Serializable] public class SpawnDictionary : SerializableDictionary<EnemyEnum, int> { }