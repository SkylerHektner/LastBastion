using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DeathCanvas : MonoBehaviour
{

    public Animator WaveCounter;
    public GameObject DeathScreen;
    //public GameObject RewardScreen;
    public GameObject SendSawCanvas;
    public TextMeshProUGUI WaveText;


    public void DisplayDeathScreen()
    {
        DeathScreen.SetActive(true);
        WaveText.text = ("Wave  " + WaveCounter.GetComponent<WaveCounter>().CurrentWave);
        //WaveCounter.SetBool("Visible", true);
        //RewardScreen.SetActive(false);
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
