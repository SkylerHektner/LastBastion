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

    // Gotta remember to delete this debug stuff
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DisplayReward();
        }
    }
}
