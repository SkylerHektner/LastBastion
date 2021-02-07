using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectralSaw : Projectile
{
    public int NumRemainingExtraBounces = 0;

    protected override void Start()
    {
        base.Start();
        ProjectileHitWallEvent.AddListener( OnProjectileHitWall );
        SetTimeScaleFilter( GameplayManager.TimeScale.UI );
    }

    private void OnProjectileHitWall( ProjectileHitInfo hit_info )
    {
        if( NumRemainingExtraBounces > 0 )
        {
            --NumRemainingExtraBounces;
            SetWallHitBehavior( Projectile.WallHitBehavior.Bounce );
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
        }
        else if( col.tag == "AbilityDrop" )
        {
            AbilityDrop drop = col.gameObject.GetComponent<AbilityDrop>();
            if( !drop.JustSpawned )
                drop.AddAbilityCharge();
        }
    }
}
