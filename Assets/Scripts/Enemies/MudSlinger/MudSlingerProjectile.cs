using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudSlingerProjectile : Projectile
{
    [HideInInspector] public float SawSlowDuration;
    [HideInInspector] public float SawMoveSpeedMultiplier;
    public void HitSaw(Saw saw)
    {
        saw.CoverInMud( SawSlowDuration, SawMoveSpeedMultiplier );
    }
}
