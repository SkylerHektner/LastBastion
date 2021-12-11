using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrike : Enemy
{
    [SerializeField] float TeleportDuration = 0.6f;
    [SerializeField] float TeleportCooldown = 2.0f;
    [SerializeField] float TeleportDistance = 2.0f;

    private float current_teleport_cooldown = 0.0f;
    private bool teleporting = false;
    private Vector3 teleport_direction = Vector3.zero;

    private Vector3 last_saw_hit_direction;
    public GameObject TeleportFX;
    public SFXEnum TeleportSound;

    protected override void Update()
    {
        base.Update();

        if( current_teleport_cooldown > 0.0f )
        {
            current_teleport_cooldown -= Time.deltaTime * GameplayManager.TimeScale;
            TeleportFX.SetActive( true );
        }
        else
        {
            TeleportFX.SetActive( false );
        }
    }

    public override void Hit( Vector3 hit_direction, bool can_dodge, DamageSource source, out bool died, out bool dodged, int damage = 1 )
    {
        if( teleporting )
        {
            died = false;
            dodged = true;
            return;
        }
        if( Moving && current_teleport_cooldown <= 0.0f && can_dodge && !Zapped && !StasisCoated && !Dying )
        {
            last_saw_hit_direction = hit_direction;
            StopMoving();
            Teleport( hit_direction );
            died = false;
            dodged = true;
        }
        else
        {
            base.Hit( hit_direction, can_dodge, source, out died, out dodged, damage );
        }
    }

    private void Teleport( Vector3 hit_direction )
    {
        if( hit_direction != Vector3.zero )
        {
            Vector3 perp_clockwise = MathUtility.PerpendicularClockwise( hit_direction );
            Vector3 perp_counter_clockwise = MathUtility.PerpendicularCounterClockwise( hit_direction );
            float desired_sign = PD.Instance.UnlockMap.Get( UnlockFlag.ShrikeUpgradeCurse ) ? -1.0f : 1.0f;
            teleport_direction = ( Mathf.Sign( perp_clockwise.y ) == Mathf.Sign( desired_sign )
                ? perp_clockwise : perp_counter_clockwise ).normalized;
        }
        else
        {
            teleport_direction.x = 0;
            teleport_direction.y = 1;
            teleport_direction.z = 0;
        }

        anim.SetTrigger( "Teleport" );
        Invoke( "TeleportFinished", TeleportDuration );
        Invoke( "ChangePosition", TeleportDuration * 0.5f );

        SFXManager.Instance.PlaySFX( TeleportSound );
    }

    private void ChangePosition()
    {
        Vector3 new_pos = transform.position + teleport_direction * TeleportDistance;
        Vector3 PlayableAreaBottomLeft = GameplayManager.Instance.ActiveEnvironment.PlayableAreaBottomLeft;
        Vector3 PlayableAreaTopRight = GameplayManager.Instance.ActiveEnvironment.PlayableAreaTopRight;

        new_pos.y = Mathf.Clamp( new_pos.y, PlayableAreaBottomLeft.y, PlayableAreaTopRight.y );
        new_pos.x = Mathf.Clamp( new_pos.x, PlayableAreaBottomLeft.y, PlayableAreaTopRight.x );
        transform.position = new_pos;
    }

    private void TeleportFinished()
    {
        if( !Zapped )
        {
            StartMoving();
        }
        else
        {
            StopMoving();
        }
        current_teleport_cooldown = TeleportCooldown;
    }
}
