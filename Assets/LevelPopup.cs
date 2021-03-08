using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelPopup : MonoBehaviour
{
    public static string SceneName;
    public int Index;
    public Animator UpgradesBar;
    public Animator Portal;
    public TextMeshProUGUI GameModeText;
    public TextMeshProUGUI CompletionPayout;
    public TextMeshProUGUI ChallengeText;
    public TextMeshProUGUI Description;

    public GameObject ScratchoutTitle;
    public GameObject ScratchoutChallenge;

    private void Update()
    {
        if (SceneName != null)
        {
            if (PD.Instance.LevelCompletionMap.GetLevelCompletion(SceneName)) // scratch out reward if claimed
            {
                ScratchoutTitle.SetActive(true);
            }
            else
            {
                ScratchoutTitle.SetActive(false);
            }
        }

    }

    public void AcceptContract() // called by animator
    {

        Portal.SetTrigger("Grow");
        Invoke("SceneChange", 1f);
        Spectator.LevelIndex = LevelScroller.LevelIndex; // reference index to the button
        Spectator.ReturningFromLevel = true;
        Debug.Log(Spectator.LevelIndex);
        UpgradesBar.SetTrigger("Hide");
        gameObject.GetComponent<Animator>().SetBool("Open", false);
    }

    public void ShrinkContract()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Accepted");
    }

    public void SceneChange()
    {
        SceneManager.LoadScene(SceneName);
    }

    public void DeclineContract()
    {
        gameObject.GetComponent<Animator>().SetBool("Open", false);
        UpgradesBar.SetTrigger("Appear");
    }
}
