using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : Enemy
{
    public float PullStrength = 1.0f;
    protected override void Update()
    {
        base.Update();
        if( !Zapped && !Spawning && !Dying && Saw.Instance.Moving )
        {
            Vector3 pull_direction = ( transform.position - Saw.Instance.transform.position );

            Saw.Instance.SetMoveDirection(
                Saw.Instance.MoveDirection + pull_direction.normalized * Time.deltaTime * PullStrength,
                Saw.Instance.AdjustedMoveSpeed + Mathf.Min( ( 1.0f / pull_direction.sqrMagnitude ) * PullStrength, 10.0f ) );
        }
    }
}
