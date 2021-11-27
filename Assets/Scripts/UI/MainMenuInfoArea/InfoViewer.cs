using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoViewer : MonoBehaviour
{

    bool isOpen;
    public GameObject PlayerInfo;
    public GameObject EnemyInfo;
    public GameObject AchievementInfo;
    public RectTransform PlayerInfoContainer;
    public RectTransform EnemyInfoContainer;
    public RectTransform AchievementInfoContainer;
    public RectTransform PremiumContentContainer;
    public RectTransform BonusContentContainer;
    public RectTransform ItemsContainer;

    public GameObject PercentageText;

    public List<GameObject> InfoTabCategories;
    [ReadOnly] public int Index;
    public RectTransform LevelBar;
    float BarStartPos = 140;
    public float BarXPos;
    public float LevelBarLength;
    public float NumberofElements;
    public float DistanceBetweenElements;
    public GameObject ArrowL;
    public GameObject ArrowR;

    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemDescription;
    public bool WindowShopping;
    public Animator SoldOutIcon;
    public Button ClaimButton;
    public GameObject PriceDisplay;

    // Start is called before the first frame update
    void Awake()
    {
        Index = 0;
        ArrowL.SetActive( false );
        ArrowR.SetActive( false );
    }

    public void DisplayPlayerInfo()
    {
        Index = 0;
        LevelBar = PlayerInfoContainer;
        PlayerInfo.SetActive( true );
        EnemyInfo.SetActive( false );
        AchievementInfo.SetActive( false );
        JumpToPosition( Index );
        ShowArrowButtons();
        PercentageText.SetActive( false );
    }

    public void DisplayEnemyInfo()
    {
        Index = 0;
        LevelBar = EnemyInfoContainer;
        PlayerInfo.SetActive( false );
        EnemyInfo.SetActive( true );
        AchievementInfo.SetActive( false );
        JumpToPosition( Index );
        ShowArrowButtons();
        PercentageText.SetActive( false );

    }

    public void DisplayAchievementInfo()
    {
        Index = 0;
        LevelBar = AchievementInfoContainer;
        PlayerInfo.SetActive( false );
        EnemyInfo.SetActive( false );
        AchievementInfo.SetActive( true );
        JumpToPosition( Index );
        ShowArrowButtons();
        PercentageText.SetActive( true );
    }

    public void DisplayPremiumContent()
    {
        Index = 0;
        LevelBar = PremiumContentContainer;
        JumpToPosition( Index );
        ShowArrowButtons();
    }
    public void DisplayBonusContent()
    {
        Index = 0;
        LevelBar = BonusContentContainer;
        JumpToPosition( Index );
        ShowArrowButtons();
    }
    public void DisplayItemContent()
    {
        Index = 0;
        LevelBar = ItemsContainer;
        JumpToPosition( Index );
        ShowArrowButtons();
    }

    public void HideAllDisplays() // sets the enemy, player, and achievement info inactive and hides buttons (exit to main menu)
    {
        Index = 0;
        LevelBar = null;
        PlayerInfo.SetActive( false );
        EnemyInfo.SetActive( false );
        AchievementInfo.SetActive( false );
        ArrowL.SetActive( false );
        ArrowR.SetActive( false );
        PercentageText.SetActive( false );
    }

    public void ShiftConentRight()
    {
        Index += 1;
        JumpToPosition( Index );
    }

    public void ShiftContentLeft()
    {
        Index -= 1;
        JumpToPosition( Index );
    }

    public void ShowArrowButtons()
    {
        ArrowL.SetActive( true );
        ArrowR.SetActive( true );
    }

    public void ShowItemDescription( int ItemIndex ) // displays the name and description of the chosen item in the list
    {
        Debug.Log( ItemIndex );
        if( WindowShopping )
        {
            StoreItem store_item = LevelBar.GetChild( ItemIndex )?.GetComponent<StoreItem>();
            if( store_item != null )
            {
                ItemName.text = store_item.GetInfoPrice();
                ItemDescription.text = store_item.GetInfoDescription();
                if (store_item.ItemPurchased) // display the universal purchased symbol
                {
                    SoldOutIcon.SetBool("Sold", true);
                    ClaimButton.interactable = false;
                    PriceDisplay.SetActive(false);
                }
                else// hide the universal purchased symbol
                {
                    SoldOutIcon.SetBool("Sold", false);
                    ClaimButton.interactable = true;
                    PriceDisplay.SetActive(true);
                }
            }
            else
            {
                Debug.LogError( "ERROR: Invalid Index or missing component in InfoViewer.ShowItemDescription", this );
            }
        }
        else
        {
            InfoItem info_item = LevelBar.GetChild( ItemIndex )?.GetComponent<InfoItem>();
            if( info_item != null )
            {
                ItemName.text = info_item.GetInfoName();
                ItemDescription.text = info_item.GetInfoDescription();
                PercentageText.GetComponent<TextMeshProUGUI>().text = info_item.GetProgressAmount();
            }
            else
            {
                Debug.LogError( "ERROR: Invalid Index or missing component in InfoViewer.ShowItemDescription", this );
            }
        }
    }

    public void JumpToPosition( int index ) // jumps the bar to the next/previous spot in the line
    {
        if( index >= 1 )
        {
            LevelBar.transform.localPosition = new Vector2( -( BarStartPos + DistanceBetweenElements * ( index ) ), LevelBar.transform.localPosition.y );
        }
        else if( index == 0 )
        {
            LevelBar.transform.localPosition = new Vector2( -( BarStartPos ), LevelBar.transform.localPosition.y );
            ArrowR.GetComponent<Button>().interactable = true;
        }
        if( index == 0 )
        {
            ArrowL.GetComponent<Button>().interactable = false;
        }
        else if( index >= LevelBar.childCount - 1 )
        {
            ArrowR.GetComponent<Button>().interactable = false;
        }
        else if( index > 0 && index < LevelBar.childCount - 1 )
        {
            ArrowL.GetComponent<Button>().interactable = true;
            ArrowR.GetComponent<Button>().interactable = true;
        }
        ShowItemDescription( index );
    }

    public GameObject GetItemAtCurrentIndex()
    {
        return LevelBar.GetChild( Index ).gameObject;
    }

    public void RefreshStoreItem()
    {
        if (WindowShopping)
        {
            ShowItemDescription(Index);
        }
    }
}
