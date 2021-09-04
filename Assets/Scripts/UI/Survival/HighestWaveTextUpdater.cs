using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class HighestWaveTextUpdater : MonoBehaviour
{
    public void Awake()
    {
        GetComponent<TMPro.TextMeshProUGUI>().SetText( PD.Instance.HighestSurvivalWave.Get().ToString() );
    }
}
