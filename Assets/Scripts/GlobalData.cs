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
            {
                Debug.Assert( Cosmetics.Count( cosmetic => cosmetic.unlock_flag == flag ) == 1,
                                $"ERROR: GlobalData Missing Entry for Cosmetic {flag}" );
            }
            else
            {
                Debug.Assert( UnlockFlagUIInfo.Count( ui_info => ui_info.UnlockFlag == flag ) == 1,
                                $"ERROR: GlobalData Missing Entry for unlock flag UI data for unlock flag {flag}" );
            }
        }
        foreach( Cosmetic cosmetic in Cosmetics )
        {
            if( cosmetic.Premium )
            {
                Debug.Assert( cosmetic.Price == 0.0f,
                    $"ERROR: Cosmetic {cosmetic.name} is premium but has a price! Premium cosmetics are all unlocked through the cosmetics dlc" );
            }
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
