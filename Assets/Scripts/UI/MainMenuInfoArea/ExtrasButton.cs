using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtrasButton : MonoBehaviour
{
    public List<Animator> ChainList;
    float ChainsSFXCooldown;
    bool ChainsCanRattle;
    public CameraUIMover MenuManager;
    bool ExtrasUnlocked;
    public Boombox GameMusic;
    public Sprite ExtrasUnlockedImage;

    public void RattleAllChains()
    {
        if (ExtrasUnlocked == false) // if player does not have this. Shake those chains!
        {
            if (ChainsCanRattle)
            {
                foreach (Animator Chain in ChainList)
                {
                    Chain.SetTrigger("Rattle");
                    ChainsCanRattle = false;
                    ChainsSFXCooldown = 1f;
                }
            }
        }
    }

    public void FixedUpdate()
    {
        ExtrasUnlocked = PD.Instance.LevelCompletionMap.GetLevelCompletion("Level 1");
        if (ExtrasUnlocked)
        {
            foreach (Animator Chain in ChainList)
            {
                Chain.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (Animator Chain in ChainList)
            {
                Chain.gameObject.SetActive(true);
            }
        }

        if (ChainsCanRattle == false) // delays the chains from being spammed
        {
            ChainsSFXCooldown -= Time.smoothDeltaTime;
            if (ChainsSFXCooldown <= 0)
            {
                ChainsSFXCooldown = 0; // no negatives pls
                ChainsCanRattle = true;
            }
        }
    }

    public void LoadExtrasArea() // Connected to the MenuManager gameobject
    {
        if (ExtrasUnlocked)
        {
            MenuManager.LoadExtras();
        }
    }
}
