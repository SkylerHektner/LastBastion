﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnomalyAbility : Ability
{
    public static AnomalyAbility ActiveAnomaly { get; private set; }
    public static float AnimatorDuration;

    public AnomalyAbilityData AbilityData;

    private List<Tuple<SpectralSaw, Vector3, float>> pending_saws = new List<Tuple<SpectralSaw, Vector3, float>>();
    private float time_remaining = 0.0f;
    private List<long> stasis_coated_enemies = new List<long>();
    public override void Start()
    {
        base.Start();
        ActiveAnomaly = this;
        Saw.Instance?.SawFiredEvent?.AddListener( OnSawFired );
        time_remaining = AbilityData.Duration * GetAbilityDurationMultiplier();

        if( PD.Instance.UnlockMap.Get( UnlockFlag.AnomalyStasisCoating ) )
        {
            Saw.Instance?.SawHitEnemyEvent?.AddListener( OnSawHitEnemy );
        }
    }

    private void OnSawFired( Vector3 pos, Vector3 direction, float speed )
    {
        SpectralSaw saw = GameObject.Instantiate( AbilityData.SpectralSawPrefab );
        saw.transform.position = pos;
        if( PD.Instance.UnlockMap.Get( UnlockFlag.AnomalyRicochetSaws ) )
            saw.NumRemainingExtraBounces = AbilityData.RichochetSawExtraBounces;
        pending_saws.Add( new Tuple<SpectralSaw, Vector3, float>( saw, direction, speed ) );

        if( PD.Instance.UnlockMap.Get( UnlockFlag.AnomalySingularity ) )
        {
            SpectralSaw mirror_saw = GameObject.Instantiate( AbilityData.SpectralSawPrefab );

            // mirror to other side
            pos.x = -pos.x;
            direction.x = -direction.x;

            mirror_saw.transform.position = pos;
            if( PD.Instance.UnlockMap.Get( UnlockFlag.AnomalyRicochetSaws ) )
                mirror_saw.NumRemainingExtraBounces = AbilityData.RichochetSawExtraBounces;
            pending_saws.Add( new Tuple<SpectralSaw, Vector3, float>( mirror_saw, direction, speed ) );
        }
    }

    private void OnSawHitEnemy( long id )
    {
        Debug.Assert( PD.Instance.UnlockMap.Get( UnlockFlag.AnomalyStasisCoating ) );
        Enemy en = SpawnManager.Instance.TryGetEnemyByID( id );
        if( en != null )
        {
            StasisCoatEnemy( en );
        }
    }

    public void StasisCoatEnemy( Enemy en )
    {
        stasis_coated_enemies.Add( en.EnemyID );
        en.StasisCoat( AbilityData.StasisTouchReplacementMaterial );
        GameObject effect = GameObject.Instantiate( AbilityData.StasisTouchEffectPrefab );
        effect.transform.position = en.transform.position;
    }

    public override void Update( float delta_time )
    {
        base.Update( delta_time );
        time_remaining -= delta_time * GameplayManager.TimeScale;
        AnimatorDuration = time_remaining;
        if( time_remaining <= 0.0f )
            Finish();
    }

    public override void Finish()
    {
        base.Finish();

        Saw.Instance?.SawFiredEvent?.RemoveListener( OnSawFired );
        Saw.Instance?.SawHitEnemyEvent?.RemoveListener( OnSawHitEnemy );

        foreach( var pending_saw in pending_saws )
        {
            if( pending_saw.Item1 != null )
            {
                Projectile saw_projectile = pending_saw.Item1.GetComponent<Projectile>();
                Debug.Assert( saw_projectile != null );
                saw_projectile.SetProjectileSpeed( pending_saw.Item3 );
                saw_projectile.StartMoveInDirection( pending_saw.Item2 );
            }
            else
            {
                Debug.LogWarning( "Warning: Anomaly lasted so long that a pending saw expired" );
            }
        }

        // record player stats
        PD.Instance.HighestAnomalySawUnleash.Set( Mathf.Max( PD.Instance.HighestAnomalySawUnleash.Get(), pending_saws.Count ) );
        PD.Instance.TotalNumberOfAnomalySawUnleash.Set( PD.Instance.TotalNumberOfAnomalySawUnleash.Get() + pending_saws.Count );

        foreach ( long id in stasis_coated_enemies )
        {
            Enemy en = SpawnManager.Instance.TryGetEnemyByID( id );
            if( en != null )
                en.EndStatisCoating();
        }

        ActiveAnomaly = null;
    }

    public override bool OnAbilityUsedWhileAlreadyActive()
    {
        time_remaining += AbilityData.Duration * GetAbilityDurationMultiplier();
        // return true to cancel new ability construction
        return true;
    }

    public override void OnSceneExit()
    {
        AnimatorDuration = 0.0f;
        ActiveAnomaly = this;
        base.OnSceneExit();
    }
}
