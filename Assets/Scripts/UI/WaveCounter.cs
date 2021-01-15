using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveCounter : MonoBehaviour
{
    [SerializeField] TMP_Text WaveNumberText;
    [SerializeField] Animator Anim;

    public void ShowNextWave(int wave)
    {
        WaveNumberText.SetText( wave.ToString() );
        Anim.SetTrigger( "Appear" );
    }
}
