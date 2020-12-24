using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static readonly Dictionary<string, Ability> AbilityStringMap = new Dictionary<string, Ability>()
    {
        ["TemporalAnomaly"] = Ability.TemporalAnomaly,
        ["ChainLightning"] = Ability.ChainLightning,
        ["Typhoon"] = Ability.Typhoon,
        ["Sawmageddon"] = Ability.Sawmageddon,
    };

    public static AbilityManager Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    public void UseAbility(Ability ability)
    {
        Debug.Log( ability.ToString() );
    }
}

public enum Ability
{
    TemporalAnomaly = 1,
    ChainLightning = 2,
    Typhoon = 3,
    Sawmageddon = 4,
}
