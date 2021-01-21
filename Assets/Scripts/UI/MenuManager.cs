using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject PlayCanvas;
    public Animator Door;
    public GameObject LevelCanvas;
    public GameObject UpgradesCanvas;
    public Animator LevelBar;
    public GameObject ProgressContent;

    private void Update()
    {
        // triggers when the player comes to the main menu from quitting the game
        if (PlayerPrefs.GetInt("Limbo") == 1)
        {
            Door.SetBool("Limbo", true);
            PlayCanvas.SetActive(false);
        }
        else
        {
            Door.SetBool("Limbo", false);
        }

        // triggers when the player returns from the menu from a portal
        if (Spectator.ReturningFromLevel)
        {
            PlayCanvas.SetActive(false);
            ShowLevels();
        }

    }

    public void PlayGame()
    {
        PlayCanvas.SetActive(false);
        Door.SetTrigger("Open");
    }

    public void ShowLevels()
    {
        LevelCanvas.SetActive(true);
        LevelBar.SetTrigger("Appear");
    }

    public void ShowUpgrades()
    {
        UpgradesCanvas.SetActive(true);
    }

    // triggered by the animator on the door
    public void ExitLimbo()
    {
        Spectator.InLimbo = false;
        PlayCanvas.SetActive(true);
    }

    // used in the door animator 
    public void HideProgressCanvas()
    {
        ProgressContent.SetActive(false);
    }

    public void ShowProgressCanvas()
    {
        ProgressContent.SetActive(true);
    }

    public void ShowPlayButton()
    {
        PlayCanvas.SetActive(true);
    }

}
