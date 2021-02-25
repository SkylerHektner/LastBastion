using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "AnomalyAbilityData", menuName = "ScriptableObjects/Abilities/AnomalyAbilityData", order = 1 )]
public class AnomalyAbilityData : ScriptableObject
{
    public float Duration = 5.0f;
    public int RichochetSawExtraBounces = 1;
    public SpectralSaw SpectralSawPrefab;
    public GameObject StasisTouchEffectPrefab;
    public Material StasisTouchReplacementMaterial;
}
