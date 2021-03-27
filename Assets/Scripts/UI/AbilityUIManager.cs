using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIManager : MonoBehaviour
{
    [SerializeField] float TimeScaleLerpDuration = 0.3f;
    private float TimeScaleReturnToNormalLerpDuratin = 0.1f; // this should always be extremely fast
    [SerializeField] List<GameObject> ShowOnAbilityMenuActive;
    [SerializeField] Animator GameplayFieldScrim;
    [SerializeField] List<GameObject> ChainLightningUsageSlots = new List<GameObject>();
    [SerializeField] List<GameObject> SawmageddonUsageSlots = new List<GameObject>();
    [SerializeField] List<GameObject> TemporalAnomalyUsageSlots = new List<GameObject>();
    [SerializeField] List<GameObject> TyphoonUsageSlots = new List<GameObject>();

    private bool showing = false;
    private AbilityUIButton currentHovering;

    private void Start()
    {
        HideIcons();
        AbilityManager.Instance.AbilityChargeChangedEvent.AddListener( OnAbilityChargeChanged );
        Debug.Assert( ChainLightningUsageSlots.Count == AbilityManager.Instance.MaxAbilityCharges );
        Debug.Assert( SawmageddonUsageSlots.Count == AbilityManager.Instance.MaxAbilityCharges );
        Debug.Assert( TemporalAnomalyUsageSlots.Count == AbilityManager.Instance.MaxAbilityCharges );
        Debug.Assert( TyphoonUsageSlots.Count == AbilityManager.Instance.MaxAbilityCharges );
    }

    private void OnAbilityChargeChanged(AbilityEnum ability, int new_charge)
    {
        List<GameObject> slot_indicators = null;
        switch( ability )
        {
            case AbilityEnum.TemporalAnomaly:
                slot_indicators = TemporalAnomalyUsageSlots;
                break;
            case AbilityEnum.ChainLightning:
                slot_indicators = ChainLightningUsageSlots;
                break;
            case AbilityEnum.Typhoon:
                slot_indicators = TyphoonUsageSlots;
                break;
            case AbilityEnum.Sawmageddon:
                slot_indicators = SawmageddonUsageSlots;
                break;
        }
        slot_indicators.ForEach( ( GameObject g ) => g.SetActive( false ) );
        for( int x = 0; x < new_charge; ++x )
            slot_indicators[x].SetActive( true );
    }

    private void Update()
    {
        if( showing )
        {
#if PC || UNITY_EDITOR
            if( !Input.GetMouseButton( 0 ) )
#endif
#if MOBILE && !UNITY_EDITOR
            if( Input.touchCount == 0 )
#endif
            {
                if( currentHovering )
                    AbilityManager.Instance.UseAbility( currentHovering.MyAbility );
                HideIcons();
            }
        }
    }

    public void ShowIcons()
    {
        foreach( var g in ShowOnAbilityMenuActive )
            g.SetActive( true );
        showing = true;
        GameplayManager.Instance.SetTimeScale( 0.1f, TimeScaleLerpDuration );
        GameplayFieldScrim.gameObject.SetActive( true );
        GameplayFieldScrim.SetBool( "Fade", true );
        currentHovering = null;
    }

    private void HideIcons()
    {
        foreach( var g in ShowOnAbilityMenuActive )
            g.SetActive( false );
        showing = false;
        GameplayManager.Instance.SetTimeScale( 1.0f, TimeScaleReturnToNormalLerpDuratin );
        GameplayFieldScrim.SetBool( "Fade", false );
        GameplayFieldScrim.gameObject.SetActive( false );
        currentHovering = null;
    }

    public void SetCurrentHovering( AbilityUIButton button )
    {
        currentHovering = button;
    }
}
