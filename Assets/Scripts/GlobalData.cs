using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// THERE SHOULD ONLY BE ONE GLOBAL DATA STRUCTURE
[CreateAssetMenu( fileName = "Achievement", menuName = "ScriptableObjects/GlobalData", order = 0 )]
public class GlobalData : ScriptableObject
{
    public List<Achievement> Achievements;
    public List<Cosmetic> Cosmetics;
    public List<UnlockFlagUIInformation> UnlockFlagUIInfo;

    public void Verify()
    {
#if UNITY_EDITOR
        foreach( UnlockFlag flag in Enum.GetValues( typeof( UnlockFlag ) ) )
        {
            if( PD.Instance.UnlockFlagCategoryMap[flag] == UnlockFlagCategory.Cosmetic )
                continue;

            Debug.Assert( UnlockFlagUIInfo.Count( ui_info => ui_info.UnlockFlag == flag ) == 1,
                $"ERROR: Survival Cards UI missing unlock flag ui information for unlock flag {flag}" );
        }
#endif
    }

    public Cosmetic GetCosmeticFromUnlockFlag( UnlockFlag flag )
    {
        Debug.Assert( PD.Instance.UnlockFlagCategoryMap[flag] == UnlockFlagCategory.Cosmetic );

        foreach( Cosmetic cosmetic in Cosmetics )
        {
            if( cosmetic.unlock_flag == flag )
            {
                return cosmetic;
            }
        }

        Debug.LogError( $"ERROR! Cosmetic not found for unlock flag {flag}" );
        return null;
    }
}
