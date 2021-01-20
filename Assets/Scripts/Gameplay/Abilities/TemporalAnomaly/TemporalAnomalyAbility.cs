using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TemporalAnomalyAbility : Ability
{
    public TemporalAnomalyAbilityData AbilityData;

    private List<Tuple<SpectralSaw, Vector3, float>> pending_saws = new List<Tuple<SpectralSaw, Vector3, float>>();
    private float time_remaining = 0.0f;
    public override void Start()
    {
        base.Start();
        Saw.Instance?.SawFiredEvent?.AddListener( OnSawFired );
        time_remaining = AbilityData.Duration;
        GameplayManager.Instance.SetTimeScale( AbilityData.GameplaySpeedMultiplier,
            AbilityData.GameplaySpeedLerpDuration, 
            GameplayManager.TimeScale.TemporalAnomaly );
    }

    private void OnSawFired( Vector3 pos, Vector3 direction, float speed)
    {
        SpectralSaw saw = GameObject.Instantiate( AbilityData.SpectralSawPrefab );
        saw.transform.position = pos;
        pending_saws.Add( new Tuple<SpectralSaw, Vector3, float>( saw, direction, speed ) );
    }

    public override void Update( float delta_time )
    {
        base.Update( delta_time );
        time_remaining -= delta_time * GameplayManager.Instance.GetTimeScale( GameplayManager.TimeScale.UI );
        if( time_remaining <= 0.0f )
            Finish();
    }

    public override void Finish()
    {
        base.Finish();
        
        GameplayManager.Instance.SetTimeScale( 1.0f,
            AbilityData.GameplaySpeedLerpDuration,
            GameplayManager.TimeScale.TemporalAnomaly );

        Saw.Instance?.SawFiredEvent?.RemoveListener( OnSawFired );
        
        foreach(var pending_saw in pending_saws)
        {
            Projectile saw_projectile = pending_saw.Item1.GetComponent<Projectile>();
            Debug.Assert( saw_projectile != null );
            saw_projectile.SetProjectileSpeed( pending_saw.Item3 );
            saw_projectile.StartMoveInDirection( pending_saw.Item2 );
        }
    }
}
