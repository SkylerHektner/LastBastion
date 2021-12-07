using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour
{
    public RectTransform GameTypeInfo;
    public RectTransform PowerupsInfo;
    public RectTransform HowToPlayInfo;
    public GameObject HowToText;
    public GameObject PowerupText;
    public GameObject GameTypeText;


    public ScrollRect HelpInfoBar;

    // Start is called before the first frame update
    void Awake()
    {
        ToggleOnHowToPlayInfo(); // by default, set this one       
    }

    public void ToggleOnGameTypeInfo()
    {
        HelpInfoBar.content = GameTypeInfo;
        GameTypeInfo.gameObject.SetActive(true);
        PowerupsInfo.gameObject.SetActive(false);
        HowToPlayInfo.gameObject.SetActive(false);
        HowToText.SetActive(false);
        PowerupText.SetActive(false);
        GameTypeText.SetActive(true);
        ResetBarPos();

    }

    public void ToggleOnPowerupsInfo()
    {
        HelpInfoBar.content = PowerupsInfo;
        GameTypeInfo.gameObject.SetActive(false);
        PowerupsInfo.gameObject.SetActive(true);
        HowToPlayInfo.gameObject.SetActive(false);
        HowToText.SetActive(false);
        PowerupText.SetActive(true);
        GameTypeText.SetActive(false);
        ResetBarPos();

    }

    public void ToggleOnHowToPlayInfo()
    {
        HelpInfoBar.content = HowToPlayInfo;
        GameTypeInfo.gameObject.SetActive(false);
        PowerupsInfo.gameObject.SetActive(false);
        HowToPlayInfo.gameObject.SetActive(true);
        HowToText.SetActive(true);
        PowerupText.SetActive(false);
        GameTypeText.SetActive(false);
        ResetBarPos();

    }

    public void ResetBarPos()
    {
        HelpInfoBar.content.localPosition = Vector3.zero;
    }
}
