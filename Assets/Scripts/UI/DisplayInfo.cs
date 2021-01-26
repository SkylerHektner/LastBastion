using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayInfo : MonoBehaviour
{

    public string PowerupName;
    public string PowerupInfo;

    public TextMeshProUGUI PowerupText;
    public TextMeshProUGUI PowerupInfoText;

    public bool Locked
    {
        get
        {
            return lockedStatus;
        }
        set
        {
            lockedStatus = value;
            if (lockedStatus == true)
            {
                LockedSymbol.SetActive(true);
                PowerupIcon.raycastTarget = false;
                PowerupSlots.SetActive(false);
            }
            else
            {
                LockedSymbol.SetActive(false);
                PowerupIcon.raycastTarget = true;
                PowerupSlots.SetActive(true);
            }
        }
    }
    private bool lockedStatus;

    public GameObject LockedSymbol;
    public Image PowerupIcon;
    public GameObject PowerupSlots;


    // used with the pointer scroll over event
    public void DisplayMyInfo()
    {
        PowerupText.text = PowerupName;
        PowerupInfoText.text = PowerupInfo;
    }

    // used with pointer scroll off event
    public void HideInfo()
    {
        PowerupText.text = "";
        PowerupInfoText.text = "";
    }

    [ContextMenu("ToggleLock")]
    public void ToggleLock()
    {
        Locked = !Locked;
    }

    public void Update()
    {
        //debug stuff (simulates hitting a powerup while ability manager is active)
        if (Input.GetKeyDown(KeyCode.T))
        {
            PowerupIcon.GetComponent<Animator>().SetBool("Empty", false);
        }
    }


}
