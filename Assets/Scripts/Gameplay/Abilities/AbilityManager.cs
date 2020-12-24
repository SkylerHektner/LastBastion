using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static readonly Dictionary<string, AbilityEnum> AbilityStringMap = new Dictionary<string, AbilityEnum>()
    {
        ["TemporalAnomaly"] = AbilityEnum.TemporalAnomaly,
        ["ChainLightning"] = AbilityEnum.ChainLightning,
        ["Typhoon"] = AbilityEnum.Typhoon,
        ["Sawmageddon"] = AbilityEnum.Sawmageddon,
    };
    public static AbilityManager Instance { get; private set; }

    public Vector3 BaseCenter;

    private void Start()
    {
        Instance = this;
    }

    public void UseAbility(AbilityEnum ability)
    {
        Debug.Log( ability.ToString() );
    }
}

public enum AbilityEnum
{
    TemporalAnomaly = 1,
    ChainLightning = 2,
    Typhoon = 3,
    Sawmageddon = 4,
}
