using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class SurvivalCardsUI : MonoBehaviour
{
    [SerializeField] List<UnlockFlagUIInformation> unlockFlagUIInformation;
    [SerializeField] List<SurvivalCardsUICard> Cards;
    [SerializeField] SurvivalCardsUIDescriptionBox DescriptionBox;
    [SerializeField] GameObject PositiveBackgroundEffects;
    [SerializeField] GameObject NegativeBackgroundEffects;
    [SerializeField] Button ContinueButton;

    private SurvivalCardsUICard selected_curse_card;
    private SurvivalCardsUICard selected_boon_card;

    private Dictionary<UnlockFlags, UnlockFlagUIInformation> ui_info_map = new Dictionary<UnlockFlags, UnlockFlagUIInformation>();
    private int num_curse_cards;
    private int num_boon_cards;

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
        Debug.Assert( PositiveBackgroundEffects != null );
        Debug.Assert( NegativeBackgroundEffects != null );
        Debug.Assert( ContinueButton );
    }

    private void PopulateUIMap()
    {
        foreach( UnlockFlagUIInformation ui_info in unlockFlagUIInformation )
        {
            ui_info_map.Add( ui_info.UnlockFlag, ui_info );
        }
        num_curse_cards = Cards.Count( c => c.CurseCard );
        num_boon_cards = Cards.Count( c => !c.CurseCard );
    }

    [ContextMenu( "ShowUpgrades" )]
    public void ShowUpgrades()
    {
        // my shame
        if( ui_info_map.Count == 0 )
        {
            PopulateUIMap();
        }

        var boon_unlock_flags = GenerateUnlockOptions( false ).GetEnumerator();
        var curse_unlock_flags = GenerateUnlockOptions( true ).GetEnumerator();
        foreach( SurvivalCardsUICard card in Cards )
        {
            var unlock_flags_enumerator = card.CurseCard ? curse_unlock_flags : boon_unlock_flags;
            if( unlock_flags_enumerator.MoveNext() )
            {
                card.Information = ui_info_map[unlock_flags_enumerator.Current];
                card.UpdateIcon();
            }
            else
            {
                card.gameObject.SetActive( false );
            }
            curse_unlock_flags = card.CurseCard ? unlock_flags_enumerator : curse_unlock_flags;
            boon_unlock_flags = !card.CurseCard ? unlock_flags_enumerator : boon_unlock_flags;
        }

        ResetCardGlowsAndBackground( true );
        ResetCardGlowsAndBackground( false );
        ContinueButton.gameObject.SetActive( false );
        gameObject.SetActive( true );
    }

    public void OnSurvivalCardClicked( SurvivalCardsUICard card )
    {
        if( card.CurseCard )
            selected_curse_card = card;
        else
            selected_boon_card = card;

        DescriptionBox.SetTextFromUIInfo( card.Information );
        ResetCardGlowsAndBackground( card.CurseCard );
        card.SetGlowColor( card.Information.GlowColor );
        ( card.CurseCard ? NegativeBackgroundEffects : PositiveBackgroundEffects ).SetActive( true );
        TryEnableContinueButton();
    }

    private void TryEnableContinueButton()
    {
        if( selected_boon_card != null && selected_curse_card != null )
            ContinueButton.gameObject.SetActive( true );
    }

    private void ResetCardGlowsAndBackground( bool curse_cards )
    {
        foreach( SurvivalCardsUICard card in Cards )
        {
            if( ( curse_cards && card.CurseCard ) || ( !curse_cards && !card.CurseCard ) )
            {
                card.SetGlowColor( Color.clear );
            }
        }
        PositiveBackgroundEffects.SetActive( false );
        NegativeBackgroundEffects.SetActive( false );
    }

    public void ConfirmOptions()
    {
        Debug.Assert( selected_boon_card != null );
        Debug.Assert( selected_curse_card != null );

        PD.Instance.UnlockMap.Set( selected_boon_card.Information.UnlockFlag, true, true );
        PD.Instance.UnlockMap.Set( selected_curse_card.Information.UnlockFlag, true, true );

        // wave start was deferred for the menu, let it play now
        SpawnManager.Instance.StartNextWave();
    }

    public void SetInactive()
    {
        gameObject.SetActive( false );
    }

    // returns a list of up to three unlock flags randomly based on current unlock state
    private List<UnlockFlags> GenerateUnlockOptions( bool curse_unlocks )
    {
        List<UnlockFlags> ret = new List<UnlockFlags>();

        List<UnlockFlags> options = new List<UnlockFlags>();

        foreach( UnlockFlags flag in Enum.GetValues( typeof( UnlockFlags ) ) )
        {
            if( !PD.Instance.UnlockMap.Get( flag, GameplayManager.Instance.Survival )
                && PD.Instance.UnlockFlagDependencyMap[flag].All( f => PD.Instance.UnlockMap.Get( f, GameplayManager.Instance.Survival ) )
                && ( curse_unlocks ? PD.Instance.UnlockFlagCurseMap[flag] : !PD.Instance.UnlockFlagCurseMap[flag] ) )
            {
                options.Add( flag );
            }
        }

        while( ret.Count < ( curse_unlocks ? num_curse_cards : num_boon_cards ) && options.Count > 0 )
        {
            int random_index = UnityEngine.Random.Range( 0, options.Count );
            ret.Add( options[random_index] );
            options.RemoveAt( random_index );
        }

        return ret;
    }
}
