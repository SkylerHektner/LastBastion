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

    protected override void Update()
    {
        base.Update();

        if( current_teleport_cooldown > 0.0f)
        {
            current_teleport_cooldown -= Time.deltaTime * GameplayManager.GamePlayTimeScale;
            TeleportFX.SetActive(true);
        }
        else
        {
            TeleportFX.SetActive(false);
        }
    }

    public override void Hit( Vector3 hit_direction, bool can_dodge, out bool died, out bool dodged, int damage = 1 )
    {
        if( teleporting )
        {
            died = false;
            dodged = true;
            return;
        }
        if( Moving && current_teleport_cooldown <= 0.0f && can_dodge && !Zapped && !StasisCoated )
        {
            last_saw_hit_direction = hit_direction;
            StopMoving();
            Teleport(hit_direction);
            died = false;
            dodged = true;
        }
        else
        {
            base.Hit( hit_direction, can_dodge, out died, out dodged, damage );
        }
    }

    private void Teleport(Vector3 hit_direction)
    {
        if(hit_direction != Vector3.zero)
        {
            Vector3 perp_clockwise = MathUtility.PerpendicularClockwise( hit_direction );
            Vector3 perp_counter_clockwise = MathUtility.PerpendicularCounterClockwise( hit_direction );
            teleport_direction = ( perp_clockwise.y > 0 ? perp_clockwise : perp_counter_clockwise ).normalized;
        }
        else
        {
            teleport_direction.x = 0; teleport_direction.y = 1; teleport_direction.z = 0;
        }
        
        anim.SetTrigger( "Teleport" );
        Invoke( "TeleportFinished", TeleportDuration );
        Invoke( "ChangePosition", TeleportDuration * 0.5f );
    }

    private void ChangePosition()
    {
        Vector3 new_pos = transform.position + teleport_direction * TeleportDistance;
        new_pos.y = Mathf.Clamp( new_pos.y, SpawnManager.Instance.PlayableAreaBottomLeft.y, SpawnManager.Instance.PlayableAreaTopRight.y );
        new_pos.x = Mathf.Clamp( new_pos.x, SpawnManager.Instance.PlayableAreaBottomLeft.y, SpawnManager.Instance.PlayableAreaTopRight.x );
        transform.position = new_pos;
    }

    private void TeleportFinished()
    {
        StartMoving();
        current_teleport_cooldown = TeleportCooldown;
    }
}
