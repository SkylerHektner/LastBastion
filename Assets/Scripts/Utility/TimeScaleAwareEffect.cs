using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TimeScaleAwareEffect : MonoBehaviour
{
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        GameplayManager.Instance.TimeScaleChanged.AddListener( OnTimeScaleChanged );
        anim.speed = GameplayManager.TimeScale;
    }

    private void OnTimeScaleChanged()
    {
        anim.speed = GameplayManager.TimeScale;
    }

    private void OnDestroy()
    {
        GameplayManager.Instance?.TimeScaleChanged.RemoveListener( OnTimeScaleChanged );
    }
}
