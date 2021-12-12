using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaman : Enemy
{
    [SerializeField] GameObject SummonEffect;
    [SerializeField] SpawnGroup SummonSpawnGroup;
    [SerializeField] SpawnGroup SecondarySummonSpawnGroup;
    [SerializeField] SpawnGroup TertiarySummonSpawnGroup;

    [SerializeField] SpawnGroup CurseSummonSpawnGroup; // the spawn group to use if our curse flag is active
    [SerializeField] string SummonAnimation;
    [SerializeField] float SummonDuration = 0.5f;
    [SerializeField] float SummonCooldown = 1.0f;
    [SerializeField] SpawnGroup OnDeathSpawnGroup;
    [SerializeField] bool UseSummonParticles;
    [SerializeField] ParticleSystem SummonParticles;
    [SerializeField] SFXEnum SummonSFX;

    private float cur_summon_cooldown = 0.0f;
    private SpawnGroup active_spawn_group;

    protected override void Start()
    {
        base.Start();
        Debug.Assert( SummonSpawnGroup );
        Debug.Assert( CurseSummonSpawnGroup );
        active_spawn_group = SummonSpawnGroup;
    }

    protected override void Update()
    {
        base.Update();

        if( !Dying && Moving && !string.IsNullOrEmpty( SummonAnimation ) )
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
            SpawnManager.Instance.SpawnSpawnGroup( OnDeathSpawnGroup, transform.position, SummonEffect );
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
        SpawnGroup group = PD.Instance.UnlockMap.Get( UnlockFlag.SummonerUpgradeCurse ) ? CurseSummonSpawnGroup : active_spawn_group;
        SpawnManager.Instance.SpawnSpawnGroup( group, transform.position, SummonEffect );
        Invoke( "FinishSummon", SummonDuration );
    }

    private void FinishSummon()
    {
        StartMoving();
    }

    public void SetSummonCooldown(float cooldown)
    {
        cur_summon_cooldown = cooldown;
        SummonCooldown = cooldown;
    }

    public void SetSummonGroup(int group_number)
    {
        switch( group_number )
        {
            case 1:
                active_spawn_group = SummonSpawnGroup;
                break;
            case 2:
                active_spawn_group = SecondarySummonSpawnGroup;
                break;
            case 3:
                active_spawn_group = TertiarySummonSpawnGroup;
                break;
        }
    }
}
