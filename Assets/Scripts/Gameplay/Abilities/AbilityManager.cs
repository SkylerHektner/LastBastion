using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }
    public UnityEvent<AbilityEnum> AbilityUsedEvent = new UnityEvent<AbilityEnum>();

    public Vector3 BaseCenter;
    [SerializeField] AnomalyAbilityData AnomalyData;
    [SerializeField] ChainLightningAbilityData ChainLightningData;
    [SerializeField] TyphoonAbilityData TyphoonData;
    [SerializeField] SawmageddonAbilityData SawmageddonData;

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

    public bool UseAbility( AbilityEnum ability )
    {
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
