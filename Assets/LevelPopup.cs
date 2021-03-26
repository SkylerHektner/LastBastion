using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelPopup : MonoBehaviour
{
    public static SpawnCadenceProfile ActivePopupSpawnCadence;
    public int Index;
    public Animator UpgradesBar;
    public Animator Portal;
    public TextMeshProUGUI GameModeText;
    public TextMeshProUGUI CompletionPayout;
    public TextMeshProUGUI ChallengeText;
    public TextMeshProUGUI ChallengeOptionalTextDecorator;
    public TextMeshProUGUI Description;

    public GameObject ScratchoutTitle;
    public GameObject ScratchoutChallenge;

    private void Update()
    {
        if ( ActivePopupSpawnCadence != null)
        {
            ScratchoutTitle.SetActive( PD.Instance.LevelCompletionMap.GetLevelCompletion( ActivePopupSpawnCadence.LevelIdentifier ) );
            ScratchoutChallenge.SetActive( ActivePopupSpawnCadence.LevelChallenge != null
                && PD.Instance.PlayerChallengeCompletionList.Contains( ActivePopupSpawnCadence.LevelChallenge.UniqueChallengeID ) );
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
        SceneManager.LoadScene( ActivePopupSpawnCadence.LevelIdentifier );
    }

    public void DeclineContract()
    {
        gameObject.GetComponent<Animator>().SetBool("Open", false);
        UpgradesBar.SetTrigger("Appear");
    }
}
