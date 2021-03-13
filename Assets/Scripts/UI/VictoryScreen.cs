using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    public static VictoryScreen instance;

    public GameObject VictoryPopup;
    public GameObject PauseCanvas;
    public GameObject SawCanvas;
    public GameObject AbilityManager;
    public TextMeshProUGUI CandyGivenText;

    public void Start()
    {
        instance = this;
    }

    [ContextMenu("DisplayVictory")]
    public void DisplayVictory(int total_level_earnings)
    {
        VictoryPopup.SetActive(true);
        PauseCanvas.SetActive(false);
        SawCanvas.SetActive(false);
        AbilityManager.SetActive(false);

        if( CandyGivenText == null )
            Debug.LogError( "CandyGivenText not assigned in victory screen", this );
        if( CandyGivenText != null )
            CandyGivenText.text = total_level_earnings.ToString();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

}
