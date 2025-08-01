﻿using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Spectator : MonoBehaviour
{
    public static Spectator Instance;

    public GlobalData GD;

    public SteamManager SteamManagerInstance;

    public bool InGamepadMode = false;

    public static bool ReturningFromLevel;
    public static bool ReturningFromSurvival;
    public static int LevelIndex;
    public static int SurvivalIndex;

    public static bool WitnessedVictory;
    public AudioSource GameMusic;
    public Texture2D CursorTexture;
    public Texture2D CursorTexture2;
    public RewardCanvas AchievementPopup;
    public float AchievementCheckRate = 1.0f;

    private float achievementCheckCooldown;

    private void Start()
    {
    }

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag( "Spectator" );
        if( objs.Length > 1 ) // no duplicate spectators (unfortunately this works but I really don't like it)
        {
            Destroy( this.gameObject );
            return;
        }

        achievementCheckCooldown = AchievementCheckRate;
        DontDestroyOnLoad( this.gameObject );
        Instance = this;

        Debug.Assert( GD != null, "ERROR: Global Data is null in Spectator. This will break a LOT of stuff" );
        GD?.Verify();
        LevelIndex = PD.Instance.CampaignLimboResumeInformation.LevelIndex;

        SteamManagerInstance = new SteamManager();
    }

    private void Update()
    {
        if( GameplayManager.Instance == null )
            PD.Instance.Tick();

        SteamManagerInstance.Update(Time.deltaTime);

        // makes the cursor a finger texture
        if ( Input.GetMouseButton( 0 ) )
        {
            Cursor.SetCursor( CursorTexture2, Vector2.zero, CursorMode.ForceSoftware );
        }
        else if( Input.GetMouseButtonUp( 0 ) )
        {
            Cursor.SetCursor( CursorTexture, Vector2.zero, CursorMode.ForceSoftware );
        }

        // show the achievement popup if the players gets an achievement
        achievementCheckCooldown -= Time.deltaTime;
        if( achievementCheckCooldown < 0.0f )
        {
            achievementCheckCooldown = AchievementCheckRate;
            CheckAchievementCompletion();
        }

        // Gross hack to fix a content problem where players are missing 5 candies needed to buy all upgrades
        if( !PD.Instance.OneTimeCandyBonusClaimed.Get() &&
            PD.Instance.LevelCompletionMap.GetLevelCompletion( "Level 1" ) &&
            PD.Instance.LevelCompletionMap.GetLevelCompletion( "Level 2" ) &&
            PD.Instance.LevelCompletionMap.GetLevelCompletion( "Level 3" ) &&
            PD.Instance.LevelCompletionMap.GetLevelCompletion( "Level 4" ) &&
            PD.Instance.LevelCompletionMap.GetLevelCompletion( "Level 5" ) &&
            PD.Instance.LevelCompletionMap.GetLevelCompletion( "Level 6" ) &&
            PD.Instance.LevelCompletionMap.GetLevelCompletion( "Level 7" ) &&
            PD.Instance.LevelCompletionMap.GetLevelCompletion( "Level 8" ) &&
            PD.Instance.EarnedAchievementList.Contains( "BeatChallenges" ) )
        {
            PD.Instance.OneTimeCandyBonusClaimed.Set( true );
            PD.Instance.PlayerWealth.Set( PD.Instance.PlayerWealth.Get() + 5 );
        }

        Gamepad gp = Gamepad.current;
        if (gp != null && !InGamepadMode)
        {
            Cursor.visible = true;
            // lord forgive my sins (NO)
            if (gp.rightShoulder.wasPressedThisFrame ||
                gp.rightStickButton.wasPressedThisFrame ||
                gp.rightTrigger.wasPressedThisFrame ||
                gp.leftShoulder.wasPressedThisFrame ||
                gp.leftStickButton.wasPressedThisFrame ||
                gp.leftTrigger.wasPressedThisFrame ||
                gp.startButton.wasPressedThisFrame ||
                gp.selectButton.wasPressedThisFrame ||
                gp.aButton.wasPressedThisFrame ||
                gp.bButton.wasPressedThisFrame ||
                gp.xButton.wasPressedThisFrame ||
                gp.yButton.wasPressedThisFrame ||
                gp.dpad.left.wasPressedThisFrame ||
                gp.dpad.right.wasPressedThisFrame ||
                gp.dpad.up.wasPressedThisFrame ||
                gp.dpad.down.wasPressedThisFrame ||
                gp.leftStick.left.wasPressedThisFrame ||
                gp.leftStick.right.wasPressedThisFrame ||
                gp.leftStick.up.wasPressedThisFrame ||
                gp.leftStick.down.wasPressedThisFrame ||
                gp.rightStick.left.wasPressedThisFrame ||
                gp.rightStick.right.wasPressedThisFrame ||
                gp.rightStick.up.wasPressedThisFrame ||
                gp.rightStick.down.wasPressedThisFrame)
            {
                Debug.Log("going to gamepad mode");
                InGamepadMode = true;
            }
        }
        else if (InGamepadMode)
        {
            Cursor.visible = false;
            if (gp == null ||
                Input.GetMouseButtonDown(0) ||
                Input.GetMouseButtonDown(1) ||
                Input.touchCount > 0)
            {
                Debug.Log("going to non gamepad mode");
                InGamepadMode = false;
            }
        }
    }

    private void CheckAchievementCompletion()
    {
        foreach( var achievement in GD.Achievements )
        {
            if( achievement.ShowPopup &&
                !PD.Instance.EarnedAchievementList.Contains( achievement.UniqueID )
                && achievement.GetProgress() == 1.0f )
            {
                AchievementPopup.DisplayReward();
                AchievementPopup.SetText( achievement.Name );
                PD.Instance.EarnedAchievementList.Add( achievement.UniqueID );
                PD.Instance.AchievementPoints.Set( PD.Instance.AchievementPoints.Get() + achievement.Payout );
                achievementCheckCooldown = AchievementCheckRate * 3; // make sure the animation has enough time to finish before we check again incase we got two achievements at once

                // Record completion for Steam
                Steamworks.Data.Achievement steam_achievement = new Steamworks.Data.Achievement(achievement.UniqueID);
                if (!steam_achievement.State)
                {
                    steam_achievement.Trigger(true);
                }

                break;
            }
        }
    }

    // used for debugging (call this mid game, then stop the editor, then start at the main menu)
    [ContextMenu( "ToggleLimbo" )]
    public void TryToggleLimbo()
    {
        if( SceneManager.GetActiveScene().name != "Menu" && SceneManager.GetActiveScene().name != "Credits"
            && ( GameplayManager.State == GameplayManager.GameState.Active
            || GameplayManager.State == GameplayManager.GameState.ChoosingUpgrade ) )
        {
            if( GameplayManager.Instance.Survival )
            {
                List<UnlockFlag> survival_unlock_flags = new List<UnlockFlag>();
                foreach( UnlockFlag flag in Enum.GetValues( typeof( UnlockFlag ) ) )
                {
                    if( PD.Instance.UnlockFlagCategoryMap[flag] == UnlockFlagCategory.Cosmetic )
                        continue;

                    if( PD.Instance.UnlockMap.Get( flag ) )
                    {
                        survival_unlock_flags.Add( flag );
                    }
                }
                PD.Instance.SurvivalLimboResumeInformation.SetInfo(
                true,
                SceneManager.GetActiveScene().name,
                SurvivalIndex,
                SpawnManager.Instance.CurrentWaveIndex,
                BaseHP.Instance.CurrentHP,
                survival_unlock_flags,
                InfiniteSpawnCadenceManager.Instance.GetCurrentSpawnCadenceIndex() );

                // get rid of campaign stats for limbo purposes
                PD.Instance.CampaignLimboResumeInformation.Clear();
            }
            else
            {
                PD.Instance.CampaignLimboResumeInformation.SetInfo(
                true,
                SceneManager.GetActiveScene().name,
                LevelIndex,
                SpawnManager.Instance.CurrentWaveIndex,
                BaseHP.Instance.CurrentHP,
                null );

                //get rid of survival stats for limbo purposes
                PD.Instance.SurvivalLimboResumeInformation.Clear();
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
        // VERY IMPORT - this function must cache then restore premium cosmetics
        List<UnlockFlag> unlocked_premium_cosmetics = new List<UnlockFlag>();
        foreach( UnlockFlag flag in Enum.GetValues( typeof( UnlockFlag ) ) )
        {
            if( PD.Instance.UnlockFlagCategoryMap[flag] == UnlockFlagCategory.Cosmetic &&
                GD.GetCosmeticFromUnlockFlag( flag ).Premium &&
                ( PD.Instance.UnlockMap.Get( flag, false ) ||
                 PD.Instance.UnlockMap.Get( flag, true ) ) )
            {
                unlocked_premium_cosmetics.Add( flag );
            }
        }

        PD.DeleteAllPlayerData();

        foreach( UnlockFlag flag in unlocked_premium_cosmetics )
        {
            PD.Instance.UnlockMap.Set( flag, true, false );
            PD.Instance.UnlockMap.Set( flag, true, true );
        }

        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }
}
