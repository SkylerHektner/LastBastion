using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( CircleCollider2D ) )]
public class Turret : MonoBehaviour
{
    public float TrueRotationSpeed { get { return power_surge_timer > 0.0f ? PowerSurgeRotationSpeed : RotationSpeed; } }

    public float FireRate = 1.0f;
    public float PowerSurgeFireRate = 2.0f;
    public float RotationSpeed = 45.0f; // degrees per second
    public float PowerSurgeRotationSpeed = 70.0f;
    public float PowerSurgeDuration = 4.0f;
    public TurretProjectile ProjectilePrefab;
    public TurretProjectile SlowingProjectilePrefab;
    public TurretProjectile DefaultProjectile;
    public Transform ProjectileSpawnPoint;

    private float range;
    private Enemy current_target;
    private Animator anim;
    private Quaternion start_rotation;

    private float power_surge_timer = 0.0f;
    private bool collateral_damage_active = false;
    private bool timed_payload_active = false;

    public GameObject PowerSurgeGlowL;
    public GameObject PowerSurgeGlowR;
    public GameObject SawmageddonGlowL;
    public GameObject SawmageddonGlowR;
    public GameObject AnomalyGlowL;
    public GameObject AnomalyGlowR;


    private void Start()
    {
        range = GetComponent<CircleCollider2D>().radius;
        GetComponent<Animator>().SetFloat( "FireRate", FireRate ); // animation speed for firing anim
        anim = GetComponent<Animator>();
        start_rotation = transform.rotation;
        AbilityManager.Instance.AbilityUsedEvent.AddListener( OnAbilityUsed );
        ProjectilePrefab = DefaultProjectile;
    }

    private void Update()
    {
        ValidateCurrentTarget();
        if( current_target != null 
            && GameplayManager.PlayerWinState == GameplayManager.PlayerState.Active )
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

        if( power_surge_timer > 0.0f )
        {
            power_surge_timer -= Time.deltaTime * GameplayManager.TimeScale;
            if( power_surge_timer <= 0.0f )
                EndPowerSurge();
        }

        if( collateral_damage_active && SawmageddonAbility.ActiveSawmageddon == null )
        {
            EndCollateralDamage();
        }

        if( timed_payload_active && AnomalyAbility.ActiveAnomaly == null )
        {
            EndTimedPayload();
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
            TrueRotationSpeed * Time.deltaTime * GameplayManager.TimeScale );
        return Mathf.Abs( ( needed_z - transform.rotation.eulerAngles.z ) % 360.0f ) < 5.0f;
    }
    private void VisualSearchForEnemy()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            start_rotation,
            TrueRotationSpeed * Time.deltaTime * GameplayManager.TimeScale );
    }

    public void FireBullet() // called by pivot animator
    {
        ValidateCurrentTarget();
        if( current_target != null )
        {
            TurretProjectile proj = GameObject.Instantiate( ProjectilePrefab );
            proj.transform.position = ProjectileSpawnPoint.position;
            proj.StartMoveInDirection( current_target.transform.position - transform.position );
            if( collateral_damage_active )
                proj.ShouldPierce = true;
            if( timed_payload_active )
                proj.ShouldFreeze = true;
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

    private void OnDestroy()
    {
        AbilityManager.Instance?.AbilityUsedEvent.RemoveListener( OnAbilityUsed );
    }

    private void OnAbilityUsed( AbilityEnum ability )
    {
        if( ability == AbilityEnum.ChainLightning &&
            PD.Instance.UnlockMap.Get( UnlockFlags.TurretsPowerSurge ) )
        {
            StartPowerSurge();
        }
        else if( ability == AbilityEnum.Sawmageddon &&
            PD.Instance.UnlockMap.Get( UnlockFlags.TurretsCollateralDamage ) )
        {
            StartCollateralDamage();
        }
        else if( ability == AbilityEnum.Anomaly &&
            PD.Instance.UnlockMap.Get( UnlockFlags.TurretsTimedPaylod ) )
        {
            StartTimedPayload();
        }
    }

    private void StartPowerSurge()
    {
        power_surge_timer = PowerSurgeDuration;
        GetComponent<Animator>().SetFloat( "FireRate", PowerSurgeFireRate ); // animation speed for firing anim
        PowerSurgeGlowL.SetActive(true);
        PowerSurgeGlowR.SetActive(true);

    }

    private void EndPowerSurge()
    {
        power_surge_timer = 0.0f;
        GetComponent<Animator>().SetFloat( "FireRate", FireRate ); // animation speed for firing anim
        PowerSurgeGlowL.SetActive(false);
        PowerSurgeGlowR.SetActive(false);
    }

    private void StartCollateralDamage()
    {
        collateral_damage_active = true;
        SawmageddonGlowL.SetActive(true);
        SawmageddonGlowR.SetActive(true);
    }

    private void EndCollateralDamage()
    {
        collateral_damage_active = false;
        SawmageddonGlowL.SetActive(false);
        SawmageddonGlowR.SetActive(false);
    }

    private void StartTimedPayload()
    {
        timed_payload_active = true;
        ProjectilePrefab = SlowingProjectilePrefab;
        AnomalyGlowL.SetActive(true);
        AnomalyGlowR.SetActive(true);
    }

    private void EndTimedPayload()
    {
        timed_payload_active = false;
        ProjectilePrefab = DefaultProjectile;
        AnomalyGlowL.SetActive(false);
        AnomalyGlowR.SetActive(false);
    }
}
