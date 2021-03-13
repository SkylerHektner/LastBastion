using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrier : Enemy
{
    [SerializeField] SpawnGroup OnDeathSpawnGroup;

    public override void Kill()
    {
        if( OnDeathSpawnGroup )
            SpawnManager.Instance.SpawnSpawnGroup( OnDeathSpawnGroup, transform.position, false );
        base.Kill();
    }
}
