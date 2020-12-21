using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeleteAfterDuration : MonoBehaviour
{
    public float duration = 5.0f;

    public UnityEvent DeleteDurationReached = new UnityEvent();

    public void Update()
    {
        duration -= Time.deltaTime;
        if( duration <= 0.0f )
        {
            DeleteDurationReached.Invoke();
            Destroy( gameObject );
        }
    }
}
