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
        DestroyProjectile();
    }

    protected override void Start()
    {
        base.Start();
        ProjectileHitWallEvent.AddListener( OnProjectileHitWall );
    }

    private void OnProjectileHitWall( ProjectileHitInfo hit_info )
    {
        if( hit_info.wall == ProjectileHitInfo.Wall.Left ||
            hit_info.wall == ProjectileHitInfo.Wall.Right )
        {
            SetWallHitBehavior( WallHitBehavior.Destroy );
        }
        else if( hit_info.wall == ProjectileHitInfo.Wall.Bottom ||
            hit_info.wall == ProjectileHitInfo.Wall.Top )
        {
            SetWallHitBehavior( WallHitBehavior.Bounce );
        }
    }
}
