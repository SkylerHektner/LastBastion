using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawmageddonAbility : Ability
{
    public SawmageddonAbilityData AbilityData;

    private float time_left = 0.0f;
    public static float AnimatorDuration;

    public override void Start()
    {
        base.Start();
        time_left = PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.SawmageddonDuration )
            ? AbilityData.ImprovedDuration : AbilityData.Duration;
        Saw.Instance?.SawFiredEvent?.AddListener( OnSawFired );
    }

    private void OnSawFired( Vector3 pos, Vector3 direction, float speed )
    {
        int num_extra_saws = PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.SawmageddonProjectiles )
            ? AbilityData.ImprovedNumberExtraSaws : AbilityData.NumberExtraSaws;
        for( int x = 0; x < num_extra_saws; ++x )
        {
            int angle_mult = ( x % 2 == 1 ? -1 : 1 ) * ( x / 2 + 1 );
            Vector3 new_direction = MathUtility.RotateVector2D( direction, AbilityData.OffsetAngle * Mathf.Deg2Rad * angle_mult );
            SpectralSaw spec_saw = GameObject.Instantiate( AbilityData.SpectralSawPrefab );
            spec_saw.gameObject.transform.position = pos;
            spec_saw.SetProjectileSpeed( speed );
            spec_saw.StartMoveInDirection( new_direction );
        }
    }

    public override void Finish()
    {
        Saw.Instance?.SawFiredEvent?.RemoveListener( OnSawFired );
        base.Finish();
    }

    public override void Update( float delta_time )
    {
        base.Update( delta_time );


        time_left -= delta_time * GameplayManager.GamePlayTimeScale;
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
}
