using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Security.Cryptography;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    public Animator WaveCounter;
    public GameObject PauseScreen;
    //public GameObject BonusScreen;
    public GameObject PauseButton;
    public TextMeshProUGUI CurrentWaveText;
    public Animator ConfirmationMenu;
    public Button ResumeButton;

    private bool paused = false;

    public void ResumeGame()
    {
        //WaveCounter.SetBool("Visible", false);
        Time.timeScale = 1;
        PauseScreen.SetActive(false);
        //BonusScreen.SetActive(true);
        PauseButton.SetActive(true);
        WaveCounter.ResetTrigger("Hide");
        paused = false;
    }
    public void DelayedResume()
    {
        Time.timeScale = 1;
        Invoke("ResumeGame", .5f);
        DenyConfirmation();
    }


    public void PauseGame()
    {
        ResumeButton.Select();
        PauseScreen.SetActive(true);
        Time.timeScale = 0;
        WaveCounter.SetTrigger("Hide");
        CurrentWaveText.text = ("Wave  " + WaveCounter.GetComponent<WaveCounter>().CurrentWave);
        //BonusScreen.SetActive(false);
        PauseButton.SetActive(false);
        paused = true;
    }

    public void ExitGame()
    {
        Invoke("LoadMenu", 1f);
        Time.timeScale = 1;
        GameplayManager.Instance.ResetLimbo();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void Start()
    {
        PauseManager.Instance = this;
        Steamworks.SteamFriends.OnGameOverlayActivated += SteamOverlayChangedStatus;
    }

    private void Update()
    {
        if (Spectator.Instance.InGamepadMode)
        {
            Gamepad gp = Gamepad.current;
            if (gp.startButton.wasPressedThisFrame)
            {
                if (!paused)
                    PauseGame();
                else
                    ResumeGame();
            }
        }
    }

    private void OnDestroy()
    {
        PauseManager.Instance = null;
        Steamworks.SteamFriends.OnGameOverlayActivated -= SteamOverlayChangedStatus;
    }

    private void SteamOverlayChangedStatus(bool InOverlay)
    {
        if (!paused && InOverlay)
        {
            PauseGame();
        }

        if (paused && !InOverlay)
        {
            ResumeGame();
        }
    }

    public void AskConfirmation()
    {
        ConfirmationMenu.SetBool("Confirming", true);

    }
    public void DenyConfirmation()
    {
        ConfirmationMenu.SetBool("Confirming", false);
    }
}
