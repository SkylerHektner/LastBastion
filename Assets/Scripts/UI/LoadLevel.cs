using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    public SpawnCadenceProfile SpawnCadence;
    // list of level identifiers that must be marked complete for level to be unlocked
    public List<string> RequiredLevelCompletion = new List<string>();

    public bool Locked
    {
        get
        {
            return locked;
        }
        set
        {
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

    public string MyGameModeText;
    public string MyDescription;


    public LevelPopup ContractPopup;


    // If I don't call this, the image appears blank white at the start.  
    //This is ok to do because the player only sees this when starting the game and returning from an unlocked level
    private void Awake()
    {
        LevelImage = UnlockedImage;

        Locked = !RequiredLevelCompletion.TrueForAll( level_identifier =>
            PD.Instance.LevelCompletionMap.GetLevelCompletion( level_identifier ) );
    }


    public void ShowContract()
    {
        LevelInfo.SetBool( "Open", true );
        LevelPopup.ActivePopupSpawnCadence = SpawnCadence; // tell the popup what scene I want it to load
        UpgradesBar.SetTrigger( "Hide" );
        // load contract info
        UpdateContract();
    }

    public void UpdateContract()
    {
        ContractPopup.GameModeText.text = MyGameModeText;
        ContractPopup.Description.text = MyDescription;

        Debug.Assert( SpawnCadence != null );
        Challenge challenge = SpawnCadence.LevelChallenge;
        if( challenge != null )
        {
            ContractPopup.CompletionPayout.text = SpawnCadence.Waves.Select( w => w.CompletionReward ).Sum().ToString();
            ContractPopup.ChallengeText.text = challenge.ChallengeDescription;
        }
        else
        {
            ContractPopup.CompletionPayout.text = "";
            ContractPopup.ChallengeText.text = "";
        }
    }


    [ContextMenu( "ToggleLocked" )]
    private void ToggleLocked()
    {
        Locked = !Locked;
    }
}
