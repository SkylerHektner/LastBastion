using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardCanvas : MonoBehaviour
{
    public Animator RewardScreen;
    public TMPro.TextMeshProUGUI tmp;

    [ContextMenu("DisplayReward")]
    public void DisplayReward()
    {
        RewardScreen.SetTrigger("Display");
    }

    public void SetText(string text)
    {
        if( tmp == null )
            Debug.LogWarning( "Trying to set text on Reward Canvas without a text mesh pro assigned" );

        tmp?.SetText( text );
    }
}
