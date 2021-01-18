using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    public GameObject VictoryPopup;
    public GameObject PauseCanvas;
    public GameObject SawCanvas;
    public GameObject AbilityManager;


    [ContextMenu("DisplayVictory")]
    public void DisplayVictory()
    {
        VictoryPopup.SetActive(true);
        PauseCanvas.SetActive(false);
        SawCanvas.SetActive(false);
        AbilityManager.SetActive(false);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

}
