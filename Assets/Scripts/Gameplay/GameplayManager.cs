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
    public static float GamePlayTimeScale {
        get {
            return Instance.GetTimeScale( TimeScale.Combined );
        }
    }

    // TIME SCALE
    public enum TimeScale
    {
        Combined,
        UI,
        TemporalAnomaly,
        NUM_TIME_SCALES,
    }
    public float GetTimeScale( TimeScale scale )
    {
        if( scale != TimeScale.Combined )
            return time_scales[(int)scale];

        float combined_scale = 1.0f;
        for( int x = 1; x < (int)TimeScale.NUM_TIME_SCALES; ++x )
            combined_scale *= time_scales[x];
        return combined_scale;
    }
    private List<float> time_scales = new List<float>();
    private List<Coroutine> time_scale_lerps = new List<Coroutine>();

    public UnityEvent TimeScaleChanged = new UnityEvent();

    private void Start()
    {
        Debug.Assert( Instance == null );
        Instance = this;
        for(int x = 0; x < (int)TimeScale.NUM_TIME_SCALES; ++x)
        {
            time_scales.Add( 1.0f );
            time_scale_lerps.Add( null );
        }
    }

    public void SetTimeScale( float new_val, float lerp_duration, TimeScale scale )
    {
        Debug.Assert( scale != TimeScale.Combined, "Cannot manually set Combined time scale" );
        if( time_scale_lerps[(int)scale] != null )
        {
            //Debug.LogWarning( "Warning: Time scale lerp for " + scale.ToString() + " being started even though one is already in progress" );
            StopCoroutine( time_scale_lerps[(int)scale] );
        }
        time_scale_lerps[(int)scale] = StartCoroutine( LerpTimeScale( new_val, lerp_duration, scale ) );
    }

    public IEnumerator LerpTimeScale( float target, float duration, TimeScale scale )
    {
        float timer = 0.0f;
        float original_time_scale = time_scales[(int)scale];
        while( timer < duration )
        {
            yield return null;
            timer = Mathf.Min( Time.deltaTime + timer, duration );
            time_scales[(int)scale] = original_time_scale + ( timer / duration ) * ( target - original_time_scale );
            TimeScaleChanged.Invoke();
        }

        time_scales[(int)scale] = target;
        TimeScaleChanged.Invoke();
    }
}
