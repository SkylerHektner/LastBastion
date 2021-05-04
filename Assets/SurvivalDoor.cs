using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SurvivalDoor : MonoBehaviour
{
    public Animator MyDoor;
    public TextMeshProUGUI HighestScore;
    //public GameObject ConfirmationMenu;
    public Button MyButton;
    public string SceneToLoad;
    public Image DoorPortal;
    public Color GlowColor;
    public EndlessScroller ScrollBar;

    public void Update()
    {
        //HighestScore.text == // highest score.text
        DoorPortal.color = GlowColor;
    }

    public void OpenDoor()
    {
        Spectator.SurvivalIndex = EndlessScroller.EndlessLevelIndex; // reference index to the button
        //DoorPortal.color = GlowColor;
        MyDoor.SetTrigger("Open");
        ScrollBar.ArrowL.GetComponent<Button>().interactable = false;
        ScrollBar.ArrowR.GetComponent<Button>().interactable = false;
        ScrollBar.HomeButton.interactable = false;
    }

    public void CloseDoor()
    {
        //DoorPortal.color = GlowColor;
        MyDoor.SetTrigger("Close");
    }


    // called by door animator
    public void LoadLevel()
    {
        Spectator.SurvivalIndex = EndlessScroller.EndlessLevelIndex; // reference index to the button
        Spectator.ReturningFromSurvival = true;
        PD.Instance.StoredLimboSurvivalIndex.Set(Spectator.SurvivalIndex);
        SceneManager.LoadScene(SceneToLoad);
    }

}
