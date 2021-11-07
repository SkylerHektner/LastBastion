using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaman : Enemy
{
    [SerializeField] GameObject SummonEffect;
    [SerializeField] SpawnGroup SummonSpawnGroup;
    [SerializeField] SpawnGroup CurseSummonSpawnGroup; // the spawn group to use if our curse flag is active
    [SerializeField] string SummonAnimation;
    [SerializeField] float SummonDuration = 0.5f;
    [SerializeField] float SummonCooldown = 1.0f;
    [SerializeField] SpawnGroup OnDeathSpawnGroup;
    [SerializeField] bool UseSummonParticles;
    [SerializeField] ParticleSystem SummonParticles;
    [SerializeField] SFXEnum SummonSFX;

    private float cur_summon_cooldown = 0.0f;

    protected override void Start()
    {
        base.Start();
        Debug.Assert( SummonSpawnGroup );
        Debug.Assert( CurseSummonSpawnGroup );
    }

    protected override void Update()
    {
        base.Update();

        if( Moving && !string.IsNullOrEmpty( SummonAnimation ) )
        {
            cur_summon_cooldown -= Time.deltaTime * GameplayManager.TimeScale;
            if( cur_summon_cooldown <= 0.0f )
            {
                StopMoving();
                Summon();
            }
        }
    }

    public override void Kill()
    {
        if( OnDeathSpawnGroup )
            SpawnManager.Instance.SpawnSpawnGroup( OnDeathSpawnGroup, transform.position, false );
        base.Kill();
    }

    public override void StartMoving()
    {
        base.StartMoving();
        cur_summon_cooldown = SummonCooldown;
    }

    private void Summon()
    {
        if (UseSummonParticles)
        {
            SummonParticles.Play();
            SFXManager.Instance.PlaySFX(SummonSFX);
        }
        anim.SetTrigger( SummonAnimation );
        SpawnGroup group = PD.Instance.UnlockMap.Get( UnlockFlag.SummonerUpgradeCurse ) ? CurseSummonSpawnGroup : SummonSpawnGroup;
        List<Vector3> spawn_points = SpawnManager.Instance.SpawnSpawnGroup( group, transform.position, false );
        foreach( var p in spawn_points )
            Instantiate( SummonEffect ).transform.position = p;
        Invoke( "FinishSummon", SummonDuration );
    }

    private void FinishSummon()
    {
        StartMoving();
    }
}
