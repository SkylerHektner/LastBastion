using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }
    public UnityEvent<AbilityEnum, int> AbilityChargeChangedEvent = new UnityEvent<AbilityEnum, int>();

    [SerializeField] GameObject AbilityInfoScroll;
    public Vector3 BaseCenter;
    [SerializeField] AnomalyAbilityData AnomalyData;
    [SerializeField] ChainLightningAbilityData ChainLightningData;
    [SerializeField] TyphoonAbilityData TyphoonData;
    [SerializeField] SawmageddonAbilityData SawmageddonData;
    public int MaxAbilityCharges = 3;

    private Dictionary<long, Ability> active_abilities = new Dictionary<long, Ability>();
    private List<long> pending_removals = new List<long>();
    private List<int> ability_charges = new List<int>();

    private void Start()
    {
        Instance = this;
        for( int x = 0; x < (int)AbilityEnum.NUM_ABILITIES; ++x )
            ability_charges.Add( 0 );
    }

    private void Update()
    {
        foreach( var ab in active_abilities.Values )
            ab.Update( Time.deltaTime );
        foreach( var r in pending_removals )
            active_abilities.Remove( r );
        pending_removals.Clear();
    }

    public void AddAbilityCharge(AbilityEnum ability)
    {
        Debug.Assert( ability != AbilityEnum.NUM_ABILITIES );
        if( ability_charges[(int)ability] < MaxAbilityCharges)
        {
            ability_charges[(int)ability]++;
            AbilityChargeChangedEvent.Invoke( ability, ability_charges[(int)ability] );
        }
    }

    public int GetAbilityCharges(AbilityEnum ability)
    {
        Debug.Assert( ability != AbilityEnum.NUM_ABILITIES );
        return ability_charges[(int)ability];
    }

    public bool UseAbility(AbilityEnum ability)
    {
        if( ability_charges[(int)ability] <= 0 )
            return false;

        ability_charges[(int)ability]--;
        AbilityChargeChangedEvent.Invoke( ability, ability_charges[(int)ability] );

        Ability ab = null;
        switch( ability )
        {
            case AbilityEnum.TemporalAnomaly:
                {
                    AnomalyAbility _ab = new AnomalyAbility();
                    _ab.AbilityData = AnomalyData;
                    ab = _ab;
                    break;
                }
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

        return true;
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
    TemporalAnomaly = 0,
    ChainLightning = 1,
    Typhoon = 2,
    Sawmageddon = 3,
    NUM_ABILITIES = 4,
}
