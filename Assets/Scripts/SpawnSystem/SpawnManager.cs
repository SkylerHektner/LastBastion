using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Events;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] SpawnCadenceProfile spawnCadenceProfile;
    [SerializeField] int DebugStartWave = 0;
    [Header( "Enemy Prefabs" )]
    [SerializeField] GameObject SkeletonPrefab;
    [SerializeField] GameObject ShieldSkeletonPrefab;
    [SerializeField] GameObject PumpkinWarriorPrefab;
    [SerializeField] GameObject PumpKINGPrefab;
    [SerializeField] GameObject BolterPrefab;
    [SerializeField] GameObject MudSlingerPrefab;
    [SerializeField] GameObject ShrikePrefab;
    [SerializeField] GameObject ShamanPrefab;
    [SerializeField] GameObject CarrierLPrefab;
    [SerializeField] GameObject CarrierMPrefab;
    [SerializeField] GameObject CarrierSPrefab;
    [SerializeField] GameObject RedSkeletonPrefab;
    [Header( "Spawn Area Information" )]
    public Vector3 SpawnableAreaTopRight;
    public Vector3 SpawnableAreaBottomLeft;
    public Vector3 PlayableAreaTopRight;
    public Vector3 PlayableAreaBottomLeft;
    public List<Vector3> DoorSpawnPoints = new List<Vector3>();
    [Header( "Misc" )]
    [SerializeField] WaveCounter WaveCounterUI;

    private float spawn_timer = -1.0f;
    private int current_wave = -1;
    private int cur_spawn_group_index = 0;
    private List<float> passive_spawn_trackers = new List<float>();
    private LinkedList<PendingSpawn> pending_spawns = new LinkedList<PendingSpawn>();
    private Dictionary<long, Enemy> spawned_enemies = new Dictionary<long, Enemy>();
    public List<Enemy> AllSpawnedEnemies { get { return spawned_enemies.Values.ToList<Enemy>(); } }
    public UnityEvent<Enemy> EnemySpawnedEvent = new UnityEvent<Enemy>();
    public List<Animator> AnimationTriggerListeners = new List<Animator>();
    struct PendingSpawn
    {
        public float time_left;
        public EnemyEnum enemy;
        public Vector3 position;
        public PendingSpawn( float _time_left, EnemyEnum _enemy, Vector3 _position )
        {
            time_left = _time_left;
            enemy = _enemy;
            position = _position;
        }
    }

    private void Start()
    {
        Instance = this;
#if UNITY_EDITOR
        current_wave = DebugStartWave - 2;
#endif
        StartNextWave();
    }

    private void Update()
    {
        //manage pending spawns
        if( pending_spawns.Count != 0 )
        {
            for( var it = pending_spawns.First; it != null; )
            {
                var next = it.Next;
                var ps = it.Value;
                ps.time_left -= Time.deltaTime;
                it.Value = ps;
                if( ps.time_left < 0.0f )
                {
                    SpawnMonster( ps.enemy, ps.position );
                    pending_spawns.Remove( ps );
                }
                it = next;
            }
        }

        if( spawn_timer != -1.0f )
        {
            // increment spawn timer
            spawn_timer += Time.deltaTime;

            // manage passive mob spawns
            // NO PASSIVE MOBS SPAWN AFTER LAST SPAWN GROUP HAS BEEN TRIGGERED
            if( cur_spawn_group_index < spawnCadenceProfile.Waves[current_wave].SpawnGroups.Count )
            {
                for( int x = 0; x < passive_spawn_trackers.Count; ++x )
                {
                    passive_spawn_trackers[x] -= Time.deltaTime;
                    if( passive_spawn_trackers[x] < 0.0f )
                    {
                        passive_spawn_trackers[x] += spawnCadenceProfile.Waves[current_wave].PassiveEnemySpawnCadence[x];
                        SpawnMonster( spawnCadenceProfile.Waves[current_wave].PassiveEnemies[x], GetRandomSpawnPoint() );
                    }
                }
            }

            // manage spawn groups
            while( cur_spawn_group_index < spawnCadenceProfile.Waves[current_wave].SpawnGroups.Count &&
                spawn_timer > spawnCadenceProfile.Waves[current_wave].SpawnGroupSpawnTimes[cur_spawn_group_index] )
            {

                SpawnSpawnGroup( spawnCadenceProfile.Waves[current_wave].SpawnGroups[cur_spawn_group_index] );
                cur_spawn_group_index++;
            }

            if( cur_spawn_group_index == spawnCadenceProfile.Waves[current_wave].SpawnGroups.Count
                && pending_spawns.Count == 0
                && spawned_enemies.Count == 0 )
            {
                WaveComplete();
            }
        }
    }

    public void StartNextWave()
    {
        spawn_timer = 0.0f;
        current_wave++;
        WaveCounterUI?.ShowNextWave( current_wave + 1 );
        cur_spawn_group_index = 0;
        passive_spawn_trackers.Clear();
        if( !string.IsNullOrEmpty( spawnCadenceProfile.Waves[current_wave].AnimationTrigger ) )
        {
            var triggers = spawnCadenceProfile.Waves[current_wave].AnimationTrigger.Split( ',' ).Select( s => s.Trim() );
            foreach( Animator anim in AnimationTriggerListeners )
            {
                foreach( string trigger in triggers )
                {
                    if( anim.parameters.Any( p => p.name == trigger ) )
                    {
                        anim.SetTrigger( trigger );
                    }
                }
            }
        }
        foreach( float time in spawnCadenceProfile.Waves[current_wave].PassiveEnemySpawnCadence )
        {
            if( time == 0.0f )
            {
                Debug.LogError( "ERROR: Passive Spawn Cadence set to 0 in Spawn Cadence Profile " + spawnCadenceProfile.name + " Wave " + ( current_wave + 1 ).ToString() );
                continue;
            }
            passive_spawn_trackers.Add( 1.0f / time );
        }
    }

    private void WaveComplete()
    {
        // if this is the first time completing this wave, mark it as complete and grant the player a reward
        if( !PD.Instance.LevelCompletionMap.GetWaveCompletion( spawnCadenceProfile.LevelIdentifier, current_wave ) )
        {
            PD.Instance.LevelCompletionMap.SetWaveCompletion( spawnCadenceProfile.LevelIdentifier, current_wave, true );
            PD.Instance.PlayerWealth.Set( PD.Instance.PlayerWealth.Get() + spawnCadenceProfile.Waves[current_wave].CompletionReward );
        }
        // if this was the final wave then mark the level complete
        if( current_wave == spawnCadenceProfile.Waves.Count - 1 && !PD.Instance.LevelCompletionMap.GetLevelCompletion( spawnCadenceProfile.LevelIdentifier ) )
        {
            PD.Instance.LevelCompletionMap.SetLevelCompletion( spawnCadenceProfile.LevelIdentifier, true );
        }
        spawn_timer = -1.0f; // stop spawning
        if( current_wave < spawnCadenceProfile.Waves.Count - 1 )
            StartNextWave();
        else
            SpawnCadenceComplete();
    }

    private void SpawnCadenceComplete()
    {
        Debug.Assert( VictoryScreen.instance != null );
        VictoryScreen.instance?.DisplayVictory();
    }

    private Vector3 GetRandomSpawnPoint()
    {
        return new Vector3( UnityEngine.Random.Range( SpawnableAreaBottomLeft.x, SpawnableAreaTopRight.x ), UnityEngine.Random.Range( SpawnableAreaBottomLeft.y, SpawnableAreaTopRight.y ), 0 );
    }

    private void SpawnSpawnGroup( SpawnGroup sg )
    {
        if( sg.layout == SpawnGroup.Layout.Cluster )
        {
            // determine radius of spawning circle
            float num_spawns = sg.SpawnMap.Aggregate( 0, ( current, next ) => next.Value + current );
            float desired_area = num_spawns / sg.ClusterDensity;
            float radius = Mathf.Sqrt( desired_area / Mathf.PI );
            if( radius > Mathf.Min( SpawnableAreaTopRight.x - SpawnableAreaBottomLeft.x, SpawnableAreaTopRight.y - SpawnableAreaBottomLeft.y ) / 2.0f )
            {
                Debug.LogError( "ERROR: Spawning Density not high enough in spawn group (" + sg.name + ")to fit all desired spawns in cluster inside play space - falling back to random distribution" );
                SpawnGroupRandomPlacement( sg );
            }
            else
            {
                Vector3 new_top_right = new Vector3( SpawnableAreaTopRight.x - radius, SpawnableAreaTopRight.y - radius );
                Vector3 new_bottom_left = new Vector3( SpawnableAreaBottomLeft.x + radius, SpawnableAreaBottomLeft.y + radius );
                Vector3 circle_center = new Vector3( UnityEngine.Random.Range( new_bottom_left.x, new_top_right.x ), UnityEngine.Random.Range( new_bottom_left.y, new_top_right.y ), 0 );

                float stagger = 0.0f;
                foreach( var e in sg.SpawnMap )
                {
                    for( int x = 0; x < e.Value; ++x )
                    {
                        float random_radius = UnityEngine.Random.Range( 0.0f, radius );
                        float random_theta = UnityEngine.Random.Range( 0.0f, Mathf.PI * 2 );
                        Vector3 final_point = new Vector3( Mathf.Cos( random_theta ) * random_radius + circle_center.x, Mathf.Sin( random_theta ) * random_radius + circle_center.y );
                        SpawnMonster( e.Key, final_point, stagger );
                        stagger += UnityEngine.Random.Range( sg.SpawnStaggerMinTime, sg.SpawnStaggerMaxTime );
                    }
                }
            }
        }
        else if( sg.layout == SpawnGroup.Layout.Door )
        {
            int door_index = UnityEngine.Random.Range( 0, DoorSpawnPoints.Count );
            float stagger = 0.0f;
            foreach( var e in sg.SpawnMap )
            {
                for( int x = 0; x < e.Value; ++x )
                {
                    SpawnMonster( e.Key, DoorSpawnPoints[door_index], stagger );
                    stagger += UnityEngine.Random.Range( sg.SpawnStaggerMinTime, sg.SpawnStaggerMaxTime );
                    door_index = ( door_index + 1 ) % DoorSpawnPoints.Count;
                }
            }
        }
        else
        {
            SpawnGroupRandomPlacement( sg );
        }
    }

    public List<Vector3> SpawnSpawnGroup( SpawnGroup sg, Vector3 circle_center, bool should_stagger )
    {
        List<Vector3> ret = new List<Vector3>();
        float num_spawns = sg.SpawnMap.Aggregate( 0, ( current, next ) => next.Value + current );
        float desired_area = num_spawns / sg.ClusterDensity;
        float radius = Mathf.Sqrt( desired_area / Mathf.PI );

        float stagger = 0.0f;
        foreach( var e in sg.SpawnMap )
        {
            for( int x = 0; x < e.Value; ++x )
            {
                Vector3 final_point;
                int loop_depth = 0;
                do
                {
                    float random_radius = UnityEngine.Random.Range( 0.0f, radius );
                    float random_theta = UnityEngine.Random.Range( 0.0f, Mathf.PI * 2 );
                    final_point = new Vector3( Mathf.Cos( random_theta ) * random_radius + circle_center.x, Mathf.Sin( random_theta ) * random_radius + circle_center.y );
                    ++loop_depth;
                } while( !PointInsidePlayableArea( final_point ) && loop_depth < 100 );
                if( loop_depth >= 100 )
                    Debug.LogError( "ERROR: Could not find spawn point inside playable area. Spawn Aborted" );
                else
                {
                    SpawnMonster( e.Key, final_point, stagger );
                    stagger += ( should_stagger ? UnityEngine.Random.Range( sg.SpawnStaggerMinTime, sg.SpawnStaggerMaxTime ) : 0.0f );
                    ret.Add( final_point );
                }
            }
        }

        return ret;
    }

    public bool PointInsidePlayableArea( Vector3 point )
    {
        return ( point.x > PlayableAreaBottomLeft.x && point.x < PlayableAreaTopRight.x
            && point.y < PlayableAreaTopRight.y && point.y > PlayableAreaBottomLeft.y );
    }

    private void SpawnGroupRandomPlacement( SpawnGroup sg )
    {
        float stagger = 0.0f;
        foreach( var e in sg.SpawnMap )
        {
            for( int x = 0; x < e.Value; ++x )
            {
                SpawnMonster( e.Key, GetRandomSpawnPoint(), stagger );
                stagger += UnityEngine.Random.Range( sg.SpawnStaggerMinTime, sg.SpawnStaggerMaxTime );
            }
        }
    }

    private void SpawnMonster( EnemyEnum enemy, Vector3 position, float delay = 0.0f )
    {
        if( delay == 0.0f )
        {
            GameObject e = InstantiateMonster( enemy, position );
            if( e != null )
                EnemySpawnedEvent.Invoke( e.GetComponent<Enemy>() );
        }
        else
            pending_spawns.AddLast( new PendingSpawn( delay, enemy, position ) );
    }

    public void RegisterEnemy( Enemy enemy )
    {
        enemy.OnDeath.AddListener( EnemyDied );
        Debug.Assert( !spawned_enemies.ContainsKey( enemy.EnemyID ) );
        spawned_enemies.Add( enemy.EnemyID, enemy );
    }

    private void EnemyDied( long EnemyID )
    {
        Debug.Assert( spawned_enemies.ContainsKey( EnemyID ) );
        spawned_enemies.Remove( EnemyID );
    }

    public Enemy TryGetEnemyByID( long id )
    {
        Enemy ret;
        spawned_enemies.TryGetValue( id, out ret );
        return ret;
    }

    GameObject InstantiateMonster( EnemyEnum enemy, Vector3 position )
    {
        GameObject ret = null;
        switch( enemy )
        {
            case EnemyEnum.Skeleton:
                ret = Instantiate( SkeletonPrefab );
                break;
            case EnemyEnum.ShieldSkeleton:
                ret = Instantiate( ShieldSkeletonPrefab );
                break;
            case EnemyEnum.PumpkinWarrior:
                ret = Instantiate( PumpkinWarriorPrefab );
                break;
            case EnemyEnum.PumpKING:
                ret = Instantiate(PumpKINGPrefab);
                break;
            case EnemyEnum.Bolter:
                ret = Instantiate( BolterPrefab );
                break;
            case EnemyEnum.MudSlinger:
                ret = Instantiate( MudSlingerPrefab );
                break;
            case EnemyEnum.Shrike:
                ret = Instantiate( ShrikePrefab );
                break;
            case EnemyEnum.Shaman:
                ret = Instantiate( ShamanPrefab );
                break;
            case EnemyEnum.CarrierL:
                ret = Instantiate( CarrierLPrefab );
                break;
            case EnemyEnum.CarrierM:
                ret = Instantiate( CarrierMPrefab );
                break;
            case EnemyEnum.CarrierS:
                ret = Instantiate( CarrierSPrefab );
                break;
            case EnemyEnum.RedSkeleton:
                ret = Instantiate( RedSkeletonPrefab );
                break;
            case 0:
                Debug.LogError( "ERROR: Tried to spawn an enemy " + enemy.ToString() + " that hasn't been added to the InstantiateMonster switch" );
                break;
        }
        if( ret )
            ret.transform.position = position;

        return ret;
    }

}

[System.Serializable]
public enum EnemyEnum
{
    Skeleton = 1,
    ShieldSkeleton = 2,
    PumpkinWarrior = 3,
    Bolter = 4,
    MudSlinger = 5,
    Shrike = 6,
    Shaman = 7,
    CarrierL = 8,
    CarrierM = 9,
    CarrierS = 10,
    RedSkeleton = 11,
    PumpKING = 12,
}

// EDITOR
#if UNITY_EDITOR
[CustomEditor( typeof( SpawnManager ) )]
public class SpawnManagerEditor : Editor
{
    private void OnSceneGUI()
    {
        SpawnManager spawn_manager = (SpawnManager)target;

        {
            Vector3 top_right = spawn_manager.SpawnableAreaTopRight;
            Vector3 bottom_left = spawn_manager.SpawnableAreaBottomLeft;
            Vector3 top_left = new Vector3( bottom_left.x, top_right.y, ( bottom_left.z + top_right.z ) / 2.0f );
            Vector3 bottom_right = new Vector3( top_right.x, bottom_left.y, ( bottom_left.z + top_right.z ) / 2.0f );
            Handles.color = Color.cyan;
            Handles.DrawLine( top_left, top_right );
            Handles.DrawLine( top_right, bottom_right );
            Handles.DrawLine( bottom_right, bottom_left );
            Handles.DrawLine( bottom_left, top_left );
        }

        {
            Vector3 top_right = spawn_manager.PlayableAreaTopRight;
            Vector3 bottom_left = spawn_manager.PlayableAreaBottomLeft;
            Vector3 top_left = new Vector3( bottom_left.x, top_right.y, ( bottom_left.z + top_right.z ) / 2.0f );
            Vector3 bottom_right = new Vector3( top_right.x, bottom_left.y, ( bottom_left.z + top_right.z ) / 2.0f );
            Handles.color = Color.green;
            Handles.DrawLine( top_left, top_right );
            Handles.DrawLine( top_right, bottom_right );
            Handles.DrawLine( bottom_right, bottom_left );
            Handles.DrawLine( bottom_left, top_left );
        }

        {
            foreach( Vector3 door_spawn_point in spawn_manager.DoorSpawnPoints )
            {
                Handles.color = Color.red;
                Handles.DrawWireDisc( door_spawn_point, Vector3.forward, 0.2f );
            }
        }

    }
}
#endif