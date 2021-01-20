using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    public Animator HurtFX;
    // scales healthbar
    public void SetSize(float sizeNormalized)
    {
        transform.localScale = new Vector3(sizeNormalized, 1f);
        HurtFX.SetTrigger("Damaged");
    }

}
