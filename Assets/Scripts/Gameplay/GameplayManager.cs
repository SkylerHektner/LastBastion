using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Home of the lost, the scared. Those with nowhere else to go
/// </summary>
public class GameplayManager : MonoBehaviour
{
    // TIME SCALE
    public static float GamePlayTimeScale {
        get {
            return gamePlayTimeScale;
        }
        set {
            gamePlayTimeScale = value;
            TimeScaleChanged.Invoke();
        }
    }
    private static float gamePlayTimeScale;
    public static UnityEvent TimeScaleChanged = new UnityEvent();

    public static GameplayManager Instance { get; private set; }

    private void Start()
    {
        GamePlayTimeScale = 1.0f;
        Instance = this;
    }

    public void SetTimeScale( float new_val, float lerp_duration )
    {
        StopAllCoroutines();
        StartCoroutine( LerpTimeScale( new_val, lerp_duration ) );
    }

    public IEnumerator LerpTimeScale( float target, float duration )
    {
        float timer = 0.0f;
        float original_time_scale = GamePlayTimeScale;
        while( timer < duration )
        {
            yield return null;
            timer = Mathf.Min( Time.deltaTime + timer, duration );
            GamePlayTimeScale = original_time_scale + ( timer / duration ) * ( target - original_time_scale );
        }

        GamePlayTimeScale = target;
    }
}
