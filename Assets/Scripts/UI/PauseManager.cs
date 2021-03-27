using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    public Animator WaveCounter;
    public GameObject PauseScreen;
    public GameObject BonusScreen;

    public void ResumeGame()
    {
        WaveCounter.SetBool("Visible", false);
        Time.timeScale = 1;
        PauseScreen.SetActive(false);
        BonusScreen.SetActive(true);
    }


    public void PauseGame()
    {
        PauseScreen.SetActive(true);
        Time.timeScale = 0;
        WaveCounter.SetBool("Visible", true);
        BonusScreen.SetActive(false);
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        PauseGame();
    //    }
    //}

    public void ExitGame()
    {
        Invoke("LoadMenu", 1f);
        Time.timeScale = 1;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void Start()
    {
        PauseManager.Instance = this;
    }

    private void OnDestroy()
    {
        PauseManager.Instance = null;
    }
}
