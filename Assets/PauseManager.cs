using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public Animator WaveCounter;
    public GameObject PauseScreen;

    public void ResumeGame()
    {
        WaveCounter.SetBool("Visible", false);
        Time.timeScale = 1;
        PauseScreen.SetActive(false);
    }


    public void PauseGame()
    {
        PauseScreen.SetActive(true);
        Time.timeScale = 0;
        WaveCounter.SetBool("Visible", true);

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void ExitGame()
    {
        Invoke("LoadMenu", 1f);
        Time.timeScale = 1;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

}
