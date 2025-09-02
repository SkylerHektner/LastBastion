using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveCounter : MonoBehaviour
{
    [SerializeField] TMP_Text WaveNumberText;
    [SerializeField] Animator Anim;
    public string CurrentWave;

    public void ShowNextWave(int wave, string totalWaves)
    {
        if (!GameplayManager.Instance.Survival)
        {
            WaveNumberText.SetText(wave.ToString() + "/" + totalWaves);
        }
        else
        {
            WaveNumberText.SetText(wave.ToString());

        }
        Anim.SetTrigger( "Appear" );
        CurrentWave = wave.ToString();
    }
}
