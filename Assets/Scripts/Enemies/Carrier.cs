using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrier : Enemy
{
    [SerializeField] SpawnGroup OnDeathSpawnGroup;

    protected override void Die()
    {
        if( OnDeathSpawnGroup )
            SpawnManager.Instance.SpawnSpawnGroup( OnDeathSpawnGroup, transform.position, false );
        base.Die();
    }
}
