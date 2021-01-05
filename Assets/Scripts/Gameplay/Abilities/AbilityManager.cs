using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public GameObject AbilityInfoScroll;

    public static readonly Dictionary<string, AbilityEnum> AbilityStringMap = new Dictionary<string, AbilityEnum>()
    {
        ["TemporalAnomaly"] = AbilityEnum.TemporalAnomaly,
        ["ChainLightning"] = AbilityEnum.ChainLightning,
        ["Typhoon"] = AbilityEnum.Typhoon,
        ["Sawmageddon"] = AbilityEnum.Sawmageddon,
    };
    public static AbilityManager Instance { get; private set; }

    public Vector3 BaseCenter;
    public ChainLightningAbilityData ChainLightningData;
    public TyphoonAbilityData TyphoonData;
    public SawmageddonAbilityData SawmageddonData;

    private Dictionary<long, Ability> active_abilities = new Dictionary<long, Ability>();
    private List<long> pending_removals = new List<long>();

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        foreach( var ab in active_abilities.Values )
            ab.Update( Time.deltaTime );
        foreach( var r in pending_removals )
            active_abilities.Remove( r );
        pending_removals.Clear();
    }

    public void UseAbility(AbilityEnum ability)
    {
        Ability ab = null;
        switch( ability )
        {
            case AbilityEnum.TemporalAnomaly:
                break;
            case AbilityEnum.ChainLightning:
                {
                    ChainLightningAbility _ab = new ChainLightningAbility();
                    _ab.AbilityData = ChainLightningData;
                    ab = _ab;
                    break;
                }
            case AbilityEnum.Typhoon:
                {
                    TyphoonAbility _ab = new TyphoonAbility();
                    _ab.AbilityData = TyphoonData;
                    ab = _ab;
                    break;
                }
            case AbilityEnum.Sawmageddon:
                {
                    SawmageddonAbility _ab = new SawmageddonAbility();
                    _ab.AbilityData = SawmageddonData;
                    ab = _ab;
                    break;
                }
        }

        Debug.Assert( ab != null, "ERROR: it seems triggered ability " + ability.ToString() + " is not mapped to an Ability class" );
        if(ab != null)
        {
            ab.AM = this;
            ab.name = ability.ToString();
            active_abilities.Add( ab.AbilityID, ab );
            ab.Start();
        }
        AbilityInfoScroll.SetActive(false); // hi, I added this.  It just turns off the scroll.
    }

    public void AbilityFinished(long AbilityID)
    {
        Debug.Assert( active_abilities.ContainsKey( AbilityID ) );
        pending_removals.Add( AbilityID );
    }

    [ContextMenu("Print All Active Abilities")]
    private void DebugLogAllActiveAbilities()
    {
        foreach(var ab in active_abilities)
            Debug.Log( ab.Value.name );
    }
}

public enum AbilityEnum
{
    TemporalAnomaly = 1,
    ChainLightning = 2,
    Typhoon = 3,
    Sawmageddon = 4,
}
