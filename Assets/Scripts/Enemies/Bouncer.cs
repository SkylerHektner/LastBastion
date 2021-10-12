using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : Enemy
{
    public AnimatorOverrideController MagmaBouncerAnim; // magama bouncer animation overrides
    [SerializeField] GameObject MagmaSpawnEffect;

    protected override void Start()
    {
        if( PD.Instance.UnlockMap.Get( UnlockFlag.BouncerUpgradeCurse ) )
        {
            anim = GetComponent<Animator>();
            anim.runtimeAnimatorController = MagmaBouncerAnim;
            SpawnEffect = MagmaSpawnEffect;
            ImmuneToTyphoon = true;
            ImmuneToFlamingSawBonusDamage = true;
        }

        base.Start();
    }
}
