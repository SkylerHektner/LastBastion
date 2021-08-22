using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressCanvas : MonoBehaviour
{
    public Animator BlackBG;
    public Animator StarClustersA;
    public Animator StarClustersB;

    public Animator Skull;
    public GameObject ProgressContent;

    private void Start()
    {
        Skull.SetBool( "Speaking", true );
    }

    public void ContinueSave()
    {

    }

    public void DeclineSave()
    {
        Skull.SetBool( "Speaking", false );
        // play animation (call load menu at the end)
    }

    public void TriggerBlackFadeFX()
    {
        BlackBG.SetTrigger("Fade");
    }

    public void TriggerStarsFadeFX()
    {
        StarClustersA.SetTrigger("Fade");
        StarClustersB.SetTrigger("Fade");

    }
}
