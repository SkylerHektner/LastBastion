using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DEPRECATED
/// </summary>
public class AbilityUIButton : MonoBehaviour
{
    public AbilityEnum MyAbility;
    public UnlockFlags UnlockFlag;
    public AbilityUIManager AbilityUIManagerInstance;
    public Animator IconAnimator;
    public Animator FXAnimator;
    public GameObject InfoScroll;
    public Animator UsageSlotsAnimator;
    public DisplayInfo MyDisplayInfo;
    public GameObject LockedImage;

    private bool hovering = false;

    private void Start()
    {
        PD.Instance.UpgradeFlagChangedEvent.AddListener( OnUpgradeUnlockFlagChanged );
    }

    private void OnDestroy()
    {
        PD.Instance.UpgradeFlagChangedEvent.RemoveListener( OnUpgradeUnlockFlagChanged );
    }

    private void OnEnable()
    {
        UpdateLockedState();
    }

    private void OnUpgradeUnlockFlagChanged( UnlockFlags flag, bool value )
    {
        if( flag == UnlockFlag )
        {
            UpdateLockedState();
        }
    }

    private void OnAbilityChargeChanged( AbilityEnum ability, int new_value )
    {
        if( ability == MyAbility )
        {
            // DEPRECTATED BEHAVIOR
            // bool has_no_charges = AbilityManager.Instance.GetAbilityCharges( MyAbility ) <= 0;
            // IconAnimator.SetBool( "Empty", has_no_charges );

            if( PD.Instance.UnlockMap.Get( UnlockFlag ) && hovering )
            {
                FXAnimator.SetTrigger( "Glow" );
                FXAnimator.ResetTrigger( "Hide" );
            }
        }
    }

    private void UpdateLockedState()
    {
        if( PD.Instance.UnlockMap.Get( UnlockFlag ) )
        {
            LockedImage.SetActive( false );
            UsageSlotsAnimator.gameObject.SetActive( true );
        }
        else
        {
            LockedImage.SetActive( true );
            UsageSlotsAnimator.gameObject.SetActive( false );
        }
    }

    public void OnPointerEnter()
    {
        hovering = true;

        if( PD.Instance.UnlockMap.Get( UnlockFlag ) )
        {
            AbilityUIManagerInstance.SetCurrentHovering( this );

            //bool has_no_charges = AbilityManager.Instance.GetAbilityCharges( MyAbility ) <= 0;

            //IconAnimator.SetBool( "Empty", has_no_charges );
            IconAnimator.SetTrigger( "Hover" );
            IconAnimator.ResetTrigger( "UnHover" );
            //if( !has_no_charges )
            //{
            //    FXAnimator.SetTrigger( "Glow" );
            //    FXAnimator.ResetTrigger( "Hide" );
            //}
            InfoScroll.SetActive( true );
            MyDisplayInfo.DisplayMyInfo();
            UsageSlotsAnimator.SetTrigger( "Grow" );
            UsageSlotsAnimator.ResetTrigger( "Shrink" );
        }
    }

    public void OnPointerExit()
    {
        hovering = false;
        Invoke( "ClearCurrentHovering", 0.1f );

        //IconAnimator.SetBool( "Empty", AbilityManager.Instance.GetAbilityCharges( MyAbility ) <= 0 );
        IconAnimator.SetTrigger( "UnHover" );
        IconAnimator.ResetTrigger( "Hover" );
        FXAnimator.SetTrigger( "Hide" );
        FXAnimator.ResetTrigger( "Glow" );
        InfoScroll.SetActive( false );
        MyDisplayInfo.HideInfo();
        UsageSlotsAnimator.SetTrigger( "Shrink" );
        UsageSlotsAnimator.ResetTrigger( "Grow" );
    }

    private void ClearCurrentHovering()
    {
        if( AbilityUIManagerInstance.CurrentHovering == this )
            AbilityUIManagerInstance.SetCurrentHovering( null );
    }
}
