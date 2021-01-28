using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyphoonAbility : Ability
{
    public TyphoonAbilityData AbilityData;
    private float time_remaining = 0.0f;
    public static float AnimatorDuration;

    public override void Start()
    {
        base.Start();
        GameObject.Instantiate( AbilityData.Effect ).GetComponent<DeleteAfterDuration>().duration = AbilityData.duration;
        time_remaining = AbilityData.duration;
    }

    public override void Update( float delta_time )
    {
        base.Update( delta_time );
        time_remaining -= delta_time * GameplayManager.Instance.GetTimeScale(GameplayManager.TimeScale.UI);
        AnimatorDuration = time_remaining;
    }

    public override void Finish()
    {
        base.Finish();
    }
}
