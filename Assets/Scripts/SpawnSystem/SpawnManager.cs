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

    [SerializeField] BaseSpawnCadenceProfile spawnCadenceProfile;
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
    [SerializeField] GameObject SkullyBossPrefab;
    [SerializeField] GameObject BlackHolePrefab;
    [SerializeField] GameObject BouncerPrefab;
    [SerializeField] GameObject GhostiePrefab;
    [SerializeField] GameObject BomberPrefab;
    [SerializeField] GameObject MudCarrierLPrefab;
    [SerializeField] GameObject MudCarrierSPrefab;
    [SerializeField] GameObject Shrike2Prefab;
    [SerializeField] GameObject Shaman2Prefab;
    [SerializeField] GameObject BlackSkeletonPrefab;

    [Header( "Misc" )]
    [SerializeField] WaveCounter WaveCounterUI;

    public SpawnWave CurrentWave { get; private set; } = null;
    [ReadOnly] public int CurrentWaveIndex = -1;

    public List<Enemy> AllSpawnedEnemies { get { return spawned_enemies.Values.ToList<Enemy>(); } }
    public UnityEvent<Enemy> EnemySpawnedEvent = new UnityEvent<Enemy>();
    public UnityEvent<int> WaveCompletedEvent = new UnityEvent<int>(); // pass completed wave number (not 0 based)

    private float spawn_timer = -1.0f;
    private int cur_spawn_group_index = 0;
    private List<float> passive_spawn_trackers = new List<float>();
    private LinkedList<PendingSpawn> pending_spawns = new LinkedList<PendingSpawn>();
    private Dictionary<long, Enemy> spawned_enemies = new Dictionary<long, Enemy>();
    private bool defer_next_wave_start = false;

    private int total_level_earnings = 0;

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

    // PUBLIC METHODS
    public void SetNewSpawnCadence( BaseSpawnCadenceProfile new_profile )
    {
        Debug.Assert( spawn_timer == -1.0f, "ERROR: Please only change the spawn cadence when the spawn manager is NOT spawning" );

        spawnCadenceProfile = new_profile;
    }

    // if invoked, the next wave will not start until YOU call StartNextWave
    public void DeferNextWaveStart()
    {
        defer_next_wave_start = true;
    }

    // Start the next wave. If no next wave, finish the cadence
    public void StartNextWave()
    {
        defer_next_wave_start = false;
        CurrentWaveIndex++;

        if( CurrentWaveIndex >= spawnCadenceProfile.GetWaveCount() )
        {
            SpawnCadenceComplete();
            return;
        }

        spawn_timer = 0.0f;
        CurrentWave = spawnCadenceProfile.GetWave( CurrentWaveIndex );
        WaveCounterUI?.ShowNextWave( CurrentWaveIndex + 1 );
        cur_spawn_group_index = 0;
        passive_spawn_trackers.Clear();
        if( !string.IsNullOrEmpty( CurrentWave.AnimationTrigger ) )
        {
            var triggers = CurrentWave.AnimationTrigger.Split( ',' ).Select( s => s.Trim() );
            foreach( Animator anim in GameplayManager.Instance.ActiveEnvironment.AnimationTriggerListeners )
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
        foreach( float time in CurrentWave.PassiveEnemySpawnCadence )
        {
            if( time == 0.0f )
            {
#if UNITY_EDITOR
                Debug.LogError( "ERROR: Passive Spawn Cadence set to 0 in Spawn Cadence Profile " + spawnCadenceProfile.GetName() + " Wave " + ( CurrentWaveIndex + 1 ).ToString() );
#endif
                continue;
            }
            passive_spawn_trackers.Add( 1.0f / time );
        }
    }

    // PRIVATE METHODS

    private void Start()
    {
        Instance = this;
#if UNITY_EDITOR
        CurrentWaveIndex = DebugStartWave - 2;
#endif

        if( PD.Instance.Limbo.Get() )
        {
            CurrentWaveIndex = PD.Instance.StoredLimboCurrentWave.Get() - 1; // subtract one since we immediately start next wave
        }

        GameplayManager.PlayerWinState = GameplayManager.PlayerState.Active;
        StartNextWave();
    }

    private void Update()
    {
        // EARLY RETURN - SPAWN MANAGER ONLY ACTIVE IF GAMEPLAY STATE IS ACTIVE
        if( GameplayManager.PlayerWinState != GameplayManager.PlayerState.Active )
            return;

        //manage pending spawns
        if( pending_spawns.Count != 0 )
        {
            for( var it = pending_spawns.First; it != null; )
            {
                var next = it.Next;
                var ps = it.Value;
                ps.time_left -= Time.deltaTime * GameplayManager.TimeScale *
                    ( PD.Instance.UnlockMap.Get( UnlockFlags.EnemySpawnSpeedCurse ) ?
                    GameplayManager.Instance.EnemySpawnSpeedCurseMultiplier : 1.0f );
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
            spawn_timer += Time.deltaTime * GameplayManager.TimeScale;

            // manage passive mob spawns
            // NO PASSIVE MOBS SPAWN AFTER LAST SPAWN GROUP HAS BEEN TRIGGERED
            if( cur_spawn_group_index < CurrentWave.SpawnGroups.Count )
            {
                for( int x = 0; x < passive_spawn_trackers.Count; ++x )
                {
                    passive_spawn_trackers[x] -= Time.deltaTime * GameplayManager.TimeScale *
                        ( PD.Instance.UnlockMap.Get( UnlockFlags.EnemySpawnSpeedCurse ) ?
                        GameplayManager.Instance.EnemySpawnSpeedCurseMultiplier : 1.0f );
                    if( passive_spawn_trackers[x] < 0.0f )
                    {
                        passive_spawn_trackers[x] += CurrentWave.PassiveEnemySpawnCadence[x];
                        SpawnMonster( CurrentWave.PassiveEnemies[x], GetRandomSpawnPoint() );
                    }
                }
            }

            // manage spawn groups
            while( cur_spawn_group_index < CurrentWave.SpawnGroups.Count &&
                spawn_timer > CurrentWave.SpawnGroupSpawnTimes[cur_spawn_group_index] )
            {

                SpawnSpawnGroup( CurrentWave.SpawnGroups[cur_spawn_group_index] );
                cur_spawn_group_index++;
            }

            if( cur_spawn_group_index == CurrentWave.SpawnGroups.Count
                && pending_spawns.Count == 0
                && spawned_enemies.Count == 0 )
            {
                WaveComplete();
            }
        }
    }

    private void WaveComplete()
    {
        // if this is the first time completing this wave, mark it as complete and grant the player a reward
        if( !PD.Instance.LevelCompletionMap.GetWaveCompletion( spawnCadenceProfile.GetLevelIdentifier(), CurrentWaveIndex ) )
        {
            int reward = CurrentWave.CompletionReward;
            if( reward > 0 )
            {
                PD.Instance.LevelCompletionMap.SetWaveCompletion( spawnCadenceProfile.GetLevelIdentifier(), CurrentWaveIndex, true );
                PD.Instance.PlayerWealth.Set( PD.Instance.PlayerWealth.Get() + reward );
                total_level_earnings += reward;
                Spectator.WitnessedVictory = true; // enables a popup on the menu screen for "New level unlocked!" notification
            }
        }
        // if this was the final wave then mark the level complete
        if( CurrentWaveIndex == spawnCadenceProfile.GetWaveCount() - 1 && !PD.Instance.LevelCompletionMap.GetLevelCompletion( spawnCadenceProfile.GetLevelIdentifier() ) )
        {
            PD.Instance.LevelCompletionMap.SetLevelCompletion( spawnCadenceProfile.GetLevelIdentifier(), true );
        }
        spawn_timer = -1.0f; // stop spawning

        WaveCompletedEvent.Invoke( CurrentWaveIndex + 1 );

        if( !defer_next_wave_start )
            StartNextWave();
    }

    private void SpawnCadenceComplete()
    {
        if( GameplayManager.PlayerWinState == GameplayManager.PlayerState.Active )
        {
            GameplayManager.PlayerWinState = GameplayManager.PlayerState.Won;

            bool challenge_success = false;
            bool has_challenge = spawnCadenceProfile.GetChallenge() != null
                && !PD.Instance.PlayerChallengeCompletionList.Contains( spawnCadenceProfile.GetChallenge().UniqueChallengeID );
            if( has_challenge )
            {
                challenge_success = GameplayManager.Instance.Challenges.EvaluateChallengeSuccess( spawnCadenceProfile.GetChallenge() );
                if( challenge_success )
                {
                    Debug.Assert( spawnCadenceProfile.GetChallenge().Reward > 0,
                        String.Format( "ERROR: Spawn Cadence {0} has succeeded but has not been assigned any reward", spawnCadenceProfile.GetChallenge().name ) );
                    PD.Instance.PlayerWealth.Set( PD.Instance.PlayerWealth.Get() + spawnCadenceProfile.GetChallenge().Reward );
                    PD.Instance.PlayerChallengeCompletionList.Add( spawnCadenceProfile.GetChallenge().UniqueChallengeID );
                    total_level_earnings += spawnCadenceProfile.GetChallenge().Reward;
                }
            }

            Debug.Assert( VictoryScreen.instance != null );
            VictoryScreen.instance?.DisplayVictory( total_level_earnings,
                has_challenge,
                challenge_success,
                spawnCadenceProfile.GetChallenge()?.ChallengeDescription );
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        Vector3 SpawnableAreaBottomLeft = GameplayManager.Instance.ActiveEnvironment.SpawnableAreaBottomLeft;
        Vector3 SpawnableAreaTopRight = GameplayManager.Instance.ActiveEnvironment.SpawnableAreaTopRight;
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
            Vector3 SpawnableAreaBottomLeft = GameplayManager.Instance.ActiveEnvironment.SpawnableAreaBottomLeft;
            Vector3 SpawnableAreaTopRight = GameplayManager.Instance.ActiveEnvironment.SpawnableAreaTopRight;
            if( radius > Mathf.Min( SpawnableAreaTopRight.x - SpawnableAreaBottomLeft.x, SpawnableAreaTopRight.y - SpawnableAreaBottomLeft.y ) / 2.0f )
            {
#if UNITY_EDITOR
                Debug.LogError( "ERROR: Spawning Density not high enough in spawn group (" + sg.name + ")to fit all desired spawns in cluster inside play space - falling back to random distribution" );
#endif
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
            Debug.Assert( GameplayManager.Instance.ActiveEnvironment.DoorSpawnPoints.Count > 0,
                "ERROR: Trying to spawn monster from door in an environment with no configured door spawn points" );

            int door_index = UnityEngine.Random.Range( 0, GameplayManager.Instance.ActiveEnvironment.DoorSpawnPoints.Count );
            float stagger = 0.0f;
            foreach( var e in sg.SpawnMap )
            {
                for( int x = 0; x < e.Value; ++x )
                {
                    SpawnMonster( e.Key, GameplayManager.Instance.ActiveEnvironment.DoorSpawnPoints[door_index], stagger );
                    stagger += UnityEngine.Random.Range( sg.SpawnStaggerMinTime, sg.SpawnStaggerMaxTime );
                    door_index = ( door_index + 1 ) % GameplayManager.Instance.ActiveEnvironment.DoorSpawnPoints.Count;
                }
            }
        }
        else if( sg.layout == SpawnGroup.Layout.Boss )
        {
            float stagger = 0.0f;
            foreach( var e in sg.SpawnMap )
            {
                for( int x = 0; x < e.Value; ++x )
                {
                    SpawnMonster( e.Key, GameplayManager.Instance.ActiveEnvironment.BossSpawnPoint, stagger );
                    stagger += UnityEngine.Random.Range( sg.SpawnStaggerMinTime, sg.SpawnStaggerMaxTime );
                }
            }
        }
        else if( sg.layout == SpawnGroup.Layout.Custom )
        {
            float stagger = 0.0f;
            int index = 0;
            foreach( var e in sg.SpawnMap )
            {
                for( int x = 0; x < e.Value; ++x )
                {
                    Debug.Assert( index < sg.custom_layout_positions.Count );

                    // relative coordinates are considering top-left as (0,0)
                    Vector2 top_left = new Vector2(
                        GameplayManager.Instance.ActiveEnvironment.PlayableAreaBottomLeft.x,
                        GameplayManager.Instance.ActiveEnvironment.PlayableAreaTopRight.y );
                    Vector2 deltas = new Vector2(
                        GameplayManager.Instance.ActiveEnvironment.PlayableAreaTopRight.x - top_left.x,
                        GameplayManager.Instance.ActiveEnvironment.PlayableAreaBottomLeft.y - top_left.y );
                    Vector3 spawn_point = new Vector3(
                        top_left.x + deltas.x * sg.custom_layout_positions[index].x,
                        top_left.y + deltas.y * sg.custom_layout_positions[index].y );
                    SpawnMonster( e.Key, spawn_point, stagger );
                    stagger += UnityEngine.Random.Range( sg.SpawnStaggerMinTime, sg.SpawnStaggerMaxTime );
                    index++;
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
                if( loop_depth < 100 )
                {
                    SpawnMonster( e.Key, final_point, stagger );
                    stagger += ( should_stagger ? UnityEngine.Random.Range( sg.SpawnStaggerMinTime, sg.SpawnStaggerMaxTime ) : 0.0f );
                    ret.Add( final_point );
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogError( "ERROR: Could not find spawn point inside playable area. Spawn Aborted" );
                }
#endif
            }
        }

        return ret;
    }

    public bool PointInsidePlayableArea( Vector3 point )
    {
        Vector3 PlayableAreaBottomLeft = GameplayManager.Instance.ActiveEnvironment.PlayableAreaBottomLeft;
        Vector3 PlayableAreaTopRight = GameplayManager.Instance.ActiveEnvironment.PlayableAreaTopRight;

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
            enemy = CheckForSkeletonUpgrade( enemy );
            Enemy e = InstantiateMonster( enemy, position ).GetComponent<Enemy>();
            if( e != null )
                EnemySpawnedEvent.Invoke( e );
            RegisterEnemy( e );
        }
        else
            pending_spawns.AddLast( new PendingSpawn( delay, enemy, position ) );
    }

    // used by SkeletonUpgradeCurse
    private EnemyEnum CheckForSkeletonUpgrade( EnemyEnum e )
    {
        if( !PD.Instance.UnlockMap.Get( UnlockFlags.SkeletonUpgradeCurse ) )
            return e;

        if( GameplayManager.Instance.SkeletonUpgradeCurseChance > UnityEngine.Random.Range( 0.0f, 1.0f ) &&
            SkeletonHashSet.EnemyUpgradeMap.TryGetValue( e, out EnemyEnum upgraded_enemy ) )
        {
            return upgraded_enemy;
        }

        return e;
    }

    public void RegisterEnemy( Enemy enemy )
    {
        enemy.DeathEvent.AddListener( EnemyDied );
        Debug.Assert( !spawned_enemies.ContainsKey( enemy.EnemyID ) );
        spawned_enemies.Add( enemy.EnemyID, enemy );
    }

    private void EnemyDied( Enemy en )
    {
        Debug.Assert( spawned_enemies.ContainsKey( en.EnemyID ) );
        spawned_enemies.Remove( en.EnemyID );
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
                ret = Instantiate( PumpKINGPrefab );
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
            case EnemyEnum.SkullyBoss:
                ret = Instantiate( SkullyBossPrefab );
                break;
            case EnemyEnum.BlackHole:
                ret = Instantiate( BlackHolePrefab );
                break;
            case EnemyEnum.Bouncer:
                ret = Instantiate( BouncerPrefab );
                break;
            case EnemyEnum.Ghostie:
                ret = Instantiate( GhostiePrefab );
                break;
            case EnemyEnum.Bomber:
                ret = Instantiate( BomberPrefab );
                break;
            case EnemyEnum.MudCarrierL:
                ret = Instantiate( MudCarrierLPrefab );
                break;
            case EnemyEnum.MudCarrierS:
                ret = Instantiate( MudCarrierSPrefab );
                break;
            case EnemyEnum.Shrike2:
                ret = Instantiate( Shrike2Prefab );
                break;
            case EnemyEnum.Shaman2:
                ret = Instantiate( Shaman2Prefab );
                break;
            case EnemyEnum.BlackSkeleton:
                ret = Instantiate( BlackSkeletonPrefab );
                break;
            case 0:
#if UNITY_EDITOR
                Debug.LogError( "ERROR: Tried to spawn an enemy " + enemy.ToString() + " that hasn't been added to the InstantiateMonster switch" );
#endif
                break;
        }
        if( ret )
            ret.transform.position = position;

        return ret;
    }
}


/// <summary>
/// IF YOU ADD A NEW ENEMY PLEASE UPDATE THE SKELETON HASH SET IF RELEVANT
/// </summary>
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
    SkullyBoss = 13,
    BlackHole = 14,
    Bouncer = 15,
    Ghostie = 16,
    Bomber = 17,
    MudCarrierL = 18,
    MudCarrierS = 19,
    Shrike2 = 20,
    Shaman2 = 21,
    BlackSkeleton = 22,
}

class SkeletonHashSet
{
    // Used for SkeletonUpgradeCurse
    public static readonly Dictionary<EnemyEnum, EnemyEnum> EnemyUpgradeMap = new Dictionary<EnemyEnum, EnemyEnum>()
    {
        {  EnemyEnum.Skeleton, EnemyEnum.ShieldSkeleton },
        { EnemyEnum.ShieldSkeleton, EnemyEnum.BlackSkeleton }
    };
}