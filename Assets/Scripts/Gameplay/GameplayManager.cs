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
    public bool Survival;

    // ENVIRONEMNT
    public Environment ActiveEnvironment;

    // WIN LOSS STATE
    public enum PlayerState
    {
        Active,
        Won,
        Lost
    }
    public static PlayerState PlayerWinState
    {
        get
        {
            return playerWinState;
        }
        set
        {
            playerWinState = value;

            // kind of a hack, but we need to make sure that the player doesn't enter limbo
            // if they paused a level but came back and finished it without closing the application completely
            if( value != PlayerState.Active )
            {
                PD.Instance?.Limbo.Set( false );
            }
        }
    }
    private static PlayerState playerWinState = PlayerState.Active;

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
    public UnityEvent TimeScaleChanged = new UnityEvent();

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
        Invoke( "LateStart", 0.1f ); // we have to delay this otherwise we run into script execution order problems
        Debug.Assert( Instance == null );
        Instance = this;
        TimeScale = 1.0f;

        // clear out all survival unlock flags every time we start
        if( Survival )
        {
            foreach( PD.UpgradeFlags flag in Enum.GetValues( typeof( PD.UpgradeFlags ) ) )
            {
                PD.Instance.UpgradeUnlockMap.SetUnlock( flag, false, true );
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
}