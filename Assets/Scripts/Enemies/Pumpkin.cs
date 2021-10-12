using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pumpkin : Enemy
{
    protected override void Start()
    {
        if( PD.Instance.UnlockMap.Get( UnlockFlag.PumpkinUpgradeCurse ) )
        {
            MaxHealth += 1;
        }
        base.Start();
    }
}
