﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUIButton : MonoBehaviour
{
    public AbilityEnum MyAbility;
    public PD.UpgradeFlags UnlockFlag;
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
        AbilityManager.Instance.AbilityChargeChangedEvent.AddListener( OnAbilityChargeChanged );
    }

    private void OnDestroy()
    {
        PD.Instance.UpgradeFlagChangedEvent.RemoveListener( OnUpgradeUnlockFlagChanged );
        AbilityManager.Instance.AbilityChargeChangedEvent.RemoveListener( OnAbilityChargeChanged );
    }

    private void OnEnable()
    {
        UpdateLockedState();
    }

    private void OnUpgradeUnlockFlagChanged( PD.UpgradeFlags flag, bool value )
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
            bool has_no_charges = AbilityManager.Instance.GetAbilityCharges( MyAbility ) <= 0;
            IconAnimator.SetBool( "Empty", has_no_charges );

            if( PD.Instance.UpgradeUnlockMap.GetUnlock( UnlockFlag ) && hovering )
            {
                FXAnimator.SetTrigger( "Glow" );
                FXAnimator.ResetTrigger( "Hide" );
            }
        }
    }

    private void UpdateLockedState()
    {
        if( PD.Instance.UpgradeUnlockMap.GetUnlock( UnlockFlag ) )
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

        if( PD.Instance.UpgradeUnlockMap.GetUnlock( UnlockFlag ) )
        {
            bool has_no_charges = AbilityManager.Instance.GetAbilityCharges( MyAbility ) <= 0;

            AbilityUIManagerInstance.SetAbilityCandidate( MyAbility );
            IconAnimator.SetBool( "Empty", has_no_charges );
            IconAnimator.SetTrigger( "Hover" );
            IconAnimator.ResetTrigger( "UnHover" );
            if( !has_no_charges )
            {
                FXAnimator.SetTrigger( "Glow" );
                FXAnimator.ResetTrigger( "Hide" );
            }
            InfoScroll.SetActive( true );
            MyDisplayInfo.DisplayMyInfo();
            UsageSlotsAnimator.SetTrigger( "Grow" );
            UsageSlotsAnimator.ResetTrigger( "Shrink" );
        }
    }

    public void OnPointerExit()
    {
        hovering = false;

        AbilityUIManagerInstance.SetAbilityCandidate( null );
        IconAnimator.SetBool( "Empty", AbilityManager.Instance.GetAbilityCharges( MyAbility ) <= 0 );
        IconAnimator.SetTrigger( "UnHover" );
        IconAnimator.ResetTrigger( "Hover" );
        FXAnimator.SetTrigger( "Hide" );
        FXAnimator.ResetTrigger( "Glow" );
        InfoScroll.SetActive( false );
        MyDisplayInfo.HideInfo();
        UsageSlotsAnimator.SetTrigger( "Shrink" );
        UsageSlotsAnimator.ResetTrigger( "Grow" );
    }
}