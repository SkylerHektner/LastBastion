﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChainLightningAbility : Ability
{
    struct PendingZap
    {
        public float time;
        public Vector3 position;
        public Vector3 last_position;
        public long EnemyID;
    }

    public ChainLightningAbilityData AbilityData;
    private List<PendingZap> pending_zaps = new List<PendingZap>();
    private float cur_time = 0.0f;
    private int cur_zap_index = 0;
    private float zapped_enemy_tracking_time = 0.0f;

    private bool listening = false;
    private int static_overload_layermask;

    public override void Start()
    {
        base.Start();

        GameObject.Instantiate( AbilityData.SceneWideEffect );

        List<Enemy> enemies_on_field = SpawnManager.Instance.AllSpawnedEnemies;
        if( enemies_on_field.Count == 0 )
        {
            Finish();
            return;
        }

        enemies_on_field.Sort( ( a, b ) => a.gameObject.transform.position.y.CompareTo( b.transform.position.y ) );
        pending_zaps.Add( new PendingZap()
        {
            time = 0.0f,
            position = enemies_on_field[0].transform.position,
            last_position = AM.BaseCenter,
            EnemyID = enemies_on_field[0].EnemyID,
        } );

        int t = 0;
        int h = 0;
        while( h < enemies_on_field.Count )
        {
            ++h;
            if( h >= enemies_on_field.Count )
                break;
            pending_zaps.Add( new PendingZap()
            {
                time = Mathf.Log( t + 1, 2.0f ) * AbilityData.TimeBetweenZaps,
                position = enemies_on_field[h].transform.position,
                last_position = enemies_on_field[t].transform.position,
                EnemyID = enemies_on_field[h].EnemyID,
            } );

            ++h;
            if( h >= enemies_on_field.Count )
                break;
            pending_zaps.Add( new PendingZap()
            {
                time = Mathf.Log( t + 1, 2.0f ) * AbilityData.TimeBetweenZaps,
                position = enemies_on_field[h].transform.position,
                last_position = enemies_on_field[t].transform.position,
                EnemyID = enemies_on_field[h].EnemyID,
            } );

            ++t;
        }

        if( PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.ChainLightningStaticOverload ) )
        {
            Saw.Instance.SawKilledEnemyEvent.AddListener( OnSawKilledEnemy );
            listening = true;
            static_overload_layermask = LayerMask.GetMask( "Enemy" );
            zapped_enemy_tracking_time = pending_zaps.Last().time + GetZapDuration() + 1.0f; // once second extra, just to be safe
        }
    }

    public override void Update( float delta_time )
    {
        base.Update( delta_time );
        if( pending_zaps.Count == 0 )
            return;

        cur_time += delta_time * GameplayManager.TimeScale;
        zapped_enemy_tracking_time -= delta_time * GameplayManager.TimeScale;

        while( cur_zap_index < pending_zaps.Count && pending_zaps[cur_zap_index].time <= cur_time )
        {
            DoZap( pending_zaps[cur_zap_index] );
            cur_zap_index++;
        }

        if( zapped_enemy_tracking_time <= 0.0f && cur_zap_index == pending_zaps.Count )
        {
            Finish();
        }
    }

    public override void Finish()
    {
        if( listening )
        {
            Saw.Instance.SawKilledEnemyEvent.RemoveListener( OnSawKilledEnemy );
            listening = false;
        }
        base.Finish();
    }

    private void DoZap( PendingZap zap )
    {
        ChainLightningEffect effect = GameObject.Instantiate( AbilityData.Effect );
        LineRenderer line_rend = effect.gameObject.GetComponent<LineRenderer>();
        line_rend.positionCount = 2;
        line_rend.SetPosition( 0, zap.last_position );
        line_rend.SetPosition( 1, zap.position );
        Enemy en = SpawnManager.Instance.TryGetEnemyByID( zap.EnemyID );
        if( en )
        {
            float duration = GetZapDuration();
            en.ZapForDuration( duration );

            DeleteAfterDuration zap_effect = GameObject.Instantiate( AbilityData.ZappedEffect );
            zap_effect.duration = duration;
            zap_effect.transform.position = zap.position;
            zap_effect.transform.parent = en.transform;
            en.DeathEvent.AddListener( zap_effect.DestroyOnDeathHook );
        }
    }

    private void OnSawKilledEnemy( Vector3 enemy_position )
    {
        Debug.Assert( PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.ChainLightningStaticOverload ) ); // shouldn't be listening unless this was true
        Collider2D[] hit = Physics2D.OverlapCircleAll( enemy_position, AbilityData.StaticOverloadExplosionRadius, static_overload_layermask );
        for( int x = 0; x < hit.Length; ++x )
        {
            Enemy en = hit[x].gameObject.GetComponent<Enemy>();
            en.Hit( ( en.transform.position - enemy_position ).normalized, true );
            GameObject.Instantiate( AbilityData.StaticOverloadExplosionEffect ).transform.position = enemy_position;
        }
    }

    private float GetZapDuration()
    {
        return PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.ChainLightningStunDuration )
                ? AbilityData.ImprovedZapDuration : AbilityData.ZapDuration;
    }
}