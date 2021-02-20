using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "ChainLightningAbilityData", menuName = "ScriptableObjects/Abilities/ChainLightningAbilityData", order = 1 )]
public class ChainLightningAbilityData : ScriptableObject
{
    public ChainLightningEffect Effect;
    public DeleteAfterDuration ZappedEffect;
    public GameObject SceneWideEffect;
    public float TimeBetweenZaps = 0.1f; // mostly cosmetic - the delay between each cycle of zaps as the lightning branches
    public float ZapDuration = 2.0f;
    public float ImprovedZapDuration = 4.0f;
    public float StaticOverloadExplosionRadius = 1.0f;
    public GameObject StaticOverloadExplosionEffect;
}