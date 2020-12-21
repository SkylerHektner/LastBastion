using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterDuration : MonoBehaviour
{
    public float duration = 5.0f;

    public void Update()
    {
        duration -= Time.deltaTime;
        if( duration <= 0.0f )
            Destroy( gameObject );
    }
}
