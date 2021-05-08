using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu( fileName = "InfiniteSpawnCadenceProfile", menuName = "ScriptableObjects/InfiniteSpawnCadenceProfile", order = 0 )]
public class InfiniteSpawnCadenceProfile : BaseSpawnCadenceProfile
{
    [System.Serializable]
    public class InfiniteSpawnGroup
    {
        public SpawnGroup SG;
        public int StartWave;
        public int SpawnWeighting;


        // Editor UI Only
        [System.NonSerialized] public bool collapsed = false;
        [System.NonSerialized] public bool deleting = false;
    }

    public int NumStartSpawns;
    public int NumAdditionalSpawnsPerWave;
    public float BaseWaveDuration;
    public float AdditionalWaveDurationPerSpawn;
    public int WavesPerUnlockFlag;
    public int WavesPerChallengeModifier;
    public List<InfiniteSpawnGroup> SpawnGroupData = new List<InfiniteSpawnGroup>();

    private Dictionary<int, SpawnWave> SpawnWaveCache = new Dictionary<int, SpawnWave>();

    public override SpawnWave GetWave( int wave_number )
    {
        if( !SpawnWaveCache.ContainsKey( wave_number ) )
        {
            SpawnWave wave = new SpawnWave();

            int num_spawn_groups = NumStartSpawns + NumAdditionalSpawnsPerWave * wave_number;
            float wave_duration = BaseWaveDuration + AdditionalWaveDurationPerSpawn * num_spawn_groups;
            float delay_between_spawns = wave_duration / num_spawn_groups;

            for( int x = 0; x < num_spawn_groups; ++x )
            {
                WeightedSelector<SpawnGroup> group_selector = new WeightedSelector<SpawnGroup>();
                foreach( InfiniteSpawnGroup group in SpawnGroupData )
                {
                    if( group.StartWave - 1 <= wave_number )
                    {
                        group_selector.AddItem( group.SG, group.SpawnWeighting );
                    }
                }

                if( group_selector.HasItem() )
                {
                    wave.SpawnGroups.Add( group_selector.GetItem() );
                    wave.SpawnGroupSpawnTimes.Add( delay_between_spawns * x );
                }
                else
                {
                    Debug.LogError( $"CRITICAL ERROR: Infinite spawn cadence has no valid spawn groups for wave {wave_number}" );
                }
            }

            SpawnWaveCache[wave_number] = wave;
        }

        return SpawnWaveCache[wave_number];
    }

    public override int GetWaveCount()
    {
        return int.MaxValue;
    }

    public override string GetName()
    {
        return name;
    }

    public override string GetLevelIdentifier()
    {
        return "INFINITE_LEVEL"; // TODO: Change to something sensible?
    }

    public override Challenge GetChallenge()
    {
        return null;
    }
}

#if UNITY_EDITOR
[CustomEditor( typeof( InfiniteSpawnCadenceProfile ) )]
public class InfiniteSpawnCadenceProfileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InfiniteSpawnCadenceProfile InfiniteSpawnCadence = (InfiniteSpawnCadenceProfile)target;

        CustomEditorUtilities.AutoDirtyLabeledInt( ref InfiniteSpawnCadence.NumStartSpawns, "Base Number of Spawns", target );
        CustomEditorUtilities.AutoDirtyLabeledInt( ref InfiniteSpawnCadence.NumAdditionalSpawnsPerWave, "Additional Spawns Per Wave", target );
        CustomEditorUtilities.AutoDirtyLabeledFloat( ref InfiniteSpawnCadence.BaseWaveDuration, "Base Wave Duration", target );
        CustomEditorUtilities.AutoDirtyLabeledFloat( ref InfiniteSpawnCadence.AdditionalWaveDurationPerSpawn, "Additional Wave Duration Per Spawn", target );
        CustomEditorUtilities.AutoDirtyLabeledInt( ref InfiniteSpawnCadence.WavesPerUnlockFlag, "Waves Per Unlock Flag", target );
        CustomEditorUtilities.AutoDirtyLabeledInt( ref InfiniteSpawnCadence.NumAdditionalSpawnsPerWave, "Waves Per Challenge Modifier", target );

        EditorGUILayout.LabelField( "------ SPAWN GROUP DATA ------" );
        if( GUILayout.Button( "Add New" ) )
        {
            InfiniteSpawnCadenceProfile.InfiniteSpawnGroup p = new InfiniteSpawnCadenceProfile.InfiniteSpawnGroup();
            InfiniteSpawnCadence.SpawnGroupData.Add( p );
            EditorUtility.SetDirty( target );
        }

        // collapse controls
        EditorGUILayout.BeginHorizontal();
        if( GUILayout.Button( "Collapse All" ) )
        {
            foreach( var g in InfiniteSpawnCadence.SpawnGroupData )
                g.collapsed = true;
        }
        else if( GUILayout.Button( "Show All" ) )
        {
            foreach( var g in InfiniteSpawnCadence.SpawnGroupData )
                g.collapsed = false;
        }
        EditorGUILayout.EndHorizontal();

        // show each spawn group data
        for( int x = 0; x < InfiniteSpawnCadence.SpawnGroupData.Count; ++x )
        {
            InfiniteSpawnCadenceProfile.InfiniteSpawnGroup g = InfiniteSpawnCadence.SpawnGroupData[x];

            CustomEditorUtilities.AutoDirtyFoldoutHeaderGroup( ref g.collapsed, "------" + ( g.SG != null ? g.SG.name : "(unset)" ) + "------", target );

            if( g.collapsed )
                continue;

            // spawn group
            g.SG = (SpawnGroup)CustomEditorUtilities.AutoDirtyUnityObject( g.SG, typeof( SpawnGroup ), "Spawn Group", target );

            // Start Wave
            CustomEditorUtilities.AutoDirtyLabeledInt( ref g.StartWave, "Start Wave", target );

            // Spawn Weighting
            CustomEditorUtilities.AutoDirtyLabeledInt( ref g.SpawnWeighting, "Spawn Weighting", target );

            // Modify Entry Buttons
            CustomEditorUtilities.ListItemControlButtons<InfiniteSpawnCadenceProfile.InfiniteSpawnGroup>( InfiniteSpawnCadence.SpawnGroupData, ref x, ref g.deleting, target );
        }
    }
}
#endif