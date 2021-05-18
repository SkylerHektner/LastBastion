using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SurvivalCardsUI : MonoBehaviour
{
    public List<UnlockFlagUIInformation> unlockFlagUIInformation;

    private Dictionary<PD.UnlockFlags, UnlockFlagUIInformation> ui_info_map = new Dictionary<PD.UnlockFlags, UnlockFlagUIInformation>();

    private void Start()
    {
#if UNITY_EDITOR
        foreach( PD.UnlockFlags flag in Enum.GetValues( typeof( PD.UnlockFlags ) ) )
        {
            Debug.Assert( unlockFlagUIInformation.Count( ui_info => ui_info.UnlockFlag == flag ) == 1,
                $"ERROR: Survival Cards UI missing unlock flag ui information for unlock flag {flag}" );
        }
#endif

        foreach(UnlockFlagUIInformation ui_info in unlockFlagUIInformation)
        {
            ui_info_map.Add( ui_info.UnlockFlag, ui_info );
        }
    }

    public void ShowUpgrades()
    {

    }

    public void ConfirmUpgrade()
    {

        // wave start was deferred for the menu, let it play now
        SpawnManager.Instance.StartNextWave();
    }

    public void SetInactive()
    {
        gameObject.SetActive( false );
    }

    // returns a list of up to three unlock flags randomly based on current unlock state
    private List<PD.UnlockFlags> GenerateUnlockOptions()
    {
        List<PD.UnlockFlags> ret = new List<PD.UnlockFlags>();

        List<PD.UnlockFlags> options = new List<PD.UnlockFlags>();

        foreach( PD.UnlockFlags flag in Enum.GetValues( typeof( PD.UnlockFlags ) ) )
        {
            if( !PD.Instance.UpgradeUnlockMap.GetUnlock( flag, GameplayManager.Instance.Survival )
                && PD.Instance.UpgradeFlagDependencyMap[flag].All( f => PD.Instance.UpgradeUnlockMap.GetUnlock( f, GameplayManager.Instance.Survival ) ) )
            {
                options.Add( flag );
            }
        }

        while( ret.Count < 3 && options.Count > 0 )
        {
            int random_index = UnityEngine.Random.Range( 0, options.Count );
            ret.Add( options[random_index] );
            options.RemoveAt( random_index );
        }

        return ret;
    }


}
