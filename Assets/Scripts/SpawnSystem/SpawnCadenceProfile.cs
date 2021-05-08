using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu( fileName = "SpawnCadenceProfile", menuName = "ScriptableObjects/SpawnCadenceProfile", order = 0 )]
public class SpawnCadenceProfile : BaseSpawnCadenceProfile
{
    public string LevelIdentifier = "CHANGE_ME";
    public Challenge LevelChallenge;
    public List<SpawnWave> Waves;

    public override Challenge GetChallenge()
    {
        return LevelChallenge;
    }

    public override string GetLevelIdentifier()
    {
        return LevelIdentifier;
    }

    public override string GetName()
    {
        return name;
    }

    public override SpawnWave GetWave(int wave_number)
    {
        Debug.Assert( wave_number < Waves.Count, "ERROR: Requesting wave beyond index of current spawn cadence profile" );
        return Waves.Count < wave_number ? Waves[wave_number] : null;
    }

    public override int GetWaveCount()
    {
        return Waves.Count;
    }
}

#if UNITY_EDITOR
[CustomEditor( typeof( SpawnCadenceProfile ) )]
public class SpawnCadenceProfileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SpawnCadenceProfile spawn_cadence_profile = (SpawnCadenceProfile)target;

        // Level Identifier
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( "Level Identifier" );
        string level_identifier = GUILayout.TextField( spawn_cadence_profile.LevelIdentifier );
        if( level_identifier != spawn_cadence_profile.LevelIdentifier )
        {
            EditorUtility.SetDirty( target );
            spawn_cadence_profile.LevelIdentifier = level_identifier;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( "Level Challenge" );
        Challenge challenge = (Challenge)EditorGUILayout.ObjectField( spawn_cadence_profile.LevelChallenge, typeof( Challenge ), true );
        if( spawn_cadence_profile.LevelChallenge != challenge )
        {
            spawn_cadence_profile.LevelChallenge = challenge;
            EditorUtility.SetDirty( target );
        }
        EditorGUILayout.EndHorizontal();

        // Waves
        if( spawn_cadence_profile.Waves == null )
        {
            spawn_cadence_profile.Waves = new List<SpawnWave>();
        }

        if( GUILayout.Button( "Add Wave" ) )
        {
            spawn_cadence_profile.Waves.Add( new SpawnWave() );
            EditorUtility.SetDirty( target );
        }

        int wave_num = 1;
        for( int index = 0; index < spawn_cadence_profile.Waves.Count; ++index )
        {
            SpawnWave wave = spawn_cadence_profile.Waves[index];
            bool c = EditorGUILayout.BeginFoldoutHeaderGroup( spawn_cadence_profile.Waves[index].collapsed, "------WAVE " + ( wave_num ).ToString() + "------" );
            if( c != spawn_cadence_profile.Waves[index].collapsed )
            {
                spawn_cadence_profile.Waves[index].collapsed = c;
                EditorUtility.SetDirty( target );
            }

            if( !c )
            {
                // Wave Completion Reward
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label( "Wave Completion Reward" );
                int completion_reward = EditorGUILayout.IntField( wave.CompletionReward );
                if( completion_reward != wave.CompletionReward )
                {
                    EditorUtility.SetDirty( target );
                    wave.CompletionReward = completion_reward;
                }
                EditorGUILayout.EndHorizontal();

                // Animation Trigger
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label( "Animation Trigger" );
                string animation_trigger = EditorGUILayout.TextField( wave.AnimationTrigger );
                if( animation_trigger != wave.AnimationTrigger )
                {
                    EditorUtility.SetDirty( target );
                    wave.AnimationTrigger = animation_trigger;
                }
                EditorGUILayout.EndHorizontal();

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
                    spawn_cadence_profile.Waves.Insert( index + 1, new SpawnWave( wave ) );
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