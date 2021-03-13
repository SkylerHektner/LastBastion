using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectralSaw : Projectile
{
    public int NumRemainingExtraBounces = 0;

    private HashSet<long> shared_collision_set = null;

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
            Enemy en = col.gameObject.GetComponent<Enemy>();
            if( shared_collision_set == null || !shared_collision_set.Contains( en.EnemyID ) )
            {
                en.Hit( MoveDirection, true );
                shared_collision_set?.Add( en.EnemyID );
            }
        }
        else if( col.tag == "AbilityDrop" )
        {
            AbilityDrop drop = col.gameObject.GetComponent<AbilityDrop>();
            if( !drop.JustSpawned )
                drop.AddAbilityCharge();
        }
    }

    // allows for the spectral saw to track a shared collision set with other spectral saws
    // and the main saw to ensure that they don't hit a single enemy more than once if they share a set
    public void SetSharedCollisionSet( HashSet<long> collision_set )
    {
        shared_collision_set = collision_set;
        Saw.Instance.SawHitEnemyEvent.AddListener( OnSawHitEnemy );
    }

    private void OnSawHitEnemy( long id )
    {
        if( !shared_collision_set.Contains( id ) )
        {
            shared_collision_set.Add( id );
        }
    }

    private void OnDestroy()
    {
        if( shared_collision_set != null )
        {
            Saw.Instance.SawHitEnemyEvent.RemoveListener( OnSawHitEnemy );
        }
    }
}
