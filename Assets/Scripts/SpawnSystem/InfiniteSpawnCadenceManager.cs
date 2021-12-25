using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteSpawnCadenceManager : MonoBehaviour
{
    public static InfiniteSpawnCadenceManager Instance;

    [SerializeField] int NumWavesPerEnvironmentSwap = 5;
    [SerializeField] List<InfiniteSpawnCadenceProfile> SpawnCadences = new List<InfiniteSpawnCadenceProfile>();
    public SpawnManager spawnManager;
    public SurvivalCardsUI survivalCardsUI;

    private InfiniteSpawnCadenceProfile cur_spawn_cadence = null;
    private Environment cur_environment = null;
    private Dictionary<InfiniteSpawnCadenceProfile, int> picked_tracker = new Dictionary<InfiniteSpawnCadenceProfile, int>();

    private void Start()
    {
        Instance = this;

        if( SpawnCadences.Count <= 1 )
        {
            Debug.LogError( "ERROR: Infinite Spawn Cadence Manager is designed to work with at least 2 cadences" );
        }

        // initialize picked tracker
        foreach( InfiniteSpawnCadenceProfile profile in SpawnCadences )
            picked_tracker.Add( profile, 1 );

        spawnManager.WaveCompletedEvent.AddListener( OnWaveCompleted );

        if( PD.Instance.SurvivalLimboResumeInformation.Active )
        {
            SetCurrentSpawnCadenceByIndex( PD.Instance.SurvivalLimboResumeInformation.SurvivalSpawnCadenceIndex );
        }
        else
        {
            PickNewSpawnCadenceProfile();
        }
    }

    private void OnDestroy()
    {
        // idk, be polite
        spawnManager?.WaveCompletedEvent.RemoveListener( OnWaveCompleted );
    }

    public void OnWaveCompleted( int wave_number )
    {
        if( wave_number % NumWavesPerEnvironmentSwap == 0 )
        {
            SpawnManager.Instance.DeferNextWaveStart();
            survivalCardsUI.ShowUpgrades();
        }
    }

    public int GetCurrentSpawnCadenceIndex()
    {
        int ret = 0;
        for( int x = 0; x < SpawnCadences.Count; ++x )
        {
            if( SpawnCadences[x] == cur_spawn_cadence )
            {
                ret = x;
                break;
            }
        }
        return ret;
    }

    public void SetCurrentSpawnCadenceByIndex( int index )
    {
        if( index > SpawnCadences.Count )
        {
            Debug.LogError( $"ERROR: Given index {index} is greater then number of spawn cadences {SpawnCadences.Count}" );
            PickNewSpawnCadenceProfile();
        }
        else
        {
            cur_spawn_cadence = SpawnCadences[index];
            InitializeCurrentSpawnCadenceProfile();
        }
    }

    private void InitializeCurrentSpawnCadenceProfile()
    {
        if( cur_environment != null )
        {
            Destroy( cur_environment.gameObject );
        }

        cur_environment = Instantiate( cur_spawn_cadence.CadenceEnvironment );
        GameplayManager.Instance.ActiveEnvironment = cur_environment;
        spawnManager.SetNewSpawnCadence( cur_spawn_cadence );
    }

    public void PickNewSpawnCadenceProfile()
    {
        WeightedSelector<InfiniteSpawnCadenceProfile> profile_selector = new WeightedSelector<InfiniteSpawnCadenceProfile>();
        foreach( InfiniteSpawnCadenceProfile profile in SpawnCadences )
        {
            if( profile != cur_spawn_cadence )
            {
                profile_selector.AddItem( profile, (int)( ( 1.0f / (float)picked_tracker[profile] ) * 100000.0f ) );
            }
        }
        Debug.Assert( profile_selector.HasItem() );

        InfiniteSpawnCadenceProfile picked_profile = profile_selector.GetItem();
        PD.Instance.LevelCompletionMap.SetLevelCompletion(picked_profile.CadenceEnvironment.EnvironmentID, true); // if you encounter a survival environment, track it
        picked_tracker[picked_profile] += 1;

        cur_spawn_cadence = picked_profile;

        InitializeCurrentSpawnCadenceProfile();
    }
}
