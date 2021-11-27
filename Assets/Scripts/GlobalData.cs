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
    public List<CosmeticBundle> CosmeticBundles;
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
            if( cosmetic.Premium && !cosmetic.BelongsToBundle )
            {
                Debug.Assert( !String.IsNullOrEmpty( cosmetic.ProductID ),
                    $"ERROR: Cosmetic {cosmetic.name} is premium but does not have a valid product ID or belong to a bundle" );

                Debug.Assert( cosmetic.Price != 0.0f,
                    $"ERROR: Cosmetic {cosmetic.name} is premium but does not have a valid price or belong to a bundle" );
            }
            else if( cosmetic.BelongsToBundle )
            {
                var bundle = CosmeticBundles.Where( b => b.Cosmetics.Contains( cosmetic ) )?.Single();
                Debug.Assert( bundle != null,
                    $"ERROR: Cosmetic {cosmetic.name} does not belong to a valid bundle" );
            }
        }
        foreach( CosmeticBundle bundle in CosmeticBundles )
        {
            if( bundle.Premium )
            {
                Debug.Assert( !String.IsNullOrEmpty( bundle.ProductID ),
                    $"ERROR: Cosmetic Bundle {bundle.name} is premium but does not have a valid product ID or belong to a bundle with a valid product ID" );

                Debug.Assert( bundle.Price != 0.0f,
                    $"ERROR: Cosmetic Bundle {bundle.name} is premium but does not have a valid price or belong to a bundle with a valid price" );
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
