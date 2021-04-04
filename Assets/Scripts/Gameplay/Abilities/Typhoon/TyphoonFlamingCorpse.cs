using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyphoonFlamingCorpse : MonoBehaviour
{

    private float radius;
    private float tick_rate;

    private float tick_time = 0.0f;
    private int enemy_layermask;

    public void Setup( float radius, float tick_rate )
    {
        this.radius = radius;
        this.tick_rate = tick_rate;
        enemy_layermask = LayerMask.GetMask( "Enemy" );
    }

    private void Update()
    {
        tick_time -= Time.deltaTime * GameplayManager.TimeScale;

        if( tick_time <= 0.0f )
        {
            tick_time += tick_rate;

            Collider2D[] hit = Physics2D.OverlapCircleAll( transform.position, radius, enemy_layermask );
            for( int x = 0; x < hit.Length; ++x )
            {
                Enemy en = hit[x].gameObject.GetComponent<Enemy>();
                en.Hit( ( en.transform.position - transform.position ).normalized, true, DamageSource.TyphoonFlamingCorpse );
            }
        }
    }
}
