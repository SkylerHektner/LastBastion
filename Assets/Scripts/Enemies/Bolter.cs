using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolter : Enemy
{
    public float CurseAdditionalMovespeed = 1.0f;
    public float CurseZigZagSpeed = 2.0f;

    private bool zagging_left;

    protected override float GetMoveSpeed()
    {
        return base.GetMoveSpeed() +
            ( PD.Instance.UnlockMap.Get( UnlockFlag.BolterUpgradeCurse ) ? CurseAdditionalMovespeed : 0.0f );
    }

    protected override void Start()
    {
        base.Start();
        zagging_left = Random.value < 0.5f;
    }

    protected override void Update()
    {
        base.Update();

        if( Moving && PD.Instance.UnlockMap.Get( UnlockFlag.BolterUpgradeCurse ) )
        {
            // zig to the zag
            Vector3 zag_direction = zagging_left ? Vector3.left : Vector3.right;
            float delta = CurseZigZagSpeed 
                * Time.deltaTime 
                * GameplayManager.TimeScale
                * GameplayManager.Instance.EnemyMoveSpeedCurseMultiplier;

            Vector3 new_position = transform.position + zag_direction * delta;
            if( new_position.x < GameplayManager.Instance.ActiveEnvironment.PlayableAreaBottomLeft.x )
            {
                new_position.x = GameplayManager.Instance.ActiveEnvironment.PlayableAreaBottomLeft.x;
                zagging_left = !zagging_left;
            }
            else if( new_position.x > GameplayManager.Instance.ActiveEnvironment.PlayableAreaTopRight.x )
            {
                new_position.x = GameplayManager.Instance.ActiveEnvironment.PlayableAreaTopRight.x;
                zagging_left = !zagging_left;
            }

            transform.position = new_position;
        }
    }
}
