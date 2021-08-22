using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spectator : MonoBehaviour
{
    public static bool ReturningFromLevel;
    public static bool ReturningFromSurvival;
    public static int LevelIndex;
    public static int SurvivalIndex;

    public static bool WitnessedVictory;
    public AudioSource GameMusic;

    public Texture2D CursorTexture;
    public Texture2D CursorTexture2;

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


        // makes the cursor a finger texture
        if( Input.GetMouseButton( 0 ) )
        {
            Cursor.SetCursor( CursorTexture2, Vector2.zero, CursorMode.ForceSoftware );
        }
        else if( Input.GetMouseButtonUp( 0 ) )
        {
            Cursor.SetCursor( CursorTexture, Vector2.zero, CursorMode.ForceSoftware );
        }
    }

    // used for debugging (call this mid game, then stop the editor, then start at the main menu)
    [ContextMenu( "ToggleLimbo" )]
    public void TryToggleLimbo()
    {
        if( SceneManager.GetActiveScene().name != "Menu"
            && ( GameplayManager.State == GameplayManager.GameState.Active
            || GameplayManager.State == GameplayManager.GameState.ChoosingUpgrade))
        {
            if( GameplayManager.Instance.Survival )
            {
                List<UnlockFlags> survival_unlock_flags = new List<UnlockFlags>();
                foreach( UnlockFlags flag in Enum.GetValues( typeof( UnlockFlags ) ) )
                {
                    if( PD.Instance.UnlockMap.Get( flag ) )
                    {
                        survival_unlock_flags.Add( flag );
                    }
                }
                PD.Instance.SurvivalLimboResumeInformation.SetInfo(
                true,
                SceneManager.GetActiveScene().name,
                SpawnManager.Instance.CurrentWaveIndex,
                BaseHP.Instance.CurrentHP,
                survival_unlock_flags );
            }
            else
            {
                PD.Instance.CampaignLimboResumeInformation.SetInfo(
                true,
                SceneManager.GetActiveScene().name,
                SpawnManager.Instance.CurrentWaveIndex,
                BaseHP.Instance.CurrentHP,
                null );
            }
        }
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
        if( pause )
        {
#if !UNITY_EDITOR
        TryToggleLimbo();
#endif
            if( GameplayManager.State == GameplayManager.GameState.Active )
            {
                PauseManager.Instance?.PauseGame();
            }
        }
    }

    public void WipeProgress()
    {
        PD.DeleteAllPlayerData();
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }
}
