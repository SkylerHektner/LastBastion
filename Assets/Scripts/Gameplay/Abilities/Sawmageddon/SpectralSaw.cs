using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectralSaw : Projectile
{
    protected override void Start()
    {
        base.Start();
        ProjectileHitWallEvent.AddListener( OnProjectileHitWall );
    }

    private void OnProjectileHitWall( ProjectileHitInfo hit_info )
    {
        if( hit_info.wall == ProjectileHitInfo.Wall.Bottom ||
            hit_info.wall == ProjectileHitInfo.Wall.Top )
        {
            SetWallHitBehavior( Projectile.WallHitBehavior.Destroy );
        }
        else if( hit_info.wall == ProjectileHitInfo.Wall.Left ||
            hit_info.wall == ProjectileHitInfo.Wall.Right )
        {
            SetWallHitBehavior( Projectile.WallHitBehavior.Destroy );
        }
    }

    private void OnTriggerEnter2D( Collider2D col )
    {
        if( col.tag == "Enemy" )
            col.gameObject.GetComponent<Enemy>().Hit( MoveDirection, true );
    }
}
