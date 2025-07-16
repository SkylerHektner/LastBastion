using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ProgressCanvas : MonoBehaviour
{
    public Animator BlackBG;
    public Animator StarClustersA;
    public Animator StarClustersB;

    public Animator Skull;
    public GameObject ProgressContent;
    public TextMeshProUGUI ModeText;
    public TextMeshProUGUI WaveText;
    string Scene2Load;
    public Boombox MenuTrack;
    public Button ContinueButton;
    public Button CampaignButton;


    private void Start()
    {
        if (Skull.isActiveAndEnabled)
        {
            Skull.SetBool("Speaking", true);
        }
    }

    public void ContinueSave()
    {

    }
    public void SelectStartingContinueButton()
    {
        ContinueButton.Select();
    }
    public void SelectCampaignButton()
    {
        CampaignButton.Select();
    }

    public void Awake()
    {
        if (PD.Instance.CampaignLimboResumeInformation != null && PD.Instance.SurvivalLimboResumeInformation != null)
        {
            if (PD.Instance.SurvivalLimboResumeInformation.Active && PD.Instance.CampaignLimboResumeInformation.Active == false) // player came from survival
            {
                ModeText.text = "Survival";
                WaveText.text = "Wave " + (PD.Instance.SurvivalLimboResumeInformation.Wave + 1); // it was showing up as 1 less than normal so I added the 1 back
                Scene2Load = PD.Instance.SurvivalLimboResumeInformation.SceneName;
                Debug.Log("Coming from survival");
            }
            else if (PD.Instance.CampaignLimboResumeInformation.Active && PD.Instance.SurvivalLimboResumeInformation.Active == false) // player came from campaign
            {
                ModeText.text = "Campaign";
                WaveText.text = "Zone " + PD.Instance.CampaignLimboResumeInformation.LevelIndex + " - Wave " + (PD.Instance.CampaignLimboResumeInformation.Wave + 1);
                Scene2Load = PD.Instance.CampaignLimboResumeInformation.SceneName;
                Debug.Log("Coming from campaign");

            }
        }

    }

    public void DeclineSave()
    {
        Skull.SetBool( "Speaking", false );
        // play animation (call load menu at the end)
        PD.Instance.CampaignLimboResumeInformation.Clear();
        PD.Instance.SurvivalLimboResumeInformation.Clear();
    }

    public void TriggerBlackFadeFX()
    {
        BlackBG.SetTrigger("Fade");
    }

    public void TriggerStarsFadeFX()
    {
        StarClustersA.SetTrigger("Fade");
        StarClustersB.SetTrigger("Fade");
    }

    public void LoadSceneFromLimboResumeAnimation()
    {
        Spectator.LevelIndex = PD.Instance.CampaignLimboResumeInformation.LevelIndex;
        Spectator.ReturningFromLevel = true;
        SceneManager.LoadScene(Scene2Load);
    }

    public void LoadMenuFromLimboResumeAnimation()
    {
        ProgressContent.SetActive(false);
        Spectator.ReturningFromLevel = true;
        MenuTrack.gameObject.SetActive(true); // toggle music soundtrack when transitioned
        //MenuTrack.SwapTrack(MenuTrack.MyClip);
        SelectCampaignButton();
    }
}
