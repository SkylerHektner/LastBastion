using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class SurvivalCardsUI : MonoBehaviour
{
    [SerializeField] List<SurvivalCardsUICard> Cards;
    [SerializeField] SurvivalCardsUIDescriptionBox DescriptionBox;
    [SerializeField] GameObject PositiveBackgroundEffects;
    [SerializeField] GameObject NegativeBackgroundEffects;
    [SerializeField] Button ContinueButton;
    [SerializeField] Animator Anim;

    private SurvivalCardsUICard selected_curse_card;
    private SurvivalCardsUICard selected_boon_card;

    private Dictionary<UnlockFlag, UnlockFlagUIInformation> ui_info_map = new Dictionary<UnlockFlag, UnlockFlagUIInformation>();
    private int num_curse_cards;
    private int num_boon_cards;
    public Button StartingSelection;

    private void Start()
    {
        Debug.Assert( Cards.Count != 0 );
        Debug.Assert( DescriptionBox != null );
        Debug.Assert( PositiveBackgroundEffects != null );
        Debug.Assert( NegativeBackgroundEffects != null );
        Debug.Assert( ContinueButton != null );
        Debug.Assert( Anim != null );
    }

    private void PopulateUIMap()
    {
        foreach( UnlockFlagUIInformation ui_info in Spectator.Instance.GD.UnlockFlagUIInfo )
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

        GameplayManager.State = GameplayManager.GameState.ChoosingUpgrade;

        var boon_unlock_flags = GenerateUnlockOptions( false );
        var curse_unlock_flags = GenerateUnlockOptions( true );
        var boon_unlock_flags_iterator = boon_unlock_flags.GetEnumerator();
        var curse_unlock_flags_iterator = curse_unlock_flags.GetEnumerator();
        foreach( SurvivalCardsUICard card in Cards )
        {
            var unlock_flags_enumerator = card.CurseCard ? curse_unlock_flags_iterator : boon_unlock_flags_iterator;
            if( unlock_flags_enumerator.MoveNext() )
            {
                card.Information = ui_info_map[unlock_flags_enumerator.Current];
                card.UpdateIcon();
            }
            else
            {
                card.gameObject.SetActive( false );
            }
            curse_unlock_flags_iterator = card.CurseCard ? unlock_flags_enumerator : curse_unlock_flags_iterator;
            boon_unlock_flags_iterator = !card.CurseCard ? unlock_flags_enumerator : boon_unlock_flags_iterator;
        }

        ResetCardGlowsAndBackground( true );
        ResetCardGlowsAndBackground( false );
        ContinueButton.gameObject.SetActive( false );
        StartingSelection.Select();
        gameObject.SetActive( true );
        Anim.SetBool( "Empty", boon_unlock_flags.Count == 0 && curse_unlock_flags.Count == 0 );
        TryEnableContinueButton();
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
        if( ( selected_boon_card != null || !Cards.Where( c => !c.CurseCard ).Any( c => c.gameObject.activeInHierarchy ) )
            && ( selected_curse_card != null || !Cards.Where( c => c.CurseCard ).Any( c => c.gameObject.activeInHierarchy ) ) )
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
        if( selected_boon_card != null )
            PD.Instance.UnlockMap.Set( selected_boon_card.Information.UnlockFlag, true, true );
        if( selected_curse_card != null )
            PD.Instance.UnlockMap.Set( selected_curse_card.Information.UnlockFlag, true, true );

        selected_boon_card?.ShowLockAnimation();
        selected_curse_card?.ShowLockAnimation();

        selected_boon_card = null;
        selected_curse_card = null;

        // call here to hide the transition while the screen is blacked out
        InfiniteSpawnCadenceManager.Instance.PickNewSpawnCadenceProfile();
    }

    public void SetInactive()
    {
        gameObject.SetActive( false );
        ContinueButton.gameObject.SetActive( false );

        // start the wave when we disable this menu
        SpawnManager.Instance.StartNextWave();
    }

    // returns a list of up to three unlock flags randomly based on current unlock state
    private List<UnlockFlag> GenerateUnlockOptions( bool curse_unlocks )
    {
        List<UnlockFlag> ret = new List<UnlockFlag>();

        List<UnlockFlag> options = new List<UnlockFlag>();

        foreach( UnlockFlag flag in Enum.GetValues( typeof( UnlockFlag ) ) )
        {
            if( PD.Instance.UnlockFlagCategoryMap[flag] == UnlockFlagCategory.Cosmetic )
                continue;

            if( !PD.Instance.UnlockMap.Get( flag, GameplayManager.Instance.Survival )
                && PD.Instance.UnlockFlagDependencyMap[flag].All( f => PD.Instance.UnlockMap.Get( f, true ) )
                && ( curse_unlocks ? ( PD.Instance.UnlockFlagCategoryMap[flag] == UnlockFlagCategory.Curse ) : ( PD.Instance.UnlockFlagCategoryMap[flag] == UnlockFlagCategory.Upgrade ) ) )
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
