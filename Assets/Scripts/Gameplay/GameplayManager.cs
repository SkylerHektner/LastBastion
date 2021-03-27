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
    private void StartChallenges()
    {
        Challenges?.Start();
    }

    // WIN LOSS STATE
    public enum PlayerState
    {
        Active,
        Won,
        Lost
    }
    public static PlayerState PlayerWinState = PlayerState.Active;

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
        Invoke( "StartChallenges", 0.1f ); // we have to delay this otherwise we run into script execution order problems
        Debug.Assert( Instance == null );
        Instance = this;
        TimeScale = 1.0f;
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