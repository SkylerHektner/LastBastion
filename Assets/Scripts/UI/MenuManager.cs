using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject LevelCanvas;
    public GameObject UpgradesCanvas;
    public Animator LevelBar;
    public GameObject ProgressContent;
    public CameraUIMover CameraMover;
    public Animator CampaignPortal;
    public Animator MenuOptions;

    private void Awake()
    {

        // triggers when the player returns from the menu from a portal
        if( Spectator.ReturningFromLevel )
        {
            Debug.Log("Returning from level");
            //ReturnFromCampaignLevel();
            CameraMover.LoadLevelShortcut();
            CampaignPortal.SetTrigger("Shrink");
            MenuOptions.SetTrigger("Skip");
        }

    }

    public void ShowLevels()
    {
        CameraMover.LoadLevelShortcut();

        LevelCanvas.SetActive( true );
        LevelBar.SetTrigger( "Appear" );

    }

    public void ShowUpgrades()
    {
        UpgradesCanvas.SetActive( true );
    }

    // triggered by the animator on the door
    public void ExitLimbo()
    {
        PD.Instance.Limbo.Set( false );
        ////PlayCanvas.SetActive( true );
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

    public void ShowPlayButton()
    {
        ////PlayCanvas.SetActive( true );
    }

}
