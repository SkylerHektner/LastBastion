using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpectralSaw : Projectile
{
    public int NumRemainingExtraBounces = 0;
    public UnityEvent<Vector3> SawKilledEnemyEvent = new UnityEvent<Vector3>();
    public UnityEvent SawReleasedEvent = new UnityEvent();

    private HashSet<long> shared_collision_set = null;

    protected override void Start()
    {
        base.Start();
        ProjectileHitWallEvent.AddListener( OnProjectileHitWall );
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
                bool died;
                bool dodged;
                en.Hit( MoveDirection, true, DamageSource.SpectralSaw, out died, out dodged );
                shared_collision_set?.Add( en.EnemyID );
                if( died )
                {
                    SawKilledEnemyEvent.Invoke( en.transform.position );
                }
            }
        }
        else if( col.tag == "AbilityDrop" )
        {
            AbilityDrop drop = col.gameObject.GetComponent<AbilityDrop>();
            drop.UseAbility();
        }
    }

    // allows for the spectral saw to track a shared collision set with other spectral saws
    // and the main saw to ensure that they don't hit a single enemy more than once if they share a set
    public void SetSharedCollisionSet( HashSet<long> collision_set )
    {
        shared_collision_set = collision_set;
        Saw.Instance.SawHitEnemyEvent.AddListener( OnSawHitEnemy );
    }

    public override void StartMoveInDirection( Vector3 move_direction )
    {
        base.StartMoveInDirection( move_direction );
        SawReleasedEvent.Invoke();
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
