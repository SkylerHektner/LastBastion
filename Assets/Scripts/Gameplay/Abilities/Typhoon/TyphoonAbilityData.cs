using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "TyphoonAbilityData", menuName = "ScriptableObjects/Abilities/TyphoonAbilityData", order = 1 )]
public class TyphoonAbilityData : ScriptableObject
{
    public GameObject Effect;
    public float duration = 5.0f;
}
