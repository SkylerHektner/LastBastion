using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent( typeof( Animator ) )]
public class Enemy : MonoBehaviour
{
    public UnityEvent OnDeath = new UnityEvent();

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

    protected virtual void Start()
    {
        CurrentHealth = MaxHealth;

        anim = GetComponent<Animator>();
        if( SpawnEffect || ( SpawnAnimation != null && SpawnAnimation.Length > 0 ) )
            Spawn();
        else
            StartMoving();
    }

    protected virtual void Update()
    {
        if( Moving )
        {
            if( transform.position.y - SpawnManager.Instance.PlayableAreaBottomLeft.y < Time.deltaTime * MoveSpeed )
            {
                Moving = false;
                // TODO: Deal damage to base or attack or something
                Kill();
            }
            else
                transform.position = transform.position + Vector3.down * MoveSpeed * Time.deltaTime;
        }
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
    }

    public void StopMoving()
    {
        Moving = false;
        anim.SetBool( "Attacking", Moving );
    }

    public virtual void Hit( Vector3 hit_direction )
    {
        if( Spawning || Dying ) return; // ignore being hit if we are spawning

        CurrentHealth--;
        if( CurrentHealth <= 0 )
            Kill();
        else
        {
            if( DamagedEffect != null )
                Instantiate( DamagedEffect ).transform.position = transform.position;
            if(DamagedAnimation != null && DamagedAnimation.Length > 0)
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
        Destroy( gameObject );
        OnDeath.Invoke();
    }
}
