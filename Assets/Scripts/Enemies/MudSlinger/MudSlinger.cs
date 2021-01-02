using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudSlinger : Enemy
{
    [SerializeField] float MudThrowFrequency = 3.0f; // throws mud every x seconds
    [SerializeField] MudSlingerProjectile MudThrowProjectile;
    [SerializeField] string MudThrowAnimation;
    [SerializeField] float CoverInMudDuration = 2.0f;
    [SerializeField] float CoverInMudMoveSpeedMultiplier = 0.5f;

    float mud_throw_cooldown = -1.0f;
    private bool vanished = false;

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
        MudSlingerProjectile mud_ball = Instantiate( MudThrowProjectile );
        mud_ball.transform.position = transform.position;
        mud_ball.StartMoveInDirection( Saw.MainSaw.transform.position - transform.position );
        mud_ball.SawSlowDuration = CoverInMudDuration;
        mud_ball.SawMoveSpeedMultiplier = CoverInMudMoveSpeedMultiplier;
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

    public override void Hit( Vector3 hit_direction, bool can_dodge )
    {
        if( !vanished )
            base.Hit( hit_direction, can_dodge );
    }
}
