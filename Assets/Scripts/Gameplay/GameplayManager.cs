using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Home of the lost, the scared. Those with nowhere else to go
/// </summary>
public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    // CHALLENGES
    public ChallengesTracker Challenges { get; private set; } = new ChallengesTracker();

    // SURIVAL
    [Header( "Survival Variables" )]
    public bool Survival;

    // CURSE VARIABLE VALUES
    [Header("Curse Variables")]
    [SerializeField] private float enemyMoveSpeedCurseMultiplier = 1.15f;
    [SerializeField] private float enemyMoveSpeedCurseMultiplier2 = 1.3f;
    [SerializeField] private float enemySpawnSpeedCurseMultiplier = 1.2f;
    [SerializeField] private float enemySpawnSpeedCurseMultiplier2 = 1.4f;
    [SerializeField] private float sawRadiusCurseMultiplier = 0.85f;
    [SerializeField] private float sawRadiusCurseMultiplier2 = 0.75f;
    [SerializeField] private float sawMovementSpeedCurseMultiplier = 0.9f;
    [SerializeField] private float sawMovementSpeedCurseMultiplier2 = 0.8f;
    [Range( 0.0f, 1.0f )]
    [SerializeField] private float crystalDropChanceCurseMultiplier = 0.75f;
    [Range( 0.0f, 1.0f )]
    [SerializeField] private float crystalDropChanceCurseMultiplier2 = 0.55f;
    [SerializeField] private float abilityDurationCurseMultiplier = 0.8f;
    [SerializeField] private float abilityDurationCurseMultiplier2 = 0.65f;
    public float SkeletonUpgradeCurseChance = 0.1f;

    public float EnemyMoveSpeedCurseMultiplier
    {
        get
        {
            if( PD.Instance.UnlockMap.Get( UnlockFlags.EnemyMovementSpeedCurse2 ) )
                return enemyMoveSpeedCurseMultiplier2;
            else if( PD.Instance.UnlockMap.Get( UnlockFlags.EnemyMovementSpeedCurse ) )
                return enemyMoveSpeedCurseMultiplier;
            else
                return 1.0f;
        }
    }

    public float EnemySpawnSpeedCurseMultiplier
    {
        get
        {
            if( PD.Instance.UnlockMap.Get( UnlockFlags.EnemySpawnSpeedCurse2 ) )
                return enemySpawnSpeedCurseMultiplier2;
            else if( PD.Instance.UnlockMap.Get( UnlockFlags.EnemySpawnSpeedCurse ) )
                return enemySpawnSpeedCurseMultiplier;
            else
                return 1.0f;
        }
    }

    public float SawRadiusCurseMultiplier
    {
        get
        {
            if( PD.Instance.UnlockMap.Get( UnlockFlags.SawRadiusCurse2 ) )
                return sawRadiusCurseMultiplier2;
            else if( PD.Instance.UnlockMap.Get( UnlockFlags.SawRadiusCurse ) )
                return sawRadiusCurseMultiplier;
            else
                return 1.0f;
        }
    }

    public float SawMovementSpeedCurseMultiplier
    {
        get
        {
            if( PD.Instance.UnlockMap.Get( UnlockFlags.SawMovementSpeedCurse2 ) )
                return sawMovementSpeedCurseMultiplier2;
            else if( PD.Instance.UnlockMap.Get( UnlockFlags.SawMovementSpeedCurse ) )
                return sawMovementSpeedCurseMultiplier;
            else
                return 1.0f;
        }
    }

    public float CrystalDropChanceCurseMultiplier
    {
        get
        {
            if( PD.Instance.UnlockMap.Get( UnlockFlags.CrystalDropChanceCurse2 ) )
                return crystalDropChanceCurseMultiplier2;
            else if( PD.Instance.UnlockMap.Get( UnlockFlags.CrystalDropChanceCurse ) )
                return crystalDropChanceCurseMultiplier;
            else
                return 1.1f;
        }
    }

    public float AbilityDurationCurseMultiplier
    {
        get
        {
            if( PD.Instance.UnlockMap.Get( UnlockFlags.AbilityDurationCurse2 ) )
                return abilityDurationCurseMultiplier2;
            else if( PD.Instance.UnlockMap.Get( UnlockFlags.AbilityDurationCurse ) )
                return abilityDurationCurseMultiplier;
            else
                return 1.0f;
        }
    }

    // ENVIRONEMNT
    [Header( "Environment Variables" )]
    public Environment ActiveEnvironment;

    // WIN LOSS STATE
    public enum GameState
    {
        Active,
        Won,
        ChoosingUpgrade, // only used in surival mode
        Lost
    }
    public static GameState State
    {
        get
        {
            return current_state;
        }
        set
        {
            current_state = value;

            // kind of a hack, but we need to make sure that the player doesn't enter limbo
            // if they paused a level but came back and finished it without closing the application completely
            if( value != GameState.Active )
            {
                PD.Instance?.Limbo.Set( false );
            }
        }
    }
    private static GameState current_state = GameState.Active;

    // TIME SCALE
    public static float TimeScale
    {
        get
        {
            return gamePlayTimeScale;
        }
        set
        {
            gamePlayTimeScale = value;
            Instance?.TimeScaleChanged.Invoke();
        }
    }
    private static float gamePlayTimeScale;
    [HideInInspector] public UnityEvent TimeScaleChanged = new UnityEvent();

    public void SetTimeScale( float new_val, float lerp_duration )
    {
        StopAllCoroutines();
        StartCoroutine( LerpTimeScale( new_val, lerp_duration ) );
    }

    public IEnumerator LerpTimeScale( float target, float duration )
    {
        float timer = 0.0f;
        float original_time_scale = TimeScale;
        while( timer < duration )
        {
            yield return null;
            timer = Mathf.Min( Time.deltaTime + timer, duration );
            TimeScale = original_time_scale + ( timer / duration ) * ( target - original_time_scale );
        }

        TimeScale = target;
    }

    // MONOBEHAVIOR
    private void Start()
    {
        PD.Instance.Start();
        Invoke( "LateStart", 0.1f ); // we have to delay this otherwise we run into script execution order problems
        Debug.Assert( Instance == null );
        Instance = this;
        TimeScale = 1.0f;

        // clear out all survival unlock flags every time we start
        if( Survival )
        {
            foreach( UnlockFlags flag in Enum.GetValues( typeof( UnlockFlags ) ) )
            {
                PD.Instance.UnlockMap.Set( flag, false, true );
            }
        }
    }
    private void LateStart()
    {
        Challenges?.Start();

        if( PD.Instance.Limbo.Get() )
        {
            PD.Instance.Limbo.Set( false );
        }

        SpawnManager.Instance.WaveCompletedEvent.AddListener( OnWaveComplete );
    }

    private void Update()
    {
        PD.Instance.Tick();
        Challenges?.Tick( Time.deltaTime * TimeScale );
    }

    private void OnDestroy()
    {
        Challenges?.End();
    }

    private void OnWaveComplete(int wave)
    {
        PD.Instance.LazyTick();
    }
}