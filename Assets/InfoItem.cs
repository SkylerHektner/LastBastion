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


    public string GetInfoName()
    {
        return AchievementInformation.Name;
    }
    public string GetInfoDescription()
    {
        return AchievementInformation.Description;
    }


    [ContextMenu("ToggleLocked")]
    private void ToggleLocked()
    {
        AchievementLocked = !AchievementLocked;
    }


}
