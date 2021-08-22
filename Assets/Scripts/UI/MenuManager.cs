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

    public GameObject UpgradeNavigator;
    public int UpgradeIndex;
    public int RecentIndex;
    public List<Transform> UpgradeLocations;
    public GameObject InfoPanel;
    public InfoPanel UpgradePanel;
    public Button RightUpgradeScroll;
    public Button LeftUpgradeScroll;


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
        }
        else if (Spectator.ReturningFromSurvival)
        {
            Debug.Log("Returning from survival");
            CameraMover.LoadSurvivalShortcut();
            MenuOptions.SetTrigger("Skip");
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

    // triggered by the animator on the door
    public void ExitLimbo()
    {

    }

    // used in the door animator 
    public void HideProgressCanvas()
    {
        ProgressContent.SetActive( false );
    }

    public void ShowProgressCanvas()
    {
        ProgressContent.SetActive( true );
    }

}
