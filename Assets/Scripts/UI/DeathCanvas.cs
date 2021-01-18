using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathCanvas : MonoBehaviour
{

    public Animator WaveCounter;
    public GameObject DeathScreen;


    public void DisplayDeathScreen()
    {
        DeathScreen.SetActive(true);
        WaveCounter.SetBool("Visible", true);
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
