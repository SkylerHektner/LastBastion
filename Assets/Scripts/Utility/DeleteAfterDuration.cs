using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeleteAfterDuration : MonoBehaviour
{
    public float duration = 5.0f;

    public UnityEvent DeleteDurationReached = new UnityEvent();

    public bool IgnoreGameplayTimeScale = false;

    public void Update()
    {
        duration -= Time.deltaTime 
            * ( IgnoreGameplayTimeScale ? 1.0f : GameplayManager.TimeScale );
        if( duration <= 0.0f )
        {
            DeleteDurationReached.Invoke();
            Destroy( gameObject );
        }
    }

    public void DestroyOnDeathHook(Enemy en)
    {
        en.DeathEvent.RemoveListener( DestroyOnDeathHook );
        Destroy( gameObject );
    }
}
