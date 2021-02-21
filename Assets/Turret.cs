using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( CircleCollider2D ) )]
public class Turret : MonoBehaviour
{
    public float FireRate;
    public float RotationSpeed = 45.0f; // degrees per second
    public Projectile ProjectilePrefab;
    public Transform ProjectileSpawnPoint;

    private float range;
    private Enemy current_target;
    private Animator anim;
    private Quaternion start_rotation;

    private void Start()
    {
        range = GetComponent<CircleCollider2D>().radius;
        GetComponent<Animator>().SetFloat( "FireRate", FireRate ); // animation speed for firing anim
        anim = GetComponent<Animator>();
        start_rotation = transform.rotation;
    }

    private void Update()
    {
        ValidateCurrentTarget();
        if( current_target != null )
        {
            // enemy sighted, fire!
            bool facing = TryRotateToTarget();
            anim.SetBool( "Detected", facing );
        }
        else
        {
            // search for enemies
            anim.SetBool( "Detected", false );
            VisualSearchForEnemy();
        }
    }


    // return true if facing target
    private bool TryRotateToTarget()
    {
        if( current_target == null )
            return false;

        // what fresh hell is this
        Vector3 delta = current_target.transform.position - transform.position;
        float needed_z = -Mathf.Atan( delta.x / delta.y ) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler( 0, 0, needed_z ),
            RotationSpeed * Time.deltaTime * GameplayManager.GamePlayTimeScale );
        return Mathf.Abs( ( needed_z - transform.rotation.eulerAngles.z ) % 360.0f ) < 5.0f;
    }
    private void VisualSearchForEnemy()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            start_rotation,
            RotationSpeed * Time.deltaTime * GameplayManager.GamePlayTimeScale );
    }

    public void FireBullet() // called by pivot animator
    {
        ValidateCurrentTarget();
        if( current_target != null )
        {
            Projectile proj = GameObject.Instantiate( ProjectilePrefab );
            proj.transform.position = ProjectileSpawnPoint.position;
            proj.StartMoveInDirection( current_target.transform.position - transform.position );
        }
    }

    private void OnTriggerStay2D( Collider2D collision )
    {
        Enemy en = collision.attachedRigidbody.gameObject.GetComponent<Enemy>();
        Debug.Assert( en != null );
        ValidateCurrentTarget();
        if( current_target == null )
        {
            current_target = en;
        }
    }

    private void ValidateCurrentTarget()
    {
        if( current_target != null )
        {
            float allowed_range = range + current_target.GetComponent<CircleCollider2D>().radius + 1.0f;
            if( ( current_target.transform.position - transform.position ).sqrMagnitude > allowed_range * allowed_range )
            {
                current_target = null;
            }
        }
    }
}
