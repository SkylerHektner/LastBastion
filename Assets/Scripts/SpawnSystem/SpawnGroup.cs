using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

[CustomEditor(typeof(SpawnGroup))]
public class SpawnGroupEdtior : Editor
{
    public override void OnInspectorGUI()
    {
        SpawnGroup spawn_group = (SpawnGroup)target;

        if( spawn_group.SpawnMap == null  || spawn_group.SpawnMap.Count == 0)
        {
            spawn_group.SpawnMap = new SpawnDictionary();
            foreach( var e in Enum.GetValues( typeof( EnemyEnum ) ) )
                spawn_group.SpawnMap[(EnemyEnum)e] = 0;
        }

        foreach (var e in Enum.GetValues(typeof(EnemyEnum)))
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
            EditorGUILayout.EndHorizontal();
        }

        SpawnGroup.Layout layout = (SpawnGroup.Layout)EditorGUILayout.EnumPopup( "Layout: ", spawn_group.layout );
        if( layout != spawn_group.layout )
        {
            spawn_group.layout = layout;
            EditorUtility.SetDirty( target );
        }

        if(layout == SpawnGroup.Layout.Cluster)
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