using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressCanvas : MonoBehaviour
{
    public Animator Door;
    public Animator Skull;
    public GameObject ProgressContent;

    private void Start()
    {
        Skull.SetBool("Speaking", true);
    }

    public void ContinueSave()
    {

    }
    public void LoadScene()
    {
        Spectator.LevelIndex = PlayerPrefs.GetInt("LevelIndex");
        Spectator.ReturningFromLevel = true;
        Spectator.InLimbo = !Spectator.InLimbo;
        PlayerPrefs.SetInt("Limbo", 0);
        SceneManager.LoadScene(PlayerPrefs.GetString("ExitedScene"));
    }

    public void DeclineSave()
    {
        Skull.SetBool("Speaking", false);
        // play animation (call load menu at the end)
    }

    public void LoadMenu()
    {
        ProgressContent.SetActive(false);
        Debug.Log("Loading Menu");
        Spectator.InLimbo = !Spectator.InLimbo;
        PlayerPrefs.SetInt("LevelIndex", 1);
        PlayerPrefs.SetInt("Limbo", 0);
        Door.SetBool("Limbo", false);
    }
}
