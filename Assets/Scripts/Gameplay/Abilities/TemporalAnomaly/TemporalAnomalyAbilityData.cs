using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "TemporalAnomalyAbilityData", menuName = "ScriptableObjects/Abilities/TemporalAnomalyAbilityData", order = 1 )]
public class TemporalAnomalyAbilityData : ScriptableObject
{
    public float Duration = 5.0f;
    public float GameplaySpeedMultiplier = 0.1f;
    public float GameplaySpeedLerpDuration = 0.2f;
    public SpectralSaw SpectralSawPrefab;
}
