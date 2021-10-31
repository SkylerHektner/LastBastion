using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoItem : MonoBehaviour
{
    public Achievement AchievementInformation;
    public bool EnemyInfo;
    public bool HiddenAchievement;
    public GameObject LockedSymbol;
    public TextMeshProUGUI PayoutText;
    public Animator Strikeout;

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
        GetProgressAmount();
        if (EnemyInfo || HiddenAchievement) // case for enemy info 
        {
            if (AchievementLocked) // this will unlock once you discover the enemy
            {
                return "???";
            }
            else
            {
                return AchievementInformation.Name;
            }
        }
        else
        {
            return AchievementInformation.Name;
        }
    }
    public virtual string GetInfoDescription()
    {
        GetProgressAmount();
        if (EnemyInfo || HiddenAchievement) // case for enemy info 
        {
            if (AchievementLocked) // this will unlock once you discover the enemy or complete achievement
            {
                return "?????????";
            }
            else
            {
                return AchievementInformation.Description;
            }
        }
        else
        {
            return AchievementInformation.Description;
        }
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
            Strikeout.SetTrigger("Strikeout");
            Strikeout.ResetTrigger("Hide");
        }
        else
        {
            AchievementLocked = true; // hide the trophy
            PayoutText.text = AchievementInformation.Payout.ToString();
            Strikeout.SetTrigger("Hide");
            Strikeout.ResetTrigger("Strikeout");
        }
        return Progress.ToString("F0") + "% Complete";

    }

}
