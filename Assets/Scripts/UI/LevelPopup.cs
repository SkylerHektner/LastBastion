using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LevelPopup : MonoBehaviour
{
    public static SpawnCadenceProfile ActivePopupSpawnCadence;
    public int Index;
    public Animator Portal;
    public TextMeshProUGUI GameModeText;
    public TextMeshProUGUI CompletionPayout;
    public TextMeshProUGUI ChallengeText;
    public TextMeshProUGUI ChallengeOptionalTextDecorator;
    public TextMeshProUGUI Description;

    public GameObject ScratchoutTitle;
    public GameObject ScratchoutChallenge;

    public GameObject ArrowR;
    public GameObject ArrowL;
    public Button HomeButton;
    public Button UpgradesButton;

    private void Update()
    {
        if ( ActivePopupSpawnCadence != null)
        {
            ScratchoutTitle.SetActive( PD.Instance.LevelCompletionMap.GetLevelCompletion( ActivePopupSpawnCadence.LevelIdentifier ) );
            ScratchoutChallenge.SetActive( ActivePopupSpawnCadence.LevelChallenge != null
                && PD.Instance.PlayerChallengeCompletionList.Contains( ActivePopupSpawnCadence.LevelChallenge.UniqueChallengeID ) );
        }
    }

    public void DisableArrows()
    {
        ArrowR.GetComponent<Button>().enabled = false;
        ArrowR.GetComponent<Image>().enabled = false;
        ArrowL.GetComponent<Button>().enabled = false;
        ArrowL.GetComponent<Image>().enabled = false;
    }

    public void AcceptContract() // called by animator
    {
        Portal.SetTrigger("Grow");
        Invoke("SceneChange", 1f);
        Spectator.LevelIndex = LevelScroller.LevelIndex; // reference index to the button
        Spectator.ReturningFromLevel = true;
        gameObject.GetComponent<Animator>().SetBool("Open", false);
    }

    public void ShrinkContract()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Accepted");
        HomeButton.enabled = false;
        UpgradesButton.enabled = false;
        ArrowL.SetActive(false);
        ArrowR.SetActive(false);
    }

    public void SceneChange()
    {
        SceneManager.LoadScene( ActivePopupSpawnCadence.LevelIdentifier );
    }

    public void DeclineContract()
    {
        gameObject.GetComponent<Animator>().SetBool("Open", false);
        ArrowR.GetComponent<Button>().enabled = true;
        ArrowR.GetComponent<Image>().enabled = true;
        ArrowL.GetComponent<Button>().enabled = true;
        ArrowL.GetComponent<Image>().enabled = true;
    }

    public void DisableButtons()
    {
        HomeButton.enabled = false;
        UpgradesButton.enabled = false;
    }
    public void EnableButtons()
    {
        HomeButton.enabled = true;
        UpgradesButton.enabled = true;
    }
}
