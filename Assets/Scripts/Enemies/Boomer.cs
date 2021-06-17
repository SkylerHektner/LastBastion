using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Boomer : Enemy
{
    public float ExplosionRange = 1.0f;
    public float ExplosionDelay = 0.65f;

    private int layermask;

    protected override void Start()
    {
        base.Start();
        layermask = LayerMask.GetMask( "Enemy" );
    }

    public override void Kill()
    {
        Invoke( "Explode", ExplosionDelay );
        base.Kill();
    }

    private void Explode()
    {
#if SENSE_OF_HUMOR
        Debug.Log( "CURSE YOU MILLENIAL SCUM!" );
#endif

        Collider2D[] hit = Physics2D.OverlapCircleAll( transform.position, ExplosionRange, layermask );
        for( int x = 0; x < hit.Length; ++x )
        {
            Enemy hit_en = hit[x].gameObject.GetComponent<Enemy>();
            if( hit_en.EnemyID != EnemyID )
            {
                hit_en.Hit( ( hit_en.transform.position - transform.position ).normalized, true, DamageSource.BoomerDeathExplosion );
            }
        }

    }
}


// EDITOR
#if UNITY_EDITOR
[CustomEditor( typeof( Boomer ) )]
public class BoomerEditor : Editor
{
    private void OnSceneGUI()
    {
        Boomer boomer = (Boomer)target;
        {
            Handles.color = Color.red;
            Handles.DrawWireDisc( boomer.transform.position, Vector3.forward, boomer.ExplosionRange );
        }
    }
}
#endif