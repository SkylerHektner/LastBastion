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


    private void Update()
    {
        if (Spectator.ReturningFromLevel)
        {
            PlayCanvas.SetActive(false);
            ShowLevels();
            //Door.SetTrigger("Open");
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
}
