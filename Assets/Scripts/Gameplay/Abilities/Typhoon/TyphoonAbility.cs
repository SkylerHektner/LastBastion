using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyphoonAbility : Ability
{
    public TyphoonAbilityData AbilityData;
    public override void Start()
    {
        base.Start();
        GameObject.Instantiate( AbilityData.Effect ).GetComponent<DeleteAfterDuration>().duration = AbilityData.duration;
    }

    public override void Update( float delta_time )
    {
        base.Update( delta_time );
    }

    public override void Finish()
    {
        base.Finish();
    }
}
