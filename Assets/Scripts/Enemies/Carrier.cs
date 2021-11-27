using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrier : Enemy
{
    [SerializeField] SpawnGroup OnDeathSpawnGroup;
    [SerializeField] SpawnGroup CurseOnDeathSpawnGroup; // the spawn group for if you have the carrier curse

    protected override void Start()
    {
        base.Start();
        if( OnDeathSpawnGroup != null && CurseOnDeathSpawnGroup == null )
            Debug.LogWarning( "WARNING: Carrier missing curse on death spawn group - is this an oversight?" );
    }

    public override void Kill()
    {
        SpawnGroup group = PD.Instance.UnlockMap.Get( UnlockFlag.CarrierUpgradeCurse ) ?
            CurseOnDeathSpawnGroup : OnDeathSpawnGroup;
        if( group )
            SpawnManager.Instance.SpawnSpawnGroup( group, transform.position );
        base.Kill();
    }
}
