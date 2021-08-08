using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : Enemy
{
    public float PullStrength = 1.0f;
    public SpawnGroup CurseSpawnGroup;
    public float CurseSpawnCooldown;

    private float cur_spawn_cooldown;

    protected override void Start()
    {
        Debug.Assert( CurseSpawnGroup );
        Debug.Assert( CurseSpawnCooldown != 0.0f );

        base.Start();

        cur_spawn_cooldown = CurseSpawnCooldown;
    }

    protected override void Update()
    {
        base.Update();
        if( !Zapped && !Spawning && !Dying && Saw.Instance.Moving )
        {
            Vector3 pull_direction = ( transform.position - Saw.Instance.transform.position );

            Saw.Instance.SetMoveDirection(
                Saw.Instance.MoveDirection + pull_direction.normalized * Time.deltaTime * PullStrength,
                Saw.Instance.AdjustedMoveSpeed + Mathf.Min( ( 1.0f / pull_direction.sqrMagnitude ) * PullStrength, 10.0f ) );
        }

        if( PD.Instance.UnlockMap.Get( UnlockFlags.BlackholeUpgradeCurse ) )
        {
            cur_spawn_cooldown -= Time.deltaTime * GameplayManager.TimeScale;
            if( cur_spawn_cooldown <= 0.0f )
            {
                cur_spawn_cooldown = CurseSpawnCooldown;
                SpawnManager.Instance.SpawnSpawnGroup( CurseSpawnGroup, transform.position, true );
            }
        }
    }
}
