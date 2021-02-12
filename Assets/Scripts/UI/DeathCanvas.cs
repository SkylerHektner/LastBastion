using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathCanvas : MonoBehaviour
{

    public Animator WaveCounter;
    public GameObject DeathScreen;
    public GameObject RewardScreen;
    public GameObject SendSawCanvas;

    public void DisplayDeathScreen()
    {
        DeathScreen.SetActive(true);
        WaveCounter.SetBool("Visible", true);
        RewardScreen.SetActive(false);
        SendSawCanvas.SetActive(false);
    }

    public void ExitGame()
    {
        //Invoke("LoadMenu", 1f);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
