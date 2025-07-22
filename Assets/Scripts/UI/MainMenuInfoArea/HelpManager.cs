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

    public GameObject[] Tips1;
    public GameObject[] Tips2;
    public GameObject[] Tips3;
    int CurrentIndex;
    public GameObject[] ActiveList;

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
        ResetList();
        ActiveList = Tips3;
        ActiveList[0].SetActive(true);
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
        ResetList();
        ActiveList = Tips2;
        ActiveList[0].SetActive(true);
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
        ResetList();
        ActiveList = Tips1;
        ActiveList[0].SetActive(true);
    }

    public void NextTip()
    {
        ActiveList[CurrentIndex].SetActive(false); // turn existing tip OFF
        CurrentIndex++;
        if (CurrentIndex >= ActiveList.Length)
        {
            CurrentIndex = ActiveList.Length - 1;
        }
        ActiveList[CurrentIndex].SetActive(true); // turn new tip ON
    }

    public void PreviousTip()
    {
        ActiveList[CurrentIndex].SetActive(false); // turn existing tip OFF
        CurrentIndex--;
        if (CurrentIndex <= 0)
        {
            CurrentIndex = 0;
        }
        ActiveList[CurrentIndex].SetActive(true); // turn new tip ON
    }

    public void ResetList()
    {
        CurrentIndex = 0;

        foreach (GameObject Tip in Tips1)
        {
            Tip.SetActive(false);
        }
        foreach (GameObject Tip in Tips2)
        {
            Tip.SetActive(false);
        }
        foreach (GameObject Tip in Tips3)
        {
            Tip.SetActive(false);
        }

    }
}
