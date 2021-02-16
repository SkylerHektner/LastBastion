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

    public override void Start()
    {
        base.Start();
        ActiveTyphoonDeleteAfterDuration = GameObject.Instantiate( AbilityData.Effect ).GetComponent<DeleteAfterDuration>();
        ActiveTyphoonDeleteAfterDuration.duration = AbilityData.Duration;
        time_remaining = AbilityData.Duration;
        ActiveTyphoon = this;
    }

    public override void Update( float delta_time )
    {
        base.Update( delta_time );
        time_remaining -= delta_time * GameplayManager.Instance.GetTimeScale( GameplayManager.TimeScale.Combined );
        AnimatorDuration = time_remaining;
        if( time_remaining <= 0.0f )
            Finish();
    }

    public void SetSawOnFire(Saw saw)
    {
        if( !PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.TyphoonFlameSaw ) )
            return;
        saw.SetOnFire( AbilityData.FlameSawDuration, AbilityData.FlameSawExtraDamage, AbilityData.FlameSawMovementSpeedMultiplier );
    }

    public override void Finish()
    {
        ActiveTyphoon = null;

        base.Finish();
    }

    public override bool OnAbilityUsedWhileAlreadyActive()
    {
        time_remaining += AbilityData.Duration;
        Debug.Assert( ActiveTyphoonDeleteAfterDuration != null );
        if( ActiveTyphoonDeleteAfterDuration != null)
            ActiveTyphoonDeleteAfterDuration.duration += AbilityData.Duration;
        // return true to cancel new ability construction
        return true;
    }
}
