using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{

    // scales healthbar
    public void SetSize(float sizeNormalized)
    {
        transform.localScale = new Vector3(sizeNormalized, 1f);
    }

}
