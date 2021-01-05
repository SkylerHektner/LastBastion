using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "SawmageddonAbilityData", menuName = "ScriptableObjects/Abilities/SawmageddonAbilityData", order = 1 )]
public class SawmageddonAbilityData : ScriptableObject
{
    public SpectralSaw SpectralSawPrefab;
    public float Duration = 5.0f;
    public float NumberExtraSaws = 2.0f;
    public float OffsetAngle = 15.0f;
}
