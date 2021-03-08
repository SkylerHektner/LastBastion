using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityDrop : MonoBehaviour
{
    public AbilityEnum ability;
    public bool JustSpawned = true;
    public float DespawnTimer;
    Animator Crystal;

    private void Start()
    {
        Invoke( "ToggleJustSpawnedOff", 0.1f );
        Crystal = gameObject.GetComponent<Animator>();
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

    private void FixedUpdate()
    {
        DespawnTimer -= Time.smoothDeltaTime * GameplayManager.GamePlayTimeScale;
        Crystal.SetFloat("DespawnTimer", DespawnTimer);
    }
}
