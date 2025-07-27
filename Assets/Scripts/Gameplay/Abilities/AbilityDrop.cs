using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityDrop : MonoBehaviour
{
    public AbilityEnum ability;
    public float DespawnTimer;
    Animator Crystal;

    private void Start()
    {
        Crystal = gameObject.GetComponent<Animator>();
    }

    public void UseAbility()
    {
        AbilityManager.Instance.UseAbility( ability );
        GetComponent<Animator>().SetTrigger( "Crushed" );
        Invoke( "AnimEnd", 1.0f );

        // record player stats
        PD.Instance.NumCrystalsUsed.Set( PD.Instance.NumCrystalsUsed.Get() + 1 );
        Spectator.Instance?.SteamManagerInstance.IncrementNumCrystalsUsed();
    }

    public void Disintegrate()
    {
        GetComponent<Animator>().SetTrigger("Atomize");
        Invoke( "AnimEnd", 1.0f );
    }

    private void AnimEnd()
    {
        Destroy( gameObject );
    }

    private void FixedUpdate()
    {
        DespawnTimer -= Time.smoothDeltaTime * GameplayManager.TimeScale;
        Crystal.SetFloat("DespawnTimer", DespawnTimer);
    }
}
