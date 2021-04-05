using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }
    public UnityEvent<AbilityEnum, int> AbilityChargeChangedEvent = new UnityEvent<AbilityEnum, int>();
    public UnityEvent<AbilityEnum> AbilityUsedEvent = new UnityEvent<AbilityEnum>();

    [SerializeField] GameObject AbilityInfoScroll1;
    [SerializeField] GameObject AbilityInfoScroll2;
    [SerializeField] GameObject AbilityInfoScroll3;
    [SerializeField] GameObject AbilityInfoScroll4;

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

        if( PD.Instance.Limbo.Get() )
        {
            foreach( var kvp in PD.Instance.StoredLimboAbilityCharges )
            {
                AddAbilityCharge( kvp.Key, kvp.Value );
            }
        }
    }

    private void Update()
    {
        foreach( var ab in active_abilities.Values )
            ab.Update( Time.deltaTime );
        foreach( var r in pending_removals )
            active_abilities.Remove( r );
        pending_removals.Clear();
    }

    public void AddAbilityCharge( AbilityEnum ability, int num = 1 )
    {
        Debug.Assert( ability != AbilityEnum.NUM_ABILITIES );
        if( ability_charges[(int)ability] < MaxAbilityCharges )
        {
            ability_charges[(int)ability] = Mathf.Min( ability_charges[(int)ability] + num, MaxAbilityCharges );
            AbilityChargeChangedEvent.Invoke( ability, ability_charges[(int)ability] );
        }
    }

    public int GetAbilityCharges( AbilityEnum ability )
    {
        Debug.Assert( ability != AbilityEnum.NUM_ABILITIES );
        return ability_charges[(int)ability];
    }

    public bool UseAbility( AbilityEnum ability )
    {
        // TODO: Move this to AbilityUIManager
        AbilityInfoScroll1.SetActive( false ); // hi, I added this.  It just turns off the scroll.
        AbilityInfoScroll2.SetActive( false );
        AbilityInfoScroll3.SetActive( false );
        AbilityInfoScroll4.SetActive( false );

        if( ability_charges[(int)ability] <= 0 )
            return false;

        ability_charges[(int)ability]--;
        AbilityChargeChangedEvent.Invoke( ability, ability_charges[(int)ability] );
        AbilityUsedEvent.Invoke( ability );

        // check if this ability is already active and consult the ability instance to see if we proceed with construction
        bool cancel_construction = false;
        foreach( Ability active_ab in active_abilities.Values )
        {
            if( active_ab.ability == ability )
            {
                cancel_construction |= active_ab.OnAbilityUsedWhileAlreadyActive();
            }
        }

        if( cancel_construction )
            return false; // EARLY RETURN

        Ability ab = null;
        switch( ability )
        {
            case AbilityEnum.Anomaly:
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
        if( ab != null )
        {
            ab.ability = ability;
            ab.AM = this;
            ab.name = ability.ToString();
            active_abilities.Add( ab.AbilityID, ab );
            ab.Start();
        }

        return true;
    }

    public void AbilityFinished( long AbilityID )
    {
        Debug.Assert( active_abilities.ContainsKey( AbilityID ) );
        pending_removals.Add( AbilityID );
    }

#if UNITY_EDITOR
    [ContextMenu( "Print All Active Abilities" )]
    private void DebugLogAllActiveAbilities()
    {
        foreach( var ab in active_abilities )
            Debug.Log( ab.Value.name );
    }
#endif

    private void OnDestroy()
    {
        foreach( var ab in active_abilities )
            ab.Value.OnSceneExit();
    }
}

public enum AbilityEnum
{
    Anomaly = 0,
    ChainLightning = 1,
    Typhoon = 2,
    Sawmageddon = 3,
    NUM_ABILITIES = 4,
}
