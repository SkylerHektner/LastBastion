using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghostie : Enemy
{
    [SerializeField] float VanishCooldown;
    [SerializeField] float VanishDuration;
    [SerializeField] string VanishAnimation;
    [SerializeField] SpawnGroup GhostFriendCurseSpawnGroup;

    float cur_vanish_cooldown = 0.0f;

    protected override void Start()
    {
        Debug.Assert( VanishCooldown != 0.0f );
        Debug.Assert( VanishDuration != 0.0f );
        Debug.Assert( !string.IsNullOrEmpty( VanishAnimation ) );
        Debug.Assert( GhostFriendCurseSpawnGroup != null );

        base.Start();

        cur_vanish_cooldown = VanishCooldown;
    }

    protected override void Update()
    {
        base.Update();

        if( Moving )
        {
            cur_vanish_cooldown -= Time.deltaTime * GameplayManager.TimeScale;
            if( cur_vanish_cooldown < Time.deltaTime )
            {
                cur_vanish_cooldown += VanishCooldown;
                Vanish();
            }
        }
    }

    private void Vanish()
    {
        StopMoving();
        anim.SetTrigger( VanishAnimation ); // this animation also disables their collider
        Invoke( "StopVanish", VanishDuration );
    }

    private void StopVanish()
    {
        StartMoving();
        if( PD.Instance.UnlockMap.Get( UnlockFlag.GhostUpgradeCurse ) )
        {
            SpawnManager.Instance.SpawnSpawnGroup( GhostFriendCurseSpawnGroup, transform.position );
        }
    }

}
