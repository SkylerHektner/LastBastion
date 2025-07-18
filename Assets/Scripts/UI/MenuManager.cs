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
    public List<Button> UpgradeButtonsFirstSelected;
    public List<CanvasGroup> MenusToSetInteractive;
    public GameObject InfoPanel;
    public InfoPanel UpgradePanel;
    public Button RightUpgradeScroll;
    public Button LeftUpgradeScroll;
    public Button HomeButton;
    public Boombox LimboTrack;
    public Boombox MenuTrack;

    public GameObject TradeCanvas;
    public GameObject OfferCanvas;
    public GameObject CustomizationCanvas;
    public Boombox SurvivalBoombox;
    public Boombox MenuUIBoombox;
    public AudioSource IntroWooshSFX;

    public GameObject BonusBandaid;
    public Image SkullMan;
    public Sprite SkullManAlt;
    public Sprite DefaulySkullSprite;
    public Button CampaignButton;

    public string RateURL;


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
            Invoke("SelectCampaignButton", 3f);
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

        if (PD.Instance.LevelCompletionMap.GetLevelCompletion("Level 9") == true) // if player has beaten campaign, show bandaid extra
        {
            BonusBandaid.SetActive(true);
            SkullMan.sprite = SkullManAlt;
        }
        else
        {
            BonusBandaid.SetActive(false);
            SkullMan.sprite = DefaulySkullSprite;
        }

    }

    public void SelectCampaignButton()
    {
        CampaignButton.Select();
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
        foreach (CanvasGroup group in MenusToSetInteractive)
        {
            group.interactable = false;
        }
    }
    public void HideUpgradeNavigator()
    {
        UpgradePanel.DenyPurchase();
        UpgradeNavigator.SetActive(false);
        InfoPanel.GetComponent<Animator>().SetTrigger("Skip");
        CameraMover.LoadLevels();
        RecentIndex = UpgradeIndex;
        CameraMover.CameraSpeed = .8f;
        foreach (CanvasGroup group in MenusToSetInteractive)
        {
            group.interactable = true;
        }

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
                LeftUpgradeScroll.Select();
                //reset the explicit navigation of the buttons
                Navigation ButtonNav_left = LeftUpgradeScroll.navigation;
                ButtonNav_left.selectOnUp = UpgradeButtonsFirstSelected[UpgradeIndex];
                ButtonNav_left.selectOnRight = null;
                ButtonNav_left.selectOnLeft = HomeButton;
                // then finally set the button to reflect the changes
                LeftUpgradeScroll.navigation = ButtonNav_left;

                Navigation HomeNav = HomeButton.navigation;
                HomeNav.selectOnDown = LeftUpgradeScroll;
                HomeNav.selectOnRight = LeftUpgradeScroll;
                HomeButton.navigation = HomeNav;
            }
            else
            {
                RightUpgradeScroll.interactable = true;
                Navigation ButtonNav_right = RightUpgradeScroll.navigation;
                ButtonNav_right.selectOnUp = UpgradeButtonsFirstSelected[UpgradeIndex];
                ButtonNav_right.selectOnRight = null;
                ButtonNav_right.selectOnLeft = LeftUpgradeScroll;

                Navigation ButtonNav_left = LeftUpgradeScroll.navigation;
                ButtonNav_left.selectOnUp = UpgradeButtonsFirstSelected[UpgradeIndex];
                ButtonNav_left.selectOnRight = RightUpgradeScroll;
                ButtonNav_left.selectOnLeft = HomeButton;

                LeftUpgradeScroll.navigation = ButtonNav_left;
                RightUpgradeScroll.navigation = ButtonNav_right;

                Navigation HomeNav = HomeButton.navigation;
                HomeNav.selectOnDown = LeftUpgradeScroll;
                HomeNav.selectOnRight = LeftUpgradeScroll ;
                HomeButton.navigation = HomeNav;
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
            if (UpgradeIndex == 0) // leftmost entry in upgrade view
            {
                LeftUpgradeScroll.interactable = false; // no left arrow to click on

                RightUpgradeScroll.Select(); // bring cursor highlight somewhere nice and redefine nav
                Navigation ButtonNav_right = RightUpgradeScroll.navigation;
                ButtonNav_right.selectOnUp = UpgradeButtonsFirstSelected[UpgradeIndex];
                ButtonNav_right.selectOnRight = null;
                ButtonNav_right.selectOnLeft = HomeButton;
                RightUpgradeScroll.navigation = ButtonNav_right;

                Navigation HomeNav = HomeButton.navigation;
                HomeNav.selectOnDown = RightUpgradeScroll;
                HomeNav.selectOnRight = RightUpgradeScroll;
                HomeButton.navigation = HomeNav;
            }
            else // any middle upgrade view
            {
                LeftUpgradeScroll.interactable = true;
                //reset the explicit navigation of the buttons
                Navigation ButtonNav_left = LeftUpgradeScroll.navigation;
                ButtonNav_left.selectOnUp = UpgradeButtonsFirstSelected[UpgradeIndex];
                ButtonNav_left.selectOnRight = RightUpgradeScroll;
                ButtonNav_left.selectOnLeft = HomeButton;

                Navigation ButtonNav_right = RightUpgradeScroll.navigation;
                ButtonNav_right.selectOnUp = UpgradeButtonsFirstSelected[UpgradeIndex];
                ButtonNav_right.selectOnRight = null;
                ButtonNav_right.selectOnLeft = LeftUpgradeScroll;
                // then finally set the button to reflect the changes
                LeftUpgradeScroll.navigation = ButtonNav_left;
                RightUpgradeScroll.navigation = ButtonNav_right;

                Navigation HomeNav = HomeButton.navigation;
                HomeNav.selectOnDown = LeftUpgradeScroll;
                HomeNav.selectOnRight = LeftUpgradeScroll;
                HomeButton.navigation = HomeNav;
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
        // dang, haven't used this since the prototype. Gonna remove this after I'm done crying about how far we've come
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

    // rate game button in settings
    public void OpenRateURL()
    {
        Application.OpenURL(RateURL);
    }

    // rate game button in settings
    public void OpenSteamURL()
    {
        string URL = "https://store.steampowered.com/app/3876840/Sawmania/";
        Application.OpenURL(URL);
    }

}
