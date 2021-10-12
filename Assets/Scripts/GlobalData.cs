using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THERE SHOULD ONLY BE ONE GLOBAL DATA STRUCTURE
[CreateAssetMenu( fileName = "Achievement", menuName = "ScriptableObjects/GlobalData", order = 0 )]
public class GlobalData : ScriptableObject
{
    public List<Achievement> Achievements;
    public List<Cosmetic> Cosmetics;


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
