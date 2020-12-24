using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent( typeof( Animator ) )]
public class Enemy : MonoBehaviour
{
    public UnityEvent<long> OnDeath = new UnityEvent<long>();
    public static long NextEnemyID = 1;
    public long EnemyID {
        get {
            if( enemyID == 0 )
            {
                enemyID = NextEnemyID++;
                if( NextEnemyID == long.MaxValue )
                    NextEnemyID = 0;
            }
            return enemyID == 0 ? ( enemyID = NextEnemyID++ ) : enemyID;
        }
    }
    private long enemyID = 0;

    [SerializeField] float MoveSpeed = 1.0f;
    [SerializeField] int MaxHealth = 1;
    [SerializeField] GameObject DeathEffect;
    [SerializeField] string DeathAnimation;
    [SerializeField] GameObject SpawnEffect;
    [SerializeField] string SpawnAnimation;
    [SerializeField] GameObject DamagedEffect;
    [SerializeField] string DamagedAnimation;
    public bool Moving { get; private set; }
    public bool Spawning { get; private set; }
    public bool Dying { get; private set; }
    public int CurrentHealth { get; private set; }

    protected Animator anim;
    protected SpriteRenderer s_rend;

    //BaseHP PlayerBase;

    protected virtual void Start()
    {
        if( NextEnemyID == long.MaxValue )
            NextEnemyID = 1;

        CurrentHealth = MaxHealth;
        GameplayManager.TimeScaleChanged.AddListener( OnTimeScaleChange );

        anim = GetComponent<Animator>();
        s_rend = GetComponent<SpriteRenderer>();
        if( SpawnEffect || ( SpawnAnimation != null && SpawnAnimation.Length > 0 ) )
            Spawn();
        else
            StartMoving();

        anim.speed = GameplayManager.GamePlayTimeScale;
    }

    public void DamageBase() //  Do my remaining health as damage to the base HP
    {
        BaseHP.Instance.ReduceHP( CurrentHealth );
    }


    protected virtual void Update()
    {
        if( Moving )
        {
            if( transform.position.y - SpawnManager.Instance.PlayableAreaBottomLeft.y < Time.deltaTime * MoveSpeed * GameplayManager.GamePlayTimeScale )
            {
                Moving = false;
                // TODO: Deal damage to base or attack or something
                DamageBase();
                Kill();
            }
            else
                transform.position = transform.position + Vector3.down * MoveSpeed * Time.deltaTime * GameplayManager.GamePlayTimeScale;
        }

        s_rend.sortingOrder = Convert.ToInt32( -transform.position.y * 100.0f );
    }

    public void Spawn()
    {
        if( SpawnEffect )
        {
            var spawn_effect = Instantiate( SpawnEffect );
            spawn_effect.transform.position = transform.position;
            spawn_effect.GetComponent<DeleteAfterDuration>()?.DeleteDurationReached.AddListener( SpawnEffectDone );
            gameObject.SetActive( false );
        }
        else
        {
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
            StartMoving();
        }
        else
        {
            anim.SetTrigger( "Spawn" );
            Invoke( "SpawnAnimationDone", 1.0f );
        }
    }

    public void SpawnAnimationDone()
    {
        Spawning = false;
        StartMoving();
    }

    public virtual void StartMoving()
    {
        Moving = true;
        anim.SetBool( "Attacking", Moving );
        anim.speed = GameplayManager.GamePlayTimeScale;
    }

    public void StopMoving()
    {
        Moving = false;
        anim.SetBool( "Attacking", Moving );
    }

    public virtual void Hit( Vector3 hit_direction, bool can_dodge )
    {
        if( Spawning || Dying ) return; // ignore being hit if we are spawning

        CurrentHealth--;
        if( CurrentHealth <= 0 )
            Kill();
        else
        {
            if( DamagedEffect != null )
                Instantiate( DamagedEffect ).transform.position = transform.position;
            if( DamagedAnimation != null && DamagedAnimation.Length > 0 )
                anim.SetTrigger( DamagedAnimation );
        }
    }

    public void Kill()
    {
        StopMoving();
        Dying = true;
        if( DeathEffect != null )
            Instantiate( DeathEffect ).transform.position = transform.position;
        if( DeathAnimation != null && DeathAnimation.Length != 0 )
        {
            anim.SetTrigger( DeathAnimation );
            Invoke( "Die", 1.0f );
        }
        else
            Die();
    }

    protected virtual void Die()
    {
        GameplayManager.TimeScaleChanged.RemoveListener( OnTimeScaleChange );
        Destroy( gameObject );
        OnDeath.Invoke( EnemyID );
    }

    private void OnTimeScaleChange()
    {
        anim.speed = GameplayManager.GamePlayTimeScale;
    }
}
