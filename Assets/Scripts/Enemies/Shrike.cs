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

    protected override void Update()
    {
        base.Update();

        if( current_teleport_cooldown > 0.0f )
            current_teleport_cooldown -= Time.deltaTime;
    }

    public override void Hit()
    {
        if( teleporting )
            return;
        if( Moving && current_teleport_cooldown <= 0.0f )
        {
            StopMoving();
            Teleport();
        }
        else
        {
            base.Hit();
        }
    }

    private void Teleport()
    {
        anim.SetTrigger( "Teleport" );
        Invoke( "TeleportFinished", TeleportDuration );
        Invoke( "ChangePosition", TeleportDuration * 0.5f);
    }

    private void ChangePosition()
    {
        Vector3 new_pos = new Vector3( transform.position.x, Mathf.Max( Rail.LeftRail.Bottom, transform.position.y - TeleportDistance ) );
        transform.position = new_pos;
    }

    private void TeleportFinished()
    {
        StartMoving();
        current_teleport_cooldown = TeleportCooldown;
    }
}
