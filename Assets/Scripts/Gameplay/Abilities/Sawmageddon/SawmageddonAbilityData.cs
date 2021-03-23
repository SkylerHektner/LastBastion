using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "SawmageddonAbilityData", menuName = "ScriptableObjects/Abilities/SawmageddonAbilityData", order = 1 )]
public class SawmageddonAbilityData : ScriptableObject
{
    public SpectralSaw SpectralSawPrefab;
    public float Duration = 5.0f;
    public float ImprovedDuration = 8.0f;
    public int NumberExtraSaws = 2;
    public int ImprovedNumberExtraSaws = 4;
    public float OffsetAngle = 15.0f;
    public int ComboKillerHPRegainKillsBase = 20;
    public float ComboKillerHPRegainScaleFactor = 1.5f;
}
