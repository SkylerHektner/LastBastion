﻿using System.Collections;
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

    public void LoadMenuFromLimboResumeAnimation()
    {
        ProgressContent.SetActive( false );
#if UNITY_EDITOR
        Debug.Log( "Loading Menu" );
#endif
        PD.Instance.Limbo.Set( false );
        Spectator.LevelIndex = 1;
    }
    public void LoadSceneFromLimboResumeAnimation() // need to make a separate case here for survival
    {
        Spectator.LevelIndex = PD.Instance.StoredLimboLevelIndex.Get();
        Spectator.ReturningFromLevel = true;
        SceneManager.LoadScene( PD.Instance.ExitedScene.Get() );
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
