using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPopup : MonoBehaviour
{
    public static string SceneName;
    public int Index;
    public Animator UpgradesBar;
    public Animator Portal;


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
