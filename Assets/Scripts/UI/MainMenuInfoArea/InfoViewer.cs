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
    int Index;
    public RectTransform LevelBar;
    float BarStartPos = 140;
    public float BarXPos;
    public float LevelBarLength;
    public float NumberofElements;
    public float DistanceBetweenElements;
    public GameObject ArrowL;
    public GameObject ArrowR;

    private List<InfoItem> InfoItems = new List<InfoItem>();
    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemDescription;



    // Start is called before the first frame update
    void Awake()
    {
        Index = 0;
        ArrowL.SetActive(false);
        ArrowR.SetActive(false);
    }

    public void DisplayPlayerInfo()
    {
        Index = 0;
        LevelBar = PlayerInfoContainer;
        PlayerInfo.SetActive(true);
        EnemyInfo.SetActive(false);
        AchievementInfo.SetActive(false);
        JumpToPosition(Index);
        ShowArrowButtons();
        PercentageText.SetActive(false);
    }

    public void DisplayEnemyInfo()
    {
        Index = 0;
        LevelBar = EnemyInfoContainer;
        PlayerInfo.SetActive(false);
        EnemyInfo.SetActive(true);
        AchievementInfo.SetActive(false);
        JumpToPosition(Index);
        ShowArrowButtons();
        PercentageText.SetActive(false);

    }

    public void DisplayAchievementInfo()
    {
        Index = 0;
        LevelBar = AchievementInfoContainer;
        PlayerInfo.SetActive(false);
        EnemyInfo.SetActive(false);
        AchievementInfo.SetActive(true);
        JumpToPosition(Index);
        ShowArrowButtons();
        PercentageText.SetActive(true);
    }

    public void DisplayPremiumContent()
    {
        Index = 0;
        LevelBar = PremiumContentContainer;
        JumpToPosition(Index);
        ShowArrowButtons();
    }
    public void DisplayBonusContent()
    {
        Index = 0;
        LevelBar = BonusContentContainer;
        JumpToPosition(Index);
        ShowArrowButtons();
    }
    public void DisplayItemContent()
    {
        Index = 0;
        LevelBar = ItemsContainer;
        JumpToPosition(Index);
        ShowArrowButtons();
    }

    public void HideAllDisplays() // sets the enemy, player, and achievement info inactive and hides buttons (exit to main menu)
    {
        Index = 0;
        LevelBar = null;
        PlayerInfo.SetActive(false);
        EnemyInfo.SetActive(false);
        AchievementInfo.SetActive(false);
        ArrowL.SetActive(false);
        ArrowR.SetActive(false);
        PercentageText.SetActive(false);
    }

    public void ShiftConentRight()
    {
        Index += 1;
        JumpToPosition(Index);
    }

    public void ShiftContentLeft()
    {
        Index -= 1;
        JumpToPosition(Index);
    }

    public void ShowArrowButtons()
    {
        ArrowL.SetActive(true);
        ArrowR.SetActive(true);
    }


    public void ShowItemDescription(int ItemIndex) // displays the name and description of the chosen item in the list
    {
        Debug.Log(ItemIndex);
        InfoItems.Clear();
        foreach (Transform Item in LevelBar)
        {
            InfoItems.Add(Item.GetComponent<InfoItem>());
            Debug.Log(Item);
        }
        ItemName.text = InfoItems[ItemIndex].GetInfoName();
        ItemDescription.text = InfoItems[ItemIndex].GetInfoDescription();
        PercentageText.GetComponent<TextMeshProUGUI>().text = InfoItems[ItemIndex].GetProgressAmount();
    }

    public void JumpToPosition(int index) // jumps the bar to the next/previous spot in the line
    {
        if (index >= 1)
        {
            LevelBar.transform.localPosition = new Vector2(-(BarStartPos + DistanceBetweenElements * (index)), LevelBar.transform.localPosition.y);
        }
        else if (index == 0)
        {
            LevelBar.transform.localPosition = new Vector2(-(BarStartPos), LevelBar.transform.localPosition.y);
            ArrowR.GetComponent<Button>().interactable = true;
        }
        if (index == 0)
        {
            ArrowL.GetComponent<Button>().interactable = false;
        }
        else if (index >= InfoItems.Count - 1)
        {
            ArrowR.GetComponent<Button>().interactable = false;
        }
        else if (index > 0 && Index < InfoItems.Count - 1)
        {
            ArrowL.GetComponent<Button>().interactable = true;
            ArrowR.GetComponent<Button>().interactable = true;
        }
        ShowItemDescription(index);
    }
}
