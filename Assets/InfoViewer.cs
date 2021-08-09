using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoViewer : MonoBehaviour
{

    bool isOpen;
    public GameObject PlayerInfo;
    public GameObject EnemyInfo;
    public GameObject AchievementInfo;
    public RectTransform PlayerInfoContainer;
    public RectTransform EnemyInfoContainer;
    public RectTransform AchievementInfoContainer;



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
    }

    public void DisplayEnemyInfo()
    {
        Index = 0;
        LevelBar = EnemyInfoContainer;
        PlayerInfo.SetActive(false);
        EnemyInfo.SetActive(true);
        AchievementInfo.SetActive(false);
        JumpToPosition(Index);

    }

    public void DisplayAchievementInfo()
    {
        Index = 0;
        LevelBar = AchievementInfoContainer;
        PlayerInfo.SetActive(false);
        EnemyInfo.SetActive(false);
        AchievementInfo.SetActive(true);
        JumpToPosition(Index);
    }

    public void HideAllDisplays()
    {
        Index = 0;
        LevelBar = null;
        PlayerInfo.SetActive(false);
        EnemyInfo.SetActive(false);
        AchievementInfo.SetActive(false);
        ArrowL.SetActive(false);
        ArrowR.SetActive(false);
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


    public void ShowItemDescription()
    {

    }

    public void JumpToPosition(int index)
    {
        if (index >= 1)
        {
            LevelBar.transform.localPosition = new Vector2(-(BarStartPos + DistanceBetweenElements * (index)), LevelBar.transform.localPosition.y);
        }
        else if (index == 0)
        {
            LevelBar.transform.localPosition = new Vector2(-(BarStartPos), LevelBar.transform.localPosition.y);
            ArrowR.SetActive(true);
        }
        if (index == 0)
        {
            ArrowL.SetActive(false);
        }
        else if (index >= InfoTabCategories.Count - 1)
        {
            ArrowR.SetActive(false);
        }
        else if (index > 0 && Index < InfoTabCategories.Count - 1)
        {
            ArrowL.SetActive(true);
            ArrowR.SetActive(true);
        }
    }
}
