using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    public float MoveSpeed = 1.0f;


    public bool Moving { get; private set; }

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        StartMoving();
    }

    private void Update()
    {
        if( transform.position.y - Rail.LeftRail.Bottom < Time.deltaTime * MoveSpeed )
        {
            Moving = false;
            Destroy( gameObject );
            // TODO: Deal damage to base or attack or something
        }
        else
            transform.position = transform.position + Vector3.down * MoveSpeed * Time.deltaTime;
    }

    public void StartMoving()
    {
        Moving = true;
        anim.SetBool( "Attacking", Moving );
    }
}
