using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject LevelCanvas;
    public GameObject UpgradesCanvas;
    public Animator LevelBar;
    public GameObject ProgressContent;
    public CameraUIMover CameraMover;
    public Animator CampaignPortal;
    public Animator MenuOptions;
    public Animator TheShop;

    public GameObject UpgradeNavigator;
    public int UpgradeIndex;
    public int RecentIndex;
    public List<Transform> UpgradeLocations;
    public GameObject InfoPanel;
    public InfoPanel UpgradePanel;
    public Button RightUpgradeScroll;
    public Button LeftUpgradeScroll;
    public Boombox LimboTrack;
    public Boombox MenuTrack;

    public GameObject TradeCanvas;
    public GameObject OfferCanvas;
    public GameObject CustomizationCanvas;
    public Boombox SurvivalBoombox;
    public Boombox MenuUIBoombox;
    public AudioSource IntroWooshSFX;


    private void Awake()
    {
        RecentIndex = 2;
        // triggers when the player returns from the menu from a portal
        if ( Spectator.ReturningFromLevel )
        {
            Debug.Log("Returning from level");
            //ReturnFromCampaignLevel();
            CameraMover.LoadLevelShortcut();
            CampaignPortal.SetTrigger("Shrink");
            MenuOptions.SetTrigger("Skip");
            MenuUIBoombox.SwapTrack(MenuUIBoombox.MyClip);
            //IntroWooshSFX.playOnAwake = false;
        }
        else if (Spectator.ReturningFromSurvival)
        {
            Debug.Log("Returning from survival");
            CameraMover.LoadSurvivalShortcut();
            MenuOptions.SetTrigger("Skip");
            SurvivalBoombox.SwapTrack(SurvivalBoombox.MyClip);
            //IntroWooshSFX.playOnAwake = false;
        }
        else if ((!Spectator.ReturningFromLevel && !Spectator.ReturningFromSurvival) && (!PD.Instance.CampaignLimboResumeInformation.Active && !PD.Instance.SurvivalLimboResumeInformation.Active))
        {
            // playing when starting game up
            Debug.Log("Starting up menu");
            IntroWooshSFX.playOnAwake = true;
            IntroWooshSFX.Play();
        }

        if (PD.Instance.CampaignLimboResumeInformation != null && PD.Instance.SurvivalLimboResumeInformation != null)
        {
            if ((PD.Instance.CampaignLimboResumeInformation.Active) || PD.Instance.SurvivalLimboResumeInformation.Active) // limbo
            {
                MenuTrack.gameObject.SetActive(false);
                MenuOptions.SetTrigger("Skip");
                ShowProgressCanvas();
            }
            else if (!PD.Instance.CampaignLimboResumeInformation.Active && !PD.Instance.SurvivalLimboResumeInformation.Active) // no limbo
            {
                MenuTrack.gameObject.SetActive(true);
                MenuTrack.SwapTrack(MenuTrack.MyClip);
            }
        }

    }

    public void ShowLevels()
    {
        CameraMover.LoadLevelShortcut();

        LevelCanvas.SetActive( true );
        LevelBar.SetTrigger( "Appear" );
    }

    public void ShowUpgradeNavigator()
    {
        UpgradeNavigator.SetActive(true);
        UpgradeIndex = RecentIndex;
        CameraMover.ShowUpgradeTree(UpgradeLocations[UpgradeIndex]); // moves the camera to the index of the currently viewed skill tree
        InfoPanel.transform.position = UpgradeLocations[UpgradeIndex].position;
        CameraMover.CameraSpeed = 1.2f;
    }
    public void HideUpgradeNavigator()
    {
        UpgradePanel.DenyPurchase();
        UpgradeNavigator.SetActive(false);
        InfoPanel.GetComponent<Animator>().SetTrigger("Skip");
        CameraMover.LoadLevels();
        RecentIndex = UpgradeIndex;
        CameraMover.CameraSpeed = .8f;

    }

    public void CycleUpgradesRight()
    {
        if (UpgradeIndex < UpgradeLocations.Count - 1) // [0,5]
        {
            UpgradeIndex += 1;
            CameraMover.ShowUpgradeTree(UpgradeLocations[UpgradeIndex]); // moves the camera to the index of the currently viewed skill tree
            UpgradePanel.DenyPurchase();
            InfoPanel.GetComponent<Animator>().SetTrigger("Skip");
            InfoPanel.transform.position = UpgradeLocations[UpgradeIndex].position;
            LeftUpgradeScroll.interactable = true;
            if (UpgradeIndex == UpgradeLocations.Count - 1)
            {
                RightUpgradeScroll.interactable = false;
            }
            else
            {
                RightUpgradeScroll.interactable = true;

            }

        }
    }
    public void CycleUpgradesLeft()
    {
        if (UpgradeIndex > 0) // [0,5]
        {
            UpgradeIndex -= 1;
            UpgradePanel.DenyPurchase();
            InfoPanel.GetComponent<Animator>().SetTrigger("Skip");
            CameraMover.ShowUpgradeTree(UpgradeLocations[UpgradeIndex]); // moves the camera to the index of the currently viewed skill tree
            InfoPanel.transform.position = UpgradeLocations[UpgradeIndex].position;
            RightUpgradeScroll.interactable = true;
            if (UpgradeIndex == 0)
            {
                LeftUpgradeScroll.interactable = false;
            }
            else
            {
                LeftUpgradeScroll.interactable = true;
            }

        }
    }

    public void LoadStore()
    {
        MenuOptions.SetTrigger("Shopping");
        TheShop.SetTrigger("OpenShop");
        TradeCanvas.SetActive(true);
        OfferCanvas.SetActive(true);
        CustomizationCanvas.SetActive(true);
    }
    public void LeaveStore()
    {
        TheShop.SetTrigger("CloseShop");
        MenuOptions.SetTrigger("DoneShopping");
        TradeCanvas.SetActive(false);
        OfferCanvas.SetActive(false);
        CustomizationCanvas.SetActive(false);
    }

    // triggered by the animator on the door
    public void ExitLimbo()
    {

    }

    // used in the door animator 
    public void HideProgressCanvas()
    {
        ProgressContent.SetActive( false );
        MenuTrack.gameObject.SetActive(true);
    }

    public void ShowProgressCanvas()
    {
        ProgressContent.SetActive( true );
        LimboTrack.SwapTrack(LimboTrack.MyClip); // play limbo track
    }

}
