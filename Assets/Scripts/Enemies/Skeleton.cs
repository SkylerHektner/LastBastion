using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{
    public float ShieldSkeletonUpgradeCurseMovespeedMultiplier = 0.7f;
    public override void Hit( Vector3 hit_direction, bool can_dodge, DamageSource source, out bool died, out bool dodged, int damage = 1 )
    {
        base.Hit( hit_direction, can_dodge, source, out died, out dodged, damage );

        if( !dodged &&
            !died &&
            source == DamageSource.Saw &&
            Saw.Instance.Moving &&
            PD.Instance.UnlockMap.Get( UnlockFlags.ShieldSkeletonUpgradeCurse ) )
        {
            Saw.Instance.SetShieldBreakMovespeedMultiplier( ShieldSkeletonUpgradeCurseMovespeedMultiplier );
        }
    }
}
