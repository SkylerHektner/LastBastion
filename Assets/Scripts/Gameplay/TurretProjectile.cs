using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectile : Projectile
{
    protected override void Start()
    {
        base.Start();
        ProjectileHitWallEvent.AddListener( OnProjectileHitWall );
    }
    private void OnProjectileHitWall( ProjectileHitInfo hit_info )
    {
        if( hit_info.wall == ProjectileHitInfo.Wall.Bottom )
        {
            SetWallHitBehavior( Projectile.WallHitBehavior.DoNothing );
        }
        else
        {
            SetWallHitBehavior( Projectile.WallHitBehavior.Destroy );
        }
    }

    private void OnTriggerEnter2D( Collider2D col )
    {
        if( col.tag == "Enemy" )
        {
            col.gameObject.GetComponent<Enemy>().Hit( MoveDirection, true );
            DestroyProjectile();
        }
    }
}
