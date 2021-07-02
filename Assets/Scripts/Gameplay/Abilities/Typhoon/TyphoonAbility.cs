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
        SetDuration( AbilityData.Duration * GetAbilityDurationMultiplier() );
        roaring_flames_duration_carryover = AbilityData.Duration * 0.5f * GetAbilityDurationMultiplier();
        ActiveTyphoon = this;
        if( PD.Instance.UnlockMap.Get( UnlockFlags.TyphoonRoaringFlames )
            || PD.Instance.UnlockMap.Get( UnlockFlags.TyphoonExtendedBBQ ) )
        {
            Saw.Instance.SawKilledEnemyEvent.AddListener( OnSawKilledEnemy );
            listening = true;
        }
    }

    public override void Update( float delta_time )
    {
        base.Update( delta_time );
        time_remaining -= delta_time * GameplayManager.TimeScale;
        AnimatorDuration = time_remaining;
        if( time_remaining <= 0.0f && !Saw.Instance.OnFire )
            Finish();
    }

    public void SetSawOnFire( Saw saw )
    {
        if( !PD.Instance.UnlockMap.Get( UnlockFlags.TyphoonFlameSaw )
            || time_remaining <= 0.0f )
            return;
        saw.SetOnFire( AbilityData.FlameSawDuration, AbilityData.FlameSawExtraDamage, AbilityData.FlameSawMovementSpeedMultiplier );
    }

    public override void Finish()
    {
        ActiveTyphoon = null;
        if( listening )
        {
            Saw.Instance.SawKilledEnemyEvent.RemoveListener( OnSawKilledEnemy );
            listening = false;
        }
        base.Finish();
    }

    public override bool OnAbilityUsedWhileAlreadyActive()
    {
        SetDuration( time_remaining + AbilityData.Duration * GetAbilityDurationMultiplier() );
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

    private void OnSawKilledEnemy( Vector3 enemy_position )
    {
        if( Saw.Instance.OnFire )
        {
            if( PD.Instance.UnlockMap.Get( UnlockFlags.TyphoonRoaringFlames ) && time_remaining >= 0.0f )
            {
                SetDuration( time_remaining + roaring_flames_duration_carryover );
                roaring_flames_duration_carryover *= 0.5f; // infinitely decreasing geometric series
            }
            if( PD.Instance.UnlockMap.Get( UnlockFlags.TyphoonExtendedBBQ ) )
            {
                if( Random.Range( 0.0f, 1.0f ) < AbilityData.ExtendedBBQCorpseChance )
                {
                    TyphoonFlamingCorpse corpse = GameObject.Instantiate( AbilityData.FlamingCorpsePrefab );
                    corpse.Setup( AbilityData.ExtendedBBQRadius, AbilityData.ExtendedBBQDamageTickRate );
                    corpse.transform.position = enemy_position;
                }
            }
        }
    }

    public override void OnSceneExit()
    {
        ActiveTyphoon = null;
        AnimatorDuration = 0.0f;
        base.OnSceneExit();
    }
}
