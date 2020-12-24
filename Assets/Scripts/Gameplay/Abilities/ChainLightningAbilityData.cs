using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "ChainLightningAbilityData", menuName = "ScriptableObjects/Abilities/ChainLightningAbilityData", order = 1 )]
public class ChainLightningAbilityData : ScriptableObject
{
    public ChainLightningEffect Effect;
    public float TimeBetweenZaps = 0.1f;
}