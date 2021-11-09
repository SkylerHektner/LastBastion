using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent( typeof( Animator ) )]
[RequireComponent( typeof( SpriteRenderer ) )]
public class Enemy : MonoBehaviour
{
    public EnemyEnum EnemyType;
    public UnityEvent<Enemy> DeathEvent = new UnityEvent<Enemy>();

    public static long NextEnemyID = 1;
    public long EnemyID
    {
        get
        {
            if( enemyID == 0 )
            {
                enemyID = NextEnemyID++;
                if( NextEnemyID == long.MaxValue )
                    NextEnemyID = 0;
            }
            return enemyID == 0 ? ( enemyID = NextEnemyID++ ) : enemyID;
        }
    }
    public DamageSource DeathSource { get; private set; } = DamageSource.UNSET;
    private long enemyID = 0;

    [SerializeField] float MoveSpeed = 1.0f;
    [SerializeField] protected int MaxHealth = 1;
    [SerializeField] GameObject DeathEffect;
    [SerializeField] string DeathAnimation;
    [SerializeField] protected GameObject SpawnEffect;
    [SerializeField] string SpawnAnimation;
    [SerializeField] GameObject DamagedEffect;
    [SerializeField] string DamagedAnimation;
    [SerializeField] string CurrentHealthAnimationParameter;
    [SerializeField] bool attacks = true;
    [SerializeField] private int powerupDropValue = 1;
    [SerializeField] bool ImmuneToZapBonusDamage;

    public bool ImmuneToTyphoon = false;
    public bool ImmuneToFlamingSawBonusDamage = false;
    public bool Bouncy = false; // if true, the saw will bounce off the creature upon colliding (bounce angle on circular collider)
    [SerializeField] int PlayerBaseBonusDamage;
    [SerializeField] SFXEnum DeathSFX;
    [SerializeField] SFXEnum HitSFX = SFXEnum.EnemyHit;
    [SerializeField] SFXEnum SpawnAnimationSFX; // plays when the spawn animation starts (ignored if no spawn anim)
    [SerializeField] SFXEnum SpawnEffectSFX; // plays when the spawn effect starts (ignored if no spawn effect)
    public int PowerupDropValue
    {
        get
        {
            return powerupDropValue * ( Zapped && PD.Instance.UnlockMap.Get( UnlockFlag.ChainLightningLightningRod ) ? 2 : 1 );
        }
    }

    public bool Moving { get; private set; }
    public bool Spawning { get; private set; }
    public bool Dying { get; private set; }
    public int CurrentHealth { get; private set; }

    protected Animator anim;
    protected SpriteRenderer s_rend;

    private float zap_duration = -1.0f;
    public bool Zapped { get { return zap_duration != -1.0f; } }
    public bool StasisCoated { get; protected set; }
    private Material original_mat;

    protected virtual void Start()
    {
        if( NextEnemyID == long.MaxValue )
            NextEnemyID = 1;

        CurrentHealth = MaxHealth;
        GameplayManager.Instance.TimeScaleChanged.AddListener( OnTimeScaleChange );

        anim = GetComponent<Animator>();
        s_rend = GetComponent<SpriteRenderer>();
        original_mat = s_rend.material;
        if( SpawnEffect || ( SpawnAnimation != null && SpawnAnimation.Length > 0 ) )
            Spawn();
        else
            StartMoving();

        anim.speed = GameplayManager.TimeScale;

        // if we haven't been registered, register ourselves (useful debugging feature for monsters that were in the scene at start)
        if( SpawnManager.Instance.TryGetEnemyByID( enemyID ) == null )
            SpawnManager.Instance?.RegisterEnemy( this );

    }

    public void DamageBase() //  Do my max health as damage to the base HP
    {
        BaseHP.Instance.ReduceHP( MaxHealth + PlayerBaseBonusDamage );

        // function overridden in Boomer
    }


    protected virtual void Update()
    {
        if( !string.IsNullOrEmpty( CurrentHealthAnimationParameter ) )
            anim.SetFloat( CurrentHealthAnimationParameter, CurrentHealth );

        if( Moving )
        {
            anim.SetBool( "Attacking", Moving );
            float move_delta = GetMoveSpeed()
                * Time.deltaTime
                * GameplayManager.TimeScale
                * GameplayManager.Instance.EnemyMoveSpeedCurseMultiplier;

            if( transform.position.y - GameplayManager.Instance.ActiveEnvironment.PlayableAreaBottomLeft.y < move_delta )
            {
                Moving = false;
                DamageBase();
                Kill();
            }
            else
            {
                transform.position = transform.position + Vector3.down * move_delta;
            }
        }

        if( zap_duration != -1.0f )
        {
            zap_duration -= Time.deltaTime * GameplayManager.TimeScale;
            if( zap_duration <= 0.0f )
                FinishZap();
        }

        s_rend.sortingOrder = Convert.ToInt32( -transform.position.y * 100.0f );
    }

    public void Spawn()
    {
        if( SpawnEffect )
        {
            SFXManager.Instance.PlaySFX( SpawnEffectSFX );
            var spawn_effect = Instantiate( SpawnEffect );
            spawn_effect.transform.position = transform.position;
            spawn_effect.GetComponent<DeleteAfterDuration>()?.DeleteDurationReached.AddListener( SpawnEffectDone );
            gameObject.SetActive( false );
        }
        else
        {
            SFXManager.Instance.PlaySFX( SpawnAnimationSFX );
            anim.SetTrigger( "Spawn" );
            Invoke( "SpawnAnimationDone", anim.GetCurrentAnimatorStateInfo( 0 ).length );
        }

        Spawning = true;

    }

    public void SpawnEffectDone()
    {
        gameObject.SetActive( true );
        if( SpawnAnimation == null || SpawnAnimation.Length == 0 )
        {
            Spawning = false;
            if( !Zapped && attacks && !StasisCoated )
                StartMoving();
        }
        else
        {
            SFXManager.Instance.PlaySFX( SpawnAnimationSFX );
            anim.SetTrigger( "Spawn" );
            Invoke( "SpawnAnimationDone", 1.0f );
        }
        if( !string.IsNullOrEmpty( CurrentHealthAnimationParameter ) )
            anim.SetFloat( CurrentHealthAnimationParameter, CurrentHealth );
    }

    public void SpawnAnimationDone()
    {
        Spawning = false;
        if( !Zapped && attacks && !StasisCoated )
            StartMoving();
    }

    public virtual void StartMoving()
    {
        if( attacks )
        {
            Moving = true;
            anim.SetBool( "Attacking", Moving );
        }
        anim.speed = GameplayManager.TimeScale;
    }

    public void StopMoving()
    {
        Moving = false;
        if( attacks )
            anim.SetBool( "Attacking", Moving );
    }

    public void Hit( Vector3 hit_direction, bool can_dodge, DamageSource source )
    {
        bool died;
        bool dodged;
        Hit( hit_direction, can_dodge, source, out died, out dodged );
    }

    public virtual void Hit( Vector3 hit_direction, bool can_dodge, DamageSource source, out bool died, out bool dodged, int damage = 1 )
    {
        dodged = false;
        if( Spawning || Dying )
        {
            dodged = true; // if they're spawning or dying did they dodge it? Hmmm. Sure, I guess so
            died = false;
            return; // ignore being hit if we are spawning
        }

        if( Zapped && !ImmuneToZapBonusDamage )
        {
            ++damage;
        }
        CurrentHealth -= damage;

        if( !string.IsNullOrEmpty( CurrentHealthAnimationParameter ) )
            anim.SetFloat( CurrentHealthAnimationParameter, CurrentHealth );

        SFXManager.Instance.PlaySFX( HitSFX );

        if( CurrentHealth <= 0 )
        {
            died = true;
            DeathSource = source;
            Kill();
        }
        else
        {
            died = false;
            if( DamagedEffect != null )
                Instantiate( DamagedEffect ).transform.position = transform.position;
            if( !string.IsNullOrEmpty( DamagedAnimation ) )
                anim.SetTrigger( DamagedAnimation );
        }
    }

    public virtual void Kill()
    {
        StopMoving();
        Dying = true;
        DeathEvent.Invoke( this );
        if( DeathEffect != null )
            Instantiate( DeathEffect ).transform.position = transform.position; // this is where they die
        if( DeathAnimation != null && DeathAnimation.Length != 0 )
        {
            anim.SetTrigger( DeathAnimation );
            Invoke( "Die", 1.0f );
        }
        else
            Die();

        SFXManager.Instance.PlaySFX( DeathSFX );
    }

    protected virtual void Die()
    {
        // record some stuff for player stats
        PD.Instance.NumKilledEnemies.Set( PD.Instance.NumKilledEnemies.Get() + 1 );
        if( Zapped )
            PD.Instance.NumZappedEnemiesKilled.Set( PD.Instance.NumZappedEnemiesKilled.Get() + 1 );
        if( DeathSource == DamageSource.Turret )
            PD.Instance.NumTurretKills.Set( PD.Instance.NumTurretKills.Get() + 1 );
        if( !PD.Instance.EncounteredEnemyList.Contains( EnemyType ) )
            PD.Instance.EncounteredEnemyList.Add( EnemyType );

        GameplayManager.Instance.TimeScaleChanged.RemoveListener( OnTimeScaleChange );
        Destroy( gameObject );
    }

    private void OnTimeScaleChange()
    {
        anim.speed = GameplayManager.TimeScale;
    }

    protected virtual void FinishZap()
    {
        zap_duration = -1.0f;
        StartMoving();
    }

    public virtual void ZapForDuration( float duration )
    {
        Debug.Assert( duration > 0.0f );

        if( Zapped )
        {
            zap_duration = Mathf.Max( duration, zap_duration );
            return;
        }

        StopMoving();
        zap_duration = duration;
    }

    public void StasisCoat( Material replacement_mat )
    {
        if( StasisCoated )
            return;
        s_rend.material = replacement_mat;
        StasisCoated = true;
        StopMoving();
    }

    public void EndStatisCoating()
    {
        s_rend.material = original_mat;
        StasisCoated = false;
        StartMoving();
    }

    protected virtual float GetMoveSpeed()
    {
        return MoveSpeed;
    }
}

public enum DamageSource
{
    Saw,
    FlamingSaw,
    SpectralSaw,
    Typhoon,
    TyphoonFlamingCorpse,
    StaticOverloadExplosion,
    Turret,
    BoomerDeathExplosion,
    UNSET,
}