using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawmageddonAbility : Ability
{
    public static SawmageddonAbility ActiveSawmageddon { get; private set; }
    public static float AnimatorDuration;

    public SawmageddonAbilityData AbilityData;

    private float time_left = 0.0f;
    private int combo_killer_max = -1;
    private int cur_combo_killer_kills = 0;
    private int num_kills_this_release = 0; // tracks the number of kills per shot. Needed for player stats

    private bool listening = false;

    public override void Start()
    {
        base.Start();
        ActiveSawmageddon = this;
        time_left = ( PD.Instance.UnlockMap.Get( UnlockFlags.SawmageddonDuration )
            ? AbilityData.ImprovedDuration : AbilityData.Duration )
            * GetAbilityDurationMultiplier();
        Saw.Instance.SawFiredEvent.AddListener( OnSawFired );
        Saw.Instance.SawAttachToWallEvent.AddListener( OnSawAttachedToWall );
        SpawnManager.Instance.EnemyDiedEvent.AddListener( OnEnemyDied );
        if( PD.Instance.UnlockMap.Get( UnlockFlags.SawmageddonComboKiller ) )
        {
            combo_killer_max = AbilityData.ComboKillerHPRegainKillsBase;
            Saw.Instance?.SawKilledEnemyEvent?.AddListener( OnSawKilledEnemy );
            listening = true;
            ComboKillerDisplay.Instance?.gameObject.SetActive( true );
            Debug.Assert( ComboKillerDisplay.Instance != null );
        }
    }

    private void OnSawFired( Vector3 pos, Vector3 direction, float speed )
    {
        int num_extra_saws = PD.Instance.UnlockMap.Get( UnlockFlags.SawmageddonProjectiles )
            ? AbilityData.ImprovedNumberExtraSaws : AbilityData.NumberExtraSaws;
        HashSet<long> shared_collision_set = new HashSet<long>();
        for( int x = 0; x < num_extra_saws; ++x )
        {
            int angle_mult = ( x % 2 == 1 ? -1 : 1 ) * ( x / 2 + 1 );
            Vector3 new_direction = MathUtility.RotateVector2D( direction, AbilityData.OffsetAngle * Mathf.Deg2Rad * angle_mult );
            SpectralSaw spec_saw = GameObject.Instantiate( AbilityData.SpectralSawPrefab );
            spec_saw.gameObject.transform.position = pos;
            spec_saw.SetProjectileSpeed( speed );
            spec_saw.StartMoveInDirection( new_direction );
            spec_saw.SetSharedCollisionSet( shared_collision_set );

            if( PD.Instance.UnlockMap.Get( UnlockFlags.SawmageddonComboKiller ) )
                spec_saw.SawKilledEnemyEvent.AddListener( OnSawKilledEnemy );
        }

        num_kills_this_release = 0;
    }

    private void OnSawAttachedToWall()
    {
        // record player stats
        PD.Instance.HighestEnemyDeathTollFromSawmageddonShot.Set(
            Mathf.Max(
                PD.Instance.HighestEnemyDeathTollFromSawmageddonShot.Get(),
                num_kills_this_release
                ) );

        num_kills_this_release = 0;
    }

    private void OnEnemyDied( Enemy en )
    {
        if( en.DeathSource == DamageSource.Saw || en.DeathSource == DamageSource.SpectralSaw )
        {
            ++num_kills_this_release;
        }
    }

    public override void Finish()
    {
        Saw.Instance?.SawFiredEvent?.RemoveListener( OnSawFired );
        Saw.Instance.SawAttachToWallEvent.RemoveListener( OnSawAttachedToWall );
        SpawnManager.Instance.EnemyDiedEvent.RemoveListener( OnEnemyDied );
        if( listening )
        {
            Saw.Instance?.SawKilledEnemyEvent?.RemoveListener( OnSawKilledEnemy );
            ComboKillerDisplay.Instance.gameObject.SetActive( false );
            Debug.Assert( ComboKillerDisplay.Instance != null );
        }
        ActiveSawmageddon = null;
        base.Finish();
    }

    public override void Update( float delta_time )
    {
        base.Update( delta_time );

        time_left -= delta_time * GameplayManager.TimeScale;
        AnimatorDuration = time_left;

        if( time_left <= 0.0f )
            Finish();
    }

    public override bool OnAbilityUsedWhileAlreadyActive()
    {
        time_left += AbilityData.Duration;
        // return true to cancel new ability construction
        return true;
    }

    private void OnSawKilledEnemy( Vector3 enemy_position )
    {
        cur_combo_killer_kills++;

        if( cur_combo_killer_kills >= combo_killer_max )
        {
            cur_combo_killer_kills = 0;
            combo_killer_max = Mathf.RoundToInt( combo_killer_max * AbilityData.ComboKillerHPRegainScaleFactor );
            BaseHP.Instance?.Heal( 3 );
        }
        Debug.Assert( ComboKillerDisplay.Instance != null );
        ComboKillerDisplay.Instance?.SetChargeAmount( cur_combo_killer_kills, combo_killer_max );
    }

    public override void OnSceneExit()
    {
        AnimatorDuration = 0.0f;
        ActiveSawmageddon = null;
        base.OnSceneExit();
    }
}
