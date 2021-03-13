using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "TyphoonAbilityData", menuName = "ScriptableObjects/Abilities/TyphoonAbilityData", order = 1 )]
public class TyphoonAbilityData : ScriptableObject
{
    public GameObject Effect;
    public float Duration = 5.0f;
    public float FlameSawDuration = 4.0f;
    public int FlameSawExtraDamage = 1;
    public float FlameSawMovementSpeedMultiplier = 1.5f;
    public float ExtendedBBQRadius = 1.0f;
    public float ExtendedBBQDamageTickRate = 0.5f;
    public float ExtendedBBQCorpseChance = 0.35f; // 1.0 = 100%
    public TyphoonFlamingCorpse FlamingCorpsePrefab;
}
