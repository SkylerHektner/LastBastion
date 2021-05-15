using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteSpawnCadenceManager : MonoBehaviour
{
    [SerializeField] int NumWavesPerEnvironmentSwap = 5;
    [SerializeField] List<InfiniteSpawnCadenceProfile> SpawnCadences = new List<InfiniteSpawnCadenceProfile>();
    public SpawnManager spawnManager;

    private InfiniteSpawnCadenceProfile cur_spawn_cadence = null;
    private Environment cur_environment = null;
    private Dictionary<InfiniteSpawnCadenceProfile, int> picked_tracker = new Dictionary<InfiniteSpawnCadenceProfile, int>();

    private void Start()
    {
        if( SpawnCadences.Count <= 1 )
        {
            Debug.LogError( "ERROR: Infinite Spawn Cadence Manager is designed to work with at least 2 cadences" );
        }

        // initialize picked tracker
        foreach( InfiniteSpawnCadenceProfile profile in SpawnCadences )
            picked_tracker.Add( profile, 1 );

        spawnManager.WaveCompletedEvent.AddListener( OnWaveCompleted );

        PickNewSpawnCadenceProfile();
    }

    private void OnDestroy()
    {
        // idk, be polite
        spawnManager?.WaveCompletedEvent.RemoveListener( OnWaveCompleted );
    }

    public void OnWaveCompleted(int wave_number)
    {
        if( wave_number % NumWavesPerEnvironmentSwap == 0 )
        {
            // TODO: ADD ME BACK IN ONCE THIS IS NOT INSTANT
            //SpawnManager.Instance.DeferNextWaveStart();

            PickNewSpawnCadenceProfile();
        }
    }

    private void PickNewSpawnCadenceProfile()
    {
        WeightedSelector<InfiniteSpawnCadenceProfile> profile_selector = new WeightedSelector<InfiniteSpawnCadenceProfile>();
        foreach( InfiniteSpawnCadenceProfile profile in SpawnCadences )
        {
            if( profile != cur_spawn_cadence )
            {
                profile_selector.AddItem( profile, (int)( ( 1.0f / (float)picked_tracker[profile] ) * 100.0f ) );
            }
        }
        Debug.Assert( profile_selector.HasItem() );

        InfiniteSpawnCadenceProfile picked_profile = profile_selector.GetItem();
        picked_tracker[picked_profile] += 1;

        cur_spawn_cadence = picked_profile;

        if( cur_environment != null )
        {
            Destroy( cur_environment.gameObject );
        }

        cur_environment = Instantiate( cur_spawn_cadence.CadenceEnvironment );
        GameplayManager.Instance.ActiveEnvironment = cur_environment;
        spawnManager.SetNewSpawnCadence( cur_spawn_cadence );

        // TODO: ADD ME BACK IN ONCE THIS IS NOT INSTANT
        // SpawnManager.Instance.StartNextWave();
    }
}
