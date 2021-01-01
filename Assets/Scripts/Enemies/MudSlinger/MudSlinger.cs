using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudSlinger : Enemy
{
    [SerializeField] float MudThrowFrequency = 3.0f; // throws mud every x seconds
    [SerializeField] Projectile MudThrowProjectile;
    [SerializeField] string MudThrowAnimation;

    float mud_throw_cooldown = -1.0f;
    private bool vanished = false;

    private Projectile cur_mud_ball;

    protected override void Start()
    {
        base.Start();
        StopMoving();
        StartThrowMud();

        Debug.Assert( MudThrowFrequency > 0.0f );
        if( MudThrowFrequency < 0.0f )
            MudThrowFrequency = 3.0f; // someone messed up
    }

    protected override void Update()
    {
        base.Update();

        if( mud_throw_cooldown != -1.0f )
        {
            mud_throw_cooldown -= Time.deltaTime * GameplayManager.GamePlayTimeScale * ( Zapped ? 0.0f : 1.0f );

            if( mud_throw_cooldown <= 0.0f )
                StartThrowMud();
        }
    }

    private void StartThrowMud()
    {
        mud_throw_cooldown = -1.0f;
        anim.SetTrigger( MudThrowAnimation );
        vanished = false;
    }

    // triggered by the throw animation when we should throw some mud
    public void ThrowMud( AnimationEvent e )
    {
        cur_mud_ball = Instantiate( MudThrowProjectile );
        cur_mud_ball.transform.position = transform.position;
        cur_mud_ball.StartMoveInDirection( Saw.MainSaw.transform.position - transform.position );
        cur_mud_ball.ProjectileHitWallEvent.AddListener( OnProjectileHitWall );
    }

    private void OnProjectileHitWall( ProjectileHitInfo hit_info )
    {
        if( hit_info.wall == ProjectileHitInfo.Wall.Left ||
            hit_info.wall == ProjectileHitInfo.Wall.Right )
        {
            cur_mud_ball.SetWallHitBehavior( Projectile.WallHitBehavior.Destroy );
            cur_mud_ball = null;
        }
        else if( hit_info.wall == ProjectileHitInfo.Wall.Bottom ||
            hit_info.wall == ProjectileHitInfo.Wall.Top )
        {
            cur_mud_ball.SetWallHitBehavior( Projectile.WallHitBehavior.Bounce );
        }
    }

    // triggered by the vanish animation
    public void Vanish( AnimationEvent e )
    {
        vanished = true;
        mud_throw_cooldown = MudThrowFrequency;
    }

    public override void ZapForDuration( float duration )
    {
        base.ZapForDuration( duration );
        anim.enabled = false;
    }

    protected override void FinishZap()
    {
        base.FinishZap();
        anim.enabled = true;
    }

    protected override void Die()
    {
        if( cur_mud_ball != null )
            cur_mud_ball.ProjectileHitWallEvent.RemoveListener( OnProjectileHitWall );
        base.Die();
    }

    public override void Hit( Vector3 hit_direction, bool can_dodge )
    {
        if( !vanished )
            base.Hit( hit_direction, can_dodge );
    }
}
