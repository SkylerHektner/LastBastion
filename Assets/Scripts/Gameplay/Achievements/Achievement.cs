using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[CreateAssetMenu( fileName = "Achievement", menuName = "ScriptableObjects/Achievement", order = 0 )]
public class Achievement : ScriptableObject
{
    public enum AchievementType
    {
        BeatLevels,
        UnlockEverythingInCampaign,
        CompleteChallenges,
        KillEnemies,
        UseCrystals,
        KillEnemiesWithTurrets,
        KillZappedEnemies,
        UnleashAtLeastXSawsDuringAnomaly,
        KillAtLeastXEnemiesWithSawmageddonShot,
        BeatWaveInSurvival,
        SetSawOnFire,
        BeatEnemy,
    }

    public string UniqueID;
    public string Name;
    public string Description;
    public AchievementType Type;
    public int desired_value;
    public EnemyEnum desired_enemy;
    public List<string> desired_strings;

    // checks the progress of this acheivement. 0.0f means no progress, 1.0f means complete. Any number between is the percentage of completion
    public float GetProgress()
    {
        float result = 0.0f;
        switch( Type )
        {
            case AchievementType.BeatLevels:
                result = (float)desired_strings.Count( s => PD.Instance.LevelCompletionMap.GetLevelCompletion( s ) ) / (float)desired_strings.Count;
                break;
            case AchievementType.UnlockEverythingInCampaign:
            {
                int num_boons = 0;
                int num_earned_in_campaign = 0;
                foreach( UnlockFlags unlock in Enum.GetValues( typeof( UnlockFlags ) ) )
                {
                    if( !PD.Instance.UnlockFlagCurseMap[unlock] )
                    {
                        ++num_boons;
                        if( PD.Instance.UnlockMap.Get( unlock, false ) )
                        {
                            ++num_earned_in_campaign;
                        }
                    }
                }
                result = (float)num_earned_in_campaign / (float)num_boons;
            }
            break;
            case AchievementType.CompleteChallenges:
                result = (float)desired_strings.Count( s => PD.Instance.PlayerChallengeCompletionList.Contains( s ) ) / (float)desired_strings.Count;
                break;
            case AchievementType.KillEnemies:
                result = (float)PD.Instance.NumKilledEnemies.Get() / (float)desired_value;
                break;
            case AchievementType.UseCrystals:
                result = (float)PD.Instance.NumCrystalsUsed.Get() / (float)desired_value;
                break;
            case AchievementType.KillEnemiesWithTurrets:
                result = (float)PD.Instance.NumTurretKills.Get() / (float)desired_value;
                break;
            case AchievementType.KillZappedEnemies:
                result = (float)PD.Instance.NumZappedEnemiesKilled.Get() / (float)desired_value;
                break;
            case AchievementType.UnleashAtLeastXSawsDuringAnomaly:
                result = (float)PD.Instance.HighestAnomalySawUnleash.Get() / (float)desired_value;
                break;
            case AchievementType.KillAtLeastXEnemiesWithSawmageddonShot:
                result = (float)PD.Instance.HighestEnemyDeathTollFromSawmageddonShot.Get() / (float)desired_value;
                break;
            case AchievementType.BeatWaveInSurvival:
                result = (float)PD.Instance.HighestSurvivalWave.Get() / (float)desired_value;
                break;
            case AchievementType.SetSawOnFire:
                result = (float)PD.Instance.NumTimesSawOnFire.Get() / (float)desired_value;
                break;
            case AchievementType.BeatEnemy:
                result = PD.Instance.EncounteredEnemyList.Contains( desired_enemy ) ? 1.0f : 0.0f;
                break;
        }
        return Mathf.Min( result, 1.0f );
    }
}

#if UNITY_EDITOR
[CustomEditor( typeof( Achievement ) )]
public class AchievementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Achievement achievement = (Achievement)target;
        CustomEditorUtilities.AutoDirtyLabeledString(
            ref achievement.UniqueID,
            "UniqueID",
            false,
            target );
        CustomEditorUtilities.AutoDirtyLabeledString(
            ref achievement.Name,
            "Name",
            false,
            target );
        CustomEditorUtilities.AutoDirtyLabeledString(
            ref achievement.Description,
            "Description",
            true,
            target );

        Achievement.AchievementType type = (Achievement.AchievementType)EditorGUILayout.EnumPopup( "Type: ", achievement.Type );
        if( type != achievement.Type )
        {
            achievement.Type = type;
            EditorUtility.SetDirty( target );
        }

        switch( achievement.Type )
        {
            // enter number
            case Achievement.AchievementType.KillEnemies:
            case Achievement.AchievementType.UseCrystals:
            case Achievement.AchievementType.KillEnemiesWithTurrets:
            case Achievement.AchievementType.KillZappedEnemies:
            case Achievement.AchievementType.UnleashAtLeastXSawsDuringAnomaly:
            case Achievement.AchievementType.KillAtLeastXEnemiesWithSawmageddonShot:
            case Achievement.AchievementType.BeatWaveInSurvival:
            case Achievement.AchievementType.SetSawOnFire:
            {
                CustomEditorUtilities.AutoDirtyLabeledInt(
                    ref achievement.desired_value,
                    "Desired Value",
                    target );
            }
            break;

            // enter strings
            case Achievement.AchievementType.BeatLevels:
            case Achievement.AchievementType.CompleteChallenges:
            {
                EditorGUILayout.LabelField( "-------- Desired Strings --------" );
                if(GUILayout.Button("Add Entry"))
                {
                    achievement.desired_strings.Add( "" );
                }

                for(int x = 0; x < achievement.desired_strings.Count; ++x)
                {
                    EditorGUILayout.BeginHorizontal();
                    string new_string = EditorGUILayout.TextField( achievement.desired_strings[x] );
                    if(new_string != achievement.desired_strings[x])
                    {
                        achievement.desired_strings[x] = new_string;
                        EditorUtility.SetDirty( target );
                    }

                    CustomEditorUtilities.ListItemControlButtonsUnsafe<string>( achievement.desired_strings, ref x, target );
                    EditorGUILayout.EndHorizontal();
                }
            }
            break;

            // enter enemy type
            case Achievement.AchievementType.BeatEnemy:
            {
                EnemyEnum new_enemy = (EnemyEnum)EditorGUILayout.EnumPopup( "Desired Enemy", achievement.desired_enemy );
                if(new_enemy != achievement.desired_enemy)
                {
                    achievement.desired_enemy = new_enemy;
                    EditorUtility.SetDirty( target );
                }
            }
            break;

            // do nothing
            case Achievement.AchievementType.UnlockEverythingInCampaign:
            break;
        }
    }
}
#endif