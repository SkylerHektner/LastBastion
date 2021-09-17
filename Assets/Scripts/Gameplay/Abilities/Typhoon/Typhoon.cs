using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Typhoon : MonoBehaviour
{
    private void OnTriggerEnter2D( Collider2D col )
    {
        if( col.tag == "Enemy" )
        {

            Enemy en = col.gameObject.GetComponent<Enemy>();
            bool died = false;
            bool dodged = false;

            // hit em till they dead
            while( en != null && !en.Dying && !en.ImmuneToTyphoon )
                en.Hit( Vector3.up, false, DamageSource.Typhoon, out died, out dodged );

            if( died )
                PD.Instance.NumEnemiesKilledByTyphoon.Set( PD.Instance.NumEnemiesKilledByTyphoon.Get() + 1 );
        }
    }
}
