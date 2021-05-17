using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// DEPRECATED
/// </summary>
public class AbilityUIManager : MonoBehaviour
{
    [SerializeField] float TimeScaleLerpDuration = 0.3f;
    private float TimeScaleReturnToNormalLerpDuratin = 0.1f; // this should always be extremely fast
    [SerializeField] List<GameObject> ShowOnAbilityMenuActive;
    [SerializeField] Animator GameplayFieldScrim;
    [SerializeField] List<GameObject> ChainLightningUsageSlots = new List<GameObject>();
    [SerializeField] List<GameObject> SawmageddonUsageSlots = new List<GameObject>();
    [SerializeField] List<GameObject> AnomalyUsageSlots = new List<GameObject>();
    [SerializeField] List<GameObject> TyphoonUsageSlots = new List<GameObject>();

    private bool showing = false;
    public AbilityUIButton CurrentHovering { get; private set; }

    private void Start()
    {
        HideIcons();
        //AbilityManager.Instance.AbilityChargeChangedEvent.AddListener( OnAbilityChargeChanged );
        //Debug.Assert( ChainLightningUsageSlots.Count == AbilityManager.Instance.MaxAbilityCharges );
        //Debug.Assert( SawmageddonUsageSlots.Count == AbilityManager.Instance.MaxAbilityCharges );
        //Debug.Assert( AnomalyUsageSlots.Count == AbilityManager.Instance.MaxAbilityCharges );
        //Debug.Assert( TyphoonUsageSlots.Count == AbilityManager.Instance.MaxAbilityCharges );

        UpdateAllSlotIndicators();
    }

    private void OnAbilityChargeChanged(AbilityEnum ability, int new_charge)
    {
        List<GameObject> slot_indicators = null;
        switch( ability )
        {
            case AbilityEnum.Anomaly:
                slot_indicators = AnomalyUsageSlots;
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

    private void UpdateAllSlotIndicators()
    {
        //AnomalyUsageSlots.ForEach( ( GameObject g ) => g.SetActive( false ) );
        //for( int x = 0; x < AbilityManager.Instance.GetAbilityCharges(AbilityEnum.Anomaly); ++x )
        //    AnomalyUsageSlots[x].SetActive( true );

        //TyphoonUsageSlots.ForEach( ( GameObject g ) => g.SetActive( false ) );
        //for( int x = 0; x < AbilityManager.Instance.GetAbilityCharges( AbilityEnum.Typhoon ); ++x )
        //    TyphoonUsageSlots[x].SetActive( true );

        //ChainLightningUsageSlots.ForEach( ( GameObject g ) => g.SetActive( false ) );
        //for( int x = 0; x < AbilityManager.Instance.GetAbilityCharges( AbilityEnum.ChainLightning ); ++x )
        //    ChainLightningUsageSlots[x].SetActive( true );

        //SawmageddonUsageSlots.ForEach( ( GameObject g ) => g.SetActive( false ) );
        //for( int x = 0; x < AbilityManager.Instance.GetAbilityCharges( AbilityEnum.Sawmageddon ); ++x )
        //    SawmageddonUsageSlots[x].SetActive( true );
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
                if( CurrentHovering )
                    AbilityManager.Instance.UseAbility( CurrentHovering.MyAbility );
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
        CurrentHovering = null;
    }

    private void HideIcons()
    {
        foreach( var g in ShowOnAbilityMenuActive )
            g.SetActive( false );
        showing = false;
        GameplayManager.Instance.SetTimeScale( 1.0f, TimeScaleReturnToNormalLerpDuratin );
        GameplayFieldScrim.SetBool( "Fade", false );
        GameplayFieldScrim.gameObject.SetActive( false );
        CurrentHovering = null;
    }

    public void SetCurrentHovering( AbilityUIButton button )
    {
        CurrentHovering = button;
    }
}
