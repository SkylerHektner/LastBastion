﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnomalyAbility : Ability
{
    public AnomalyAbilityData AbilityData;

    private List<Tuple<SpectralSaw, Vector3, float>> pending_saws = new List<Tuple<SpectralSaw, Vector3, float>>();
    private float time_remaining = 0.0f;
    public static float AnimatorDuration;
    public override void Start()
    {
        base.Start();
        Saw.Instance?.SawFiredEvent?.AddListener( OnSawFired );
        time_remaining = AbilityData.Duration;
    }

    private void OnSawFired( Vector3 pos, Vector3 direction, float speed )
    {
        SpectralSaw saw = GameObject.Instantiate( AbilityData.SpectralSawPrefab );
        saw.transform.position = pos;
        if( PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.AnomalyRicochetSaws ) )
            saw.NumRemainingExtraBounces = AbilityData.RichochetSawExtraBounces;
        pending_saws.Add( new Tuple<SpectralSaw, Vector3, float>( saw, direction, speed ) );

        if( PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.AnomalySingularity ) )
        {
            SpectralSaw mirror_saw = GameObject.Instantiate( AbilityData.SpectralSawPrefab );
            
            // mirror to other side
            pos.x = -pos.x; 
            direction.x = -direction.x;
            
            mirror_saw.transform.position = pos;
            if( PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.AnomalyRicochetSaws ) )
                mirror_saw.NumRemainingExtraBounces = AbilityData.RichochetSawExtraBounces;
            pending_saws.Add( new Tuple<SpectralSaw, Vector3, float>( mirror_saw, direction, speed ) );
        }
    }

    public override void Update( float delta_time )
    {
        base.Update( delta_time );
        time_remaining -= delta_time * GameplayManager.Instance.GetTimeScale( GameplayManager.TimeScale.UI );
        AnimatorDuration = time_remaining;
        if( time_remaining <= 0.0f )
            Finish();
    }

    public override void Finish()
    {
        base.Finish();

        Saw.Instance?.SawFiredEvent?.RemoveListener( OnSawFired );

        foreach( var pending_saw in pending_saws )
        {
            Projectile saw_projectile = pending_saw.Item1.GetComponent<Projectile>();
            Debug.Assert( saw_projectile != null );
            saw_projectile.SetProjectileSpeed( pending_saw.Item3 );
            saw_projectile.StartMoveInDirection( pending_saw.Item2 );
        }
    }
}