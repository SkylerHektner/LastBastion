﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    public string SceneToLoad;
    public bool Locked {
        get {
            return locked;
        }
        set {
            locked = value;

            if( locked )
            {
                LockedSymbol.SetActive( true );
                gameObject.GetComponent<Button>().enabled = false;
                LevelImage = LockedImage;
            }
            else
            {
                LockedSymbol.SetActive( false );
                gameObject.GetComponent<Button>().enabled = true;
                LevelImage = UnlockedImage; // Display the requirement to unlock level here
            }

            LockStatusChangedEvent.Invoke( transform.GetSiblingIndex() + 1 );
        }
    }
    private bool locked;
    public GameObject LockedSymbol;

    public Sprite UnlockedImage;
    public Sprite LockedImage;

    public Sprite LevelImage; // This is the one referenced by the level scroller.  Don't put anything in this;
    public Animator UpgradesBar;

    public UnityEvent<int> LockStatusChangedEvent = new UnityEvent<int>();

    public Vector4 GlowRGB;
    public Animator LevelInfo;


    // If I don't call this, the image appears blank white at the start.  
    //This is ok to do because the player only sees this when starting the game and returning from an unlocked level
    private void Awake()
    {
        LevelImage = UnlockedImage;

        Locked = false;
        bool my_bool = Locked;
    }


    public void ShowContract()
    {
        LevelInfo.SetBool("Open", true);
        LevelPopup.SceneName = SceneToLoad; // tell the popup what scene I want it to load
        UpgradesBar.SetTrigger("Hide");
    }


    [ContextMenu( "ToggleLocked" )]
    private void ToggleLocked()
    {
        Locked = !Locked;
    }
}
