using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyphoonAbility : Ability
{
    public static float AnimatorDuration;
    public static TyphoonAbility ActiveTyphoon { get; private set; }

    public TyphoonAbilityData AbilityData;
    private float time_remaining = 0.0f;
    private DeleteAfterDuration ActiveTyphoonDeleteAfterDuration;

    private float roaring_flames_duration_carryover = 1.0f;
    bool listening = false;

    public override void Start()
    {
        base.Start();
        ActiveTyphoonDeleteAfterDuration = GameObject.Instantiate( AbilityData.Effect ).GetComponent<DeleteAfterDuration>();
        SetDuration( AbilityData.Duration );
        roaring_flames_duration_carryover = AbilityData.Duration * 0.5f;
        ActiveTyphoon = this;
        if( PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.TyphoonRoaringFlames ) )
        {
            Saw.Instance.KilledEnemyEvent.AddListener( OnSawKilledEnemy );
            listening = true;
        }
    }

    public override void Update( float delta_time )
    {
        base.Update( delta_time );
        time_remaining -= delta_time * GameplayManager.Instance.GetTimeScale( GameplayManager.TimeScale.Combined );
        AnimatorDuration = time_remaining;
        if( time_remaining <= 0.0f )
            Finish();
    }

    public void SetSawOnFire( Saw saw )
    {
        if( !PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.TyphoonFlameSaw ) )
            return;
        saw.SetOnFire( AbilityData.FlameSawDuration, AbilityData.FlameSawExtraDamage, AbilityData.FlameSawMovementSpeedMultiplier );
    }

    public override void Finish()
    {
        ActiveTyphoon = null;
        if( listening )
        {
            Saw.Instance.KilledEnemyEvent.RemoveListener( OnSawKilledEnemy );
            listening = false;
        }
        base.Finish();
    }

    public override bool OnAbilityUsedWhileAlreadyActive()
    {
        SetDuration( time_remaining + AbilityData.Duration );
        roaring_flames_duration_carryover = AbilityData.Duration * 0.5f;

        // return true to cancel new ability construction
        return true;
    }

    private void SetDuration( float duration )
    {
        time_remaining = duration;
        Debug.Assert( ActiveTyphoonDeleteAfterDuration != null );
        if( ActiveTyphoonDeleteAfterDuration != null )
            ActiveTyphoonDeleteAfterDuration.duration = time_remaining;
    }

    private void OnSawKilledEnemy( long ID )
    {
        if( Saw.Instance.OnFire )
        {
            SetDuration( time_remaining + roaring_flames_duration_carryover );
            roaring_flames_duration_carryover *= 0.5f; // infinitely decreasing geometric series
        }
    }
}
