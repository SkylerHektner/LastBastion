using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spectator : MonoBehaviour
{

    public static int LevelIndex;
    public static bool ReturningFromLevel;
    public int PlayerWealth;
    public static bool InLimbo;


    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Spectator");

        if (objs.Length > 1) // no duplicate spectators
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    // used for debugging (call this mid game, then stop the editor, then start at the main menu)
    [ContextMenu("ToggleLimbo")]
    public void ToggleLimbo()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            //InLimbo = !InLimbo;
            PlayerPrefs.SetInt("LevelIndex", LevelIndex);
            PlayerPrefs.SetInt("PlayerWealth", PlayerWealth);
            PlayerPrefs.SetInt("Limbo", 1);
            PlayerPrefs.SetString("ExitedScene", SceneManager.GetActiveScene().name);
            Debug.Log(InLimbo);
        }

    }

    ////////// TOGGLE BACK ON LATER //////////////
    // game is quit mid-game, save my current progress and put me in limbo
    // this plays when you stop playing the game in-editor

    //private void OnApplicationQuit()
    //{
    //    Debug.Log("donkeys");
    //    if (SceneManager.GetActiveScene().name != "Menu") // only save current progress if you are in a current level
    //    {
    //        PlayerPrefs.SetInt("LevelIndex", LevelIndex);
    //        PlayerPrefs.SetInt("PlayerWealth", PlayerWealth);
    //        PlayerPrefs.SetInt("Limbo", 1);
    //        PlayerPrefs.SetString("ExitedScene", SceneManager.GetActiveScene().name);
    //        // set current wave here too
    //        // save base stats too
    //        // save powerup charges
    //          // make sure player is not able to call this while dead
    //    }

    //}

}
