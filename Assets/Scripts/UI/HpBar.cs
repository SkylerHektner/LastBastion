using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    public Animator HurtFX;
    public Animator HealFX;

    // scales healthbar
    public void SetSize( float sizeNormalized )
    {
        transform.localScale = new Vector3( sizeNormalized, 1f );
        if( HurtFX != null )
            HurtFX.SetTrigger( "Damaged" );
    }
    public void PlayHealAnim()
    {
        HealFX.SetTrigger("Heal");

    }

}
