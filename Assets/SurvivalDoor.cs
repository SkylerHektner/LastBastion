using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalDoor : MonoBehaviour
{
    public Animator MyDoor;
    public string HighestScoreText;
    //public GameObject ConfirmationMenu;
    public Button MyButton;


    public void OpenDoor()
    {
        MyDoor.SetTrigger("Open");

    }

    public void CloseDoor()
    {
        MyDoor.SetTrigger("Close");

    }

    public void AskConfirmation() // ask before loading level
    {
        //ConfirmationMenu.SetActive(true);
    }

    public void DenyConfirmation()
    {

    }

    public void AcceptConfirmation()
    {

    }
}
