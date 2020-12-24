using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaman : Enemy
{
    [SerializeField] GameObject SummonEffect;
    [SerializeField] SpawnGroup SummonSpawnGroup;
    [SerializeField] string SummonAnimation;
    [SerializeField] float SummonDuration = 0.5f;
    [SerializeField] float SummonCooldown = 1.0f;

    private float cur_summon_cooldown = 0.0f;

    protected override void Update()
    {
        base.Update();

        if( Moving )
        {
            cur_summon_cooldown -= Time.deltaTime * GameplayManager.GamePlayTimeScale;
            if(cur_summon_cooldown <= 0.0f)
            {
                StopMoving();
                Summon();
            }
        }
    }

    public override void StartMoving()
    {
        base.StartMoving();
        cur_summon_cooldown = SummonCooldown;
    }

    private void Summon()
    {
        anim.SetTrigger( SummonAnimation );
        List<Vector3> spawn_points = SpawnManager.Instance.SpawnSpawnGroup( SummonSpawnGroup, transform.position, false );
        foreach( var p in spawn_points )
            Instantiate( SummonEffect ).transform.position = p;
        Invoke( "FinishSummon", SummonDuration );
    }

    private void FinishSummon()
    {
        StartMoving();
    }
}
