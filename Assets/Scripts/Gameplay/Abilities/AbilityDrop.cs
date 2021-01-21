using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDrop : MonoBehaviour
{
    public AbilityEnum ability;
    public bool JustSpawned = true;

    private void Start()
    {
        Invoke( "ToggleJustSpawnedOff", 0.1f );
    }

    private void ToggleJustSpawnedOff()
    {
        JustSpawned = false;
    }

    public void AddAbilityCharge()
    {
        AbilityManager.Instance.AddAbilityCharge( ability );
        GetComponent<Animator>().SetTrigger( "Crushed" );
        GetComponent<CircleCollider2D>().enabled = false;
        Invoke( "AnimEnd", 1.0f );
    }

    private void AnimEnd()
    {
        Destroy( gameObject );
    }
}
