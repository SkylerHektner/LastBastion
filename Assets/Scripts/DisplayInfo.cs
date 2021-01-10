using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayInfo : MonoBehaviour
{

    public string PowerupName;
    public string PowerupInfo;

    public TextMeshProUGUI PowerupText;
    public TextMeshProUGUI PowerupInfoText;


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

}
