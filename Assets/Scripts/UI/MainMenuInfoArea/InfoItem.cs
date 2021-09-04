using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoItem : MonoBehaviour
{
    public Achievement AchievementInformation;
    public GameObject LockedSymbol;
    public bool AchievementLocked
    {
        get
        {
            return locked;
        }
        set
        {
            locked = value;

            if (locked)
            {
                LockedSymbol.SetActive(true);
            }
            else
            {
                LockedSymbol.SetActive(false);
            }
       }
    }
    private bool locked;

    public virtual void Start()
    {

    }

    public virtual string GetInfoName()
    {
        return AchievementInformation.Name;
    }
    public virtual string GetInfoDescription()
    {
        return AchievementInformation.Description;
    }


    [ContextMenu("ToggleLocked")]
    private void ToggleLocked()
    {
        AchievementLocked = !AchievementLocked;
    }

    public virtual string GetProgressAmount()
    {
        float Progress = AchievementInformation.GetProgress();
        Progress = Progress * 100; // fix decimal
        if (Progress.ToString("F0") == "100") // complete?
        {
            AchievementLocked = false; // give me my trophy!
        }
        else
        {
            AchievementLocked = true; // hide the trophy
        }
        return Progress.ToString("F0") + "% Complete";

    }

}
