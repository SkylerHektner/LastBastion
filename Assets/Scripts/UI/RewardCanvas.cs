using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardCanvas : MonoBehaviour
{
    public Animator RewardScreen;

    [ContextMenu("DisplayReward")]
    public void DisplayReward()
    {
        RewardScreen.SetTrigger("Display");
    }
}
