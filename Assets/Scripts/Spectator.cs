﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spectator : MonoBehaviour
{
    public static bool ReturningFromLevel;
    public static int LevelIndex;

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag( "Spectator" );

        if( objs.Length > 1 ) // no duplicate spectators
        {
            Destroy( this.gameObject );
        }

        DontDestroyOnLoad( this.gameObject );
    }

    private void Update()
    {
        if( GameplayManager.Instance == null )
            PD.Instance.Tick();
    }

    // used for debugging (call this mid game, then stop the editor, then start at the main menu)
    [ContextMenu( "ToggleLimbo" )]
    public void TryToggleLimbo()
    {
        if( SceneManager.GetActiveScene().name != "Menu" 
            && GameplayManager.PlayerWinState == GameplayManager.PlayerState.Active )
        {
            PD.Instance.Limbo.Set( true );
            PD.Instance.ExitedScene.Set( SceneManager.GetActiveScene().name );
        }
        PD.Instance.StoredLimboLevelIndex.Set( Spectator.LevelIndex );
#if UNITY_EDITOR
        Debug.Log("Storing index: " + PD.Instance.StoredLimboLevelIndex.Get());
#endif
    }

    // game is quit mid-game, save my current progress and put me in limbo
    private void OnApplicationQuit()
    {
        // you can comment out this ifdef if you want to test this, or use the context menu instead
#if !UNITY_EDITOR 
        TryToggleLimbo();
#endif
    }

    private void OnApplicationPause( bool pause )
    {
        if(pause)
        {
#if !UNITY_EDITOR
        TryToggleLimbo();
#endif
            if( GameplayManager.PlayerWinState == GameplayManager.PlayerState.Active )
            {
                PauseManager.Instance?.PauseGame();
            }
        }
    }
}
