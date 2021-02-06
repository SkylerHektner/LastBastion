using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUIButton : MonoBehaviour
{
    public AbilityEnum MyAbility;
    public PlayerData.UpgradeFlags UnlockFlag;
    public AbilityUIManager AbilityUIManagerInstance;
    public Animator IconAnimator;
    public Animator FXAnimator;
    public GameObject InfoScroll;
    public Animator UsageSlotsAnimator;
    public DisplayInfo MyDisplayInfo;
    public GameObject LockedImage;

    private void Start()
    {
        PlayerData.Instance.UpgradeFlagChangedEvent.AddListener( OnUpgradeUnlockFlagChanged );
    }

    private void OnDestroy()
    {
        PlayerData.Instance.UpgradeFlagChangedEvent.RemoveListener( OnUpgradeUnlockFlagChanged );
    }

    private void OnEnable()
    {
        UpdateLockedState();
    }

    private void OnUpgradeUnlockFlagChanged(PlayerData.UpgradeFlags flag, bool value)
    {
        if( flag == UnlockFlag )
        {
            UpdateLockedState();
        }
    }

    private void UpdateLockedState()
    {
        LockedImage.SetActive( !PlayerData.Instance.UpgradeUnlockMap.GetUnlock( UnlockFlag ) );
    }

    public void OnPointerEnter()
    {
        if( AbilityManager.Instance.GetAbilityCharges( MyAbility ) > 0
            && PlayerData.Instance.UpgradeUnlockMap.GetUnlock( UnlockFlag ) )
        {
            AbilityUIManagerInstance.SetAbilityCandidate( MyAbility );
            IconAnimator.SetTrigger( "Hover" );
            IconAnimator.ResetTrigger( "UnHover" );
            FXAnimator.SetTrigger( "Glow" );
            FXAnimator.ResetTrigger( "Hide" );
            InfoScroll.SetActive( true );
            MyDisplayInfo.DisplayMyInfo();
            UsageSlotsAnimator.SetTrigger( "Grow" );
            UsageSlotsAnimator.ResetTrigger( "Shrink" );
        }
    }

    public void OnPointerExit()
    {
        AbilityUIManagerInstance.SetAbilityCandidate( null );
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
