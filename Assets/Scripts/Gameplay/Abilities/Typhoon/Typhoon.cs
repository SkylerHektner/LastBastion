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

            // hit em till they dead
            while( en != null && !en.Dying )
                en.Hit( Vector3.up, false );
        }
    }
}
