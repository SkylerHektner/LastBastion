using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent( typeof( Animator ) )]
public class Enemy : MonoBehaviour
{
    public UnityEvent OnDeath = new UnityEvent();

    [SerializeField] float MoveSpeed = 1.0f;
    [SerializeField] GameObject DeathEffect;
    [SerializeField] GameObject SpawnEffect;
    public bool Moving { get; private set; }
    public bool Spawning { get; private set; }

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if( SpawnEffect )
            Spawn();
        else
            StartMoving();
    }

    private void Update()
    {
        if( transform.position.y - Rail.LeftRail.Bottom < Time.deltaTime * MoveSpeed )
        {
            Moving = false;
            // TODO: Deal damage to base or attack or something
            Kill();
        }
        else
            transform.position = transform.position + Vector3.down * MoveSpeed * Time.deltaTime;
    }

    public void Spawn()
    {
        var spawn_effect = Instantiate( SpawnEffect );
        spawn_effect.transform.position = transform.position;
        spawn_effect.GetComponent<DeleteAfterDuration>()?.DeleteDurationReached.AddListener( SpawnAnimationDone );
        gameObject.SetActive( false );
        Spawning = true;
    }

    public void SpawnAnimationDone()
    {
        gameObject.SetActive( true );
        Spawning = false;
        StartMoving();
    }

    public void StartMoving()
    {
        Moving = true;
        anim.SetBool( "Attacking", Moving );
    }

    public void Kill()
    {
        if( DeathEffect != null )
            Instantiate( DeathEffect ).transform.position = transform.position;
        Destroy( gameObject );
        OnDeath.Invoke();
    }
}
