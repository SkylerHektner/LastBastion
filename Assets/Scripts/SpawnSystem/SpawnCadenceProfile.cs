using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu( fileName = "SpawnCadenceProfile", menuName = "ScriptableObjects/SpawnCadenceProfile", order = 0 )]
public class SpawnCadenceProfile : ScriptableObject
{
    public List<Wave> Waves;

    [Serializable]
    public class Wave
    {
        public List<EnemyEnum> PassiveEnemies;
        public List<float> PassiveEnemySpawnCadence;
        public List<SpawnGroup> SpawnGroups;
        public List<float> SpawnGroupSpawnTimes;
        public bool collapsed = false; // Editor UI Only

        public Wave()
        {
            PassiveEnemies = new List<EnemyEnum>();
            PassiveEnemySpawnCadence = new List<float>();
            SpawnGroups = new List<SpawnGroup>();
            SpawnGroupSpawnTimes = new List<float>();
        }

        public Wave( Wave other )
        {
            PassiveEnemies = new List<EnemyEnum>();
            PassiveEnemySpawnCadence = new List<float>();
            SpawnGroups = new List<SpawnGroup>();
            SpawnGroupSpawnTimes = new List<float>();

            foreach( var e in other.PassiveEnemies )
                PassiveEnemies.Add( e );
            foreach( var e in other.PassiveEnemySpawnCadence )
                PassiveEnemySpawnCadence.Add( e );
            foreach( var e in other.SpawnGroups )
                SpawnGroups.Add( e );
            foreach( var e in other.SpawnGroupSpawnTimes )
                SpawnGroupSpawnTimes.Add( e );
        }
    }
}

#if UNITY_EDITOR
[CustomEditor( typeof( SpawnCadenceProfile ) )]
public class SpawnCadenceProfileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SpawnCadenceProfile spawn_cadence_profile = (SpawnCadenceProfile)target;

        if( spawn_cadence_profile.Waves == null )
        {
            spawn_cadence_profile.Waves = new List<SpawnCadenceProfile.Wave>();
        }

        if( GUILayout.Button( "Add Wave" ) )
        {
            spawn_cadence_profile.Waves.Add( new SpawnCadenceProfile.Wave() );
            EditorUtility.SetDirty( target );
        }

        int wave_num = 1;
        for( int index = 0; index < spawn_cadence_profile.Waves.Count; ++index )
        {
            SpawnCadenceProfile.Wave wave = spawn_cadence_profile.Waves[index];
            bool c = EditorGUILayout.BeginFoldoutHeaderGroup( spawn_cadence_profile.Waves[index].collapsed, "------WAVE " + ( wave_num ).ToString() + "------" );
            if( c != spawn_cadence_profile.Waves[index].collapsed )
            {
                spawn_cadence_profile.Waves[index].collapsed = c;
                EditorUtility.SetDirty( target );
            }

            if( !c )
            {
                // passive enemy spawn rates - add new entry
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label( "Passive Enemy Spawn Rates" );
                if( GUILayout.Button( "+" ) )
                {
                    wave.PassiveEnemies.Add( EnemyEnum.Skeleton );
                    wave.PassiveEnemySpawnCadence.Add( 0.0f );
                    EditorUtility.SetDirty( target );
                }
                EditorGUILayout.EndHorizontal();

                // edit passive enemy spawn rates
                for( int x = 0; x < wave.PassiveEnemies.Count; ++x )
                {

                    EditorGUILayout.BeginHorizontal();
                    EnemyEnum enemy = (EnemyEnum)EditorGUILayout.EnumPopup( wave.PassiveEnemies[x] );
                    if( enemy != wave.PassiveEnemies[x] )
                    {
                        wave.PassiveEnemies[x] = enemy;
                        EditorUtility.SetDirty( target );
                    }
                    float enemy_spawn_cadence = EditorGUILayout.DelayedFloatField( wave.PassiveEnemySpawnCadence[x] );
                    if( enemy_spawn_cadence != wave.PassiveEnemySpawnCadence[x] )
                    {
                        wave.PassiveEnemySpawnCadence[x] = enemy_spawn_cadence;
                        EditorUtility.SetDirty( target );
                    }
                    GUILayout.Label( "Per Second" );
                    if( GUILayout.Button( "Copy" ) )
                    {
                        wave.PassiveEnemies.Insert( x + 1, wave.PassiveEnemies[x] );
                        wave.PassiveEnemySpawnCadence.Insert( x + 1, wave.PassiveEnemySpawnCadence[x] );
                        EditorUtility.SetDirty( target );
                    }
                    if( GUILayout.Button( "Delete" ) )
                    {
                        wave.PassiveEnemies.RemoveAt( x );
                        wave.PassiveEnemySpawnCadence.RemoveAt( x );
                        x--;
                        EditorUtility.SetDirty( target );
                    }
                    EditorGUILayout.EndHorizontal();
                }

                // SpawnGroups
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label( "Spawn Groups" );
                if( GUILayout.Button( "+" ) )
                {
                    wave.SpawnGroups.Add( null );
                    wave.SpawnGroupSpawnTimes.Add( 0.0f );
                    EditorUtility.SetDirty( target );
                }
                EditorGUILayout.EndHorizontal();
                for( int x = 0; x < wave.SpawnGroups.Count; ++x )
                {
                    EditorGUILayout.BeginHorizontal();
                    SpawnGroup group = (SpawnGroup)EditorGUILayout.ObjectField( wave.SpawnGroups[x], typeof( SpawnGroup ), true );
                    if( wave.SpawnGroups[x] != group )
                    {
                        wave.SpawnGroups[x] = group;
                        EditorUtility.SetDirty( target );
                    }
                    GUILayout.Label( "Spawn At Wave Time: " );
                    float group_spawn_time = EditorGUILayout.FloatField( wave.SpawnGroupSpawnTimes[x] );
                    if( group_spawn_time != wave.SpawnGroupSpawnTimes[x] )
                    {
                        wave.SpawnGroupSpawnTimes[x] = group_spawn_time;
                        EditorUtility.SetDirty( target );
                    }
                    if( GUILayout.Button( "Copy" ) )
                    {
                        wave.SpawnGroups.Insert( x + 1, wave.SpawnGroups[x] );
                        wave.SpawnGroupSpawnTimes.Insert( x + 1, wave.SpawnGroupSpawnTimes[x] );
                        EditorUtility.SetDirty( target );
                    }
                    if( GUILayout.Button( "Delete" ) )
                    {
                        wave.SpawnGroups.RemoveAt( x );
                        wave.SpawnGroupSpawnTimes.RemoveAt( x );
                        x--;
                        EditorUtility.SetDirty( target );
                    }
                    EditorGUILayout.EndHorizontal();
                }

                // delete or duplicate entry
                EditorGUILayout.BeginHorizontal();
                if( GUILayout.Button( "Duplicate Wave" ) )
                {
                    spawn_cadence_profile.Waves.Insert( index + 1, new SpawnCadenceProfile.Wave( wave ) );
                    EditorUtility.SetDirty( target );
                }
                if( GUILayout.Button( "Delete Wave" ) )
                {
                    spawn_cadence_profile.Waves.RemoveAt( index );
                    index--;
                    EditorUtility.SetDirty( target );
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            wave_num++;
        }
    }
}
#endif