using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BonusMusic : MonoBehaviour
{
    public string MyTrackName;
    public bool Discovered;
    public Boombox TheBoombox;
    public AudioClip MyTunes;
    public AudioClip DefaultMenuTrack;
    public Animator MyChains;
    float ChainsSFXCooldown;
    bool ChainsCanRattle;
    public GameObject MyActiveFX;
    public List<Animator> SkeleDancers;


    public TextMeshProUGUI DisplayedTrackName;

    private void Awake()
    {
        DisplayedTrackName.text = "Default";
    }
    public void PlayMyTunes() // called when the button is clicked
    {
        if (Discovered)
        {
            if (TheBoombox.GetCurrentTrack() == MyTunes) // already playing? Pause
            {
                TheBoombox.SwapTrack(DefaultMenuTrack);
                DisplayedTrackName.text = "Default";
                foreach (Animator skele in SkeleDancers)
                {
                    skele.SetBool("Dancing", false);
                }
            }
            else // different track? play it
            {
                TheBoombox.SwapTrack(MyTunes); // pop that track out and give it this cool new one
                DisplayedTrackName.text = MyTrackName; // gimme that track name while you're at it
                foreach (Animator skele in SkeleDancers)
                {
                    skele.SetBool("Dancing", true);
                }
            }
        }
        else
        {
            RattleMyChains(); // boo, haven't seen this level yet? Keep playing.
            DisplayedTrackName.text = "Undiscovered";
        }
    }

    public void FixedUpdate()
    {
        if (Discovered)
        {
            MyChains.gameObject.SetActive(false);
            if (DisplayedTrackName.text == MyTrackName) // not my favorite way to do this, but I'll worry about that later
            {
                MyActiveFX.SetActive(true);
            }
            else
            {
                MyActiveFX.SetActive(false);
            }
        }
        else
        {
            MyChains.gameObject.SetActive(true);
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

    public void RattleMyChains() 
    {
        if (ChainsCanRattle)
        {
            MyChains.SetTrigger("Rattle");
            ChainsCanRattle = false;
            ChainsSFXCooldown = 1f;
        }
    }


}
