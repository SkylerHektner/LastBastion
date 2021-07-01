using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SurvivalCardsUI : MonoBehaviour
{
    [SerializeField] List<UnlockFlagUIInformation> unlockFlagUIInformation;
    [SerializeField] List<SurvivalCardsUICard> Cards;
    [SerializeField] SurvivalCardsUIDescriptionBox DescriptionBox;

    private Dictionary<UnlockFlags, UnlockFlagUIInformation> ui_info_map = new Dictionary<UnlockFlags, UnlockFlagUIInformation>();
    private SurvivalCardsUICard current_selected_card;

    private void Start()
    {
#if UNITY_EDITOR
        foreach( UnlockFlags flag in Enum.GetValues( typeof( UnlockFlags ) ) )
        {
            Debug.Assert( unlockFlagUIInformation.Count( ui_info => ui_info.UnlockFlag == flag ) == 1,
                $"ERROR: Survival Cards UI missing unlock flag ui information for unlock flag {flag}" );
        }
#endif

        Debug.Assert( Cards.Count != 0 );
        Debug.Assert( DescriptionBox != null );
    }

    private void PopulateUIMap()
    {
        foreach( UnlockFlagUIInformation ui_info in unlockFlagUIInformation )
        {
            ui_info_map.Add( ui_info.UnlockFlag, ui_info );
        }
    }

    [ContextMenu( "ShowUpgrades" )]
    public void ShowUpgrades()
    {
        // my shame
        if( ui_info_map.Count == 0 )
        {
            PopulateUIMap();
        }

        List<UnlockFlags> unlock_flags = GenerateUnlockOptions();
        for( int x = 0; x < Cards.Count; ++x )
        {
            if( unlock_flags.Count > x )
            {
                Cards[x].Information = ui_info_map[unlock_flags[x]];
                Cards[x].UpdateIcon();
            }
            else
            {
                Cards[x].gameObject.SetActive( false );
            }
        }

        ResetCardGlows();
        gameObject.SetActive( true );
    }

    public void OnSurvivalCardClicked( SurvivalCardsUICard card )
    {
        current_selected_card = card;
        DescriptionBox.SetTextFromUIInfo( card.Information );
        ResetCardGlows();
        card.SetGlowColor( card.Information.GlowColor );
    }

    private void ResetCardGlows()
    {
        foreach(SurvivalCardsUICard card in Cards)
        {
            card.SetGlowColor( Color.clear );
        }
    }

    public void ConfirmUpgrade()
    {
        Debug.Assert( current_selected_card != null );
        PD.Instance.UnlockMap.Set( current_selected_card.Information.UnlockFlag, true, true );
        current_selected_card.ShowLockAnimation();

        // wave start was deferred for the menu, let it play now
        SpawnManager.Instance.StartNextWave();
    }

    public void SetInactive()
    {
        gameObject.SetActive( false );
    }

    // returns a list of up to three unlock flags randomly based on current unlock state
    private List<UnlockFlags> GenerateUnlockOptions()
    {
        List<UnlockFlags> ret = new List<UnlockFlags>();

        List<UnlockFlags> options = new List<UnlockFlags>();

        foreach( UnlockFlags flag in Enum.GetValues( typeof( UnlockFlags ) ) )
        {
            if( !PD.Instance.UnlockMap.Get( flag, GameplayManager.Instance.Survival )
                && PD.Instance.UnlockFlagDependencyMap[flag].All( f => PD.Instance.UnlockMap.Get( f, GameplayManager.Instance.Survival ) ) )
            {
                options.Add( flag );
            }
        }

        while( ret.Count < 6 && options.Count > 0 )
        {
            int random_index = UnityEngine.Random.Range( 0, options.Count );
            ret.Add( options[random_index] );
            options.RemoveAt( random_index );
        }

        return ret;
    }


}
