using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BonusTuneData : MonoBehaviour
{
    public Boombox MainBoombox;
    public AudioClip BonusTrack;
    public string Level_ID; // "Level 1", "Level 2", "Survival1", etc
    public GameObject MyChains;
    bool Discovered;
    public List<Animator> ChainList;
    float ChainsSFXCooldown;
    bool ChainsCanRattle;
    public TextMeshProUGUI TrackName;
    public GameObject ActiveFX;
    public ExtrasButton ExtrasButton;
    public Sprite CassetteColor;
    public bool SurvivalTrack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        if (PD.Instance.LevelCompletionMap.GetLevelCompletion(Level_ID))
        {
            MyChains.SetActive(false);
            Discovered = true;
        }
        else if (PD.Instance.SurvivalDiscoveryMap.GetLevelCompletion(Level_ID)) // survival
        {
            MyChains.SetActive(false);
            Discovered = true;
        }
        else
        {
            Discovered = false;
        }
        if (MainBoombox.GetCurrentTrack() != BonusTrack)
        {
            ActiveFX.SetActive(false);
        }
        else
        {
            ActiveFX.SetActive(true);
        }
    }


    public void ChangeMusic()
    {
        if (MainBoombox.GetCurrentTrack() != BonusTrack) // don't do anything if already playing it
        {
            if (Discovered) // must beat that level to replay the track
            {
                MainBoombox.SwapTrack(BonusTrack);
                TrackName.text = Level_ID;
                ExtrasButton.BigCassette.sprite = CassetteColor;
                if (ExtrasButton.ActiveTrack != null)
                {
                    ExtrasButton.ActiveTrack.ActiveFX.SetActive(false); // turn off previous one's FX
                }
                ExtrasButton.ActiveTrack = this;
                ActiveFX.SetActive(true);
            }
            else
            {
                foreach (Animator Chain in ChainList)
                {
                    Chain.SetTrigger("Rattle");
                    ChainsCanRattle = false;
                    ChainsSFXCooldown = 1f;
                    //TrackName.text = "Undiscovered";
                }
            }
        }

    }

    public void FixedUpdate()
    {
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
}
