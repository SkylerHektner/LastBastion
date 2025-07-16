using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    public static VictoryScreen instance;

    public GameObject VictoryPopup;
    public GameObject PauseCanvas;
    public GameObject SawCanvas;
    public GameObject AbilityManager;
    public TextMeshProUGUI CandyGivenText;
    public GameObject ChallengeDisplay;
    public TextMeshProUGUI ChallengeSuccessNotifierText;
    public TextMeshProUGUI ChallengeDescriptionText;
    public Button ContinueButton;

    public void Start()
    {
        instance = this;
    }

    [ContextMenu( "DisplayVictory" )]
    public void DisplayVictory( int total_level_earnings, bool has_challenge, bool challenge_succeeded, string challenge_description_text )
    {
        VictoryPopup.SetActive( true );
        PauseCanvas.SetActive( false );
        SawCanvas.SetActive( false );
        AbilityManager.SetActive( false );
        Invoke("SelectContinueButton", 3f);

#if UNITY_EDITOR
        if ( CandyGivenText == null )
            Debug.LogError( "CandyGivenText not assigned in victory screen", this );
#endif
        if( CandyGivenText != null )
            CandyGivenText.text = total_level_earnings.ToString();

        if( has_challenge )
        {
            ChallengeDisplay.SetActive( true );
            ChallengeSuccessNotifierText.text = challenge_succeeded ? "Challenge Complete!" : "Challenge Failed";
            ChallengeDescriptionText.text = challenge_description_text;
            if (ChallengeSuccessNotifierText.text == "Challenge Complete!")
            {
                ChallengeDisplay.GetComponent<Animator>().SetBool("Winner", true);
            }
        }
    }

    public void SelectContinueButton()
    {
        ContinueButton.Select();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene( "Menu" );
    }

    public void LoadCandyVault() // called after beating the final boss
    {
        Invoke("LoadCredits", 6f);
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
