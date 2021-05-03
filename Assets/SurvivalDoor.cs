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

    //public void Update()
    //{
    //        HighestScore.text == // highest score.text
    //}

    public void OpenDoor()
    {
        DoorPortal.color = GlowColor;
        MyDoor.SetTrigger("Open");
    }

    public void CloseDoor()
    {
        DoorPortal.color = GlowColor;
        MyDoor.SetTrigger("Close");
    }

    public void AskConfirmation() // ask before loading level
    {
        //ConfirmationMenu.SetActive(true);
    }

    //public void DenyConfirmation()
    //{

    //}

    //public void AcceptConfirmation()
    //{

    //}

    // called by door animator
    public void LoadLevel()
    {
        SceneManager.LoadScene(SceneToLoad);
    }

}
