using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;
using System;

public class ScoresCanvasManager : MonoBehaviour
{

    public float DistanceBetweenElements;
    public GameObject ArrowL;
    public GameObject ArrowR;
    public RectTransform LevelBar; // the entries are stored in this parent object as a list of children
    public int NumberofElements;
    public int CurrentIndex;
    public List<Sprite> RankIcons;

    public GameObject ScoreEntryPrefab;
    GameObject ScoreEntryClone;
    public Sprite DefaultProfilePic;
    public Button HomeButton;


    private void Awake()
    {
        ShowArrows();
    }

    public void ShiftNextLevel()
    {
        CurrentIndex = CurrentIndex + 1;
        if (CurrentIndex < NumberofElements)
        {
            LevelBar.localPosition = new Vector2(LevelBar.localPosition.x - DistanceBetweenElements, LevelBar.localPosition.y);
        }
        else
        {
            CurrentIndex = NumberofElements - 1;
        }
        ShowArrows();
        Debug.Log(CurrentIndex);

    }
    public void ShiftPreviousLevel()
    {
        CurrentIndex = CurrentIndex - 1;
        LevelBar.localPosition = new Vector2(LevelBar.localPosition.x + DistanceBetweenElements, LevelBar.localPosition.y);

        if (CurrentIndex <= 0)
        {
            CurrentIndex = 0;
        }
        ShowArrows();
        Debug.Log(CurrentIndex);
    }

    public void ShowArrows()
    {
        if (NumberofElements == 0)
        {
            ArrowL.SetActive(false);
            ArrowR.SetActive(false);
            Navigation HomeNav = HomeButton.navigation;
            HomeNav.selectOnRight = null;
            HomeNav.selectOnDown = null;
            HomeButton.navigation = HomeNav;
        }
        if (CurrentIndex <= 0)
        {
            ArrowL.SetActive(false);
            ArrowR.GetComponent<Button>().Select();
            Navigation RightArrowNav = ArrowR.GetComponent<Button>().navigation; // with missing buttons, set nav explicitely
            RightArrowNav.selectOnLeft = HomeButton;
            RightArrowNav.selectOnUp = HomeButton;
            ArrowR.GetComponent<Button>().navigation = RightArrowNav;
        }
        else if (CurrentIndex == NumberofElements - 1)
        {
            ArrowR.SetActive(false);
            ArrowL.GetComponent<Button>().Select();
            Navigation LeftArrowNav = ArrowL.GetComponent<Button>().navigation; // right arrow is missing, redirect button nav
            LeftArrowNav.selectOnLeft = HomeButton;
            LeftArrowNav.selectOnUp = HomeButton;
            LeftArrowNav.selectOnRight = null;
            ArrowL.GetComponent<Button>().navigation = LeftArrowNav;
        }
        else if (CurrentIndex >= 0 && CurrentIndex < NumberofElements - 1)
        {
            ArrowL.SetActive(true);
            ArrowR.SetActive(true);
            Navigation HomeNav = HomeButton.navigation;
            HomeNav.selectOnRight = ArrowL.GetComponent<Button>();
            HomeNav.selectOnDown = ArrowL.GetComponent<Button>();
            HomeButton.navigation = HomeNav;

            // and set arrow nav too for right arrow
            Navigation RightArrowNav = ArrowR.GetComponent<Button>().navigation;
            RightArrowNav.selectOnLeft = ArrowL.GetComponent<Button>();
            RightArrowNav.selectOnUp = HomeButton;
            ArrowR.GetComponent<Button>().navigation = RightArrowNav;

            // and left arrow
            Navigation LeftArrowNav = ArrowL.GetComponent<Button>().navigation; // right arrow is missing, redirect button nav
            LeftArrowNav.selectOnLeft = HomeButton;
            LeftArrowNav.selectOnUp = HomeButton;
            LeftArrowNav.selectOnRight = ArrowR.GetComponent<Button>();
            ArrowL.GetComponent<Button>().navigation = LeftArrowNav;


        }
        if (CurrentIndex <= 0 && NumberofElements >= 2)
        {
            ArrowR.SetActive(true);
            Navigation HomeNav = HomeButton.navigation;
            HomeNav.selectOnRight = ArrowR.GetComponent<Button>();
            HomeNav.selectOnDown = ArrowR.GetComponent<Button>();
            HomeButton.navigation = HomeNav;

        }
        if (CurrentIndex == 1 && NumberofElements == 2)
        {
            ArrowL.SetActive(true);
            Navigation HomeNav = HomeButton.navigation;
            HomeNav.selectOnRight = ArrowL.GetComponent<Button>();
            HomeNav.selectOnDown = ArrowL.GetComponent<Button>();
            HomeButton.navigation = HomeNav;
        }
    }


    // used to populate scoreboard prefab entries
    public void CreateScoreEntries(string PlayerNameString, int PlayerBestWaveScore, Sprite PlayerPicture)
    {
        ScoreEntryClone = Instantiate(ScoreEntryPrefab, transform.position, Quaternion.identity);
        ScoreEntryClone.GetComponent<ScoreboardEntryData>().SetMyInfo(PlayerNameString, PlayerBestWaveScore, PlayerPicture);
        ScoreEntryClone.transform.parent = LevelBar;
        ScoreEntryClone.transform.localScale = new Vector3(1, 1, 1);
        ScoreboardEntryData[] ChildrenEntries = GetComponentsInChildren<ScoreboardEntryData>();
        ScoreboardEntryData[] ChildrenOrdered = ChildrenEntries.OrderBy(go => go.HighestWave).ToArray();
        Array.Reverse(ChildrenOrdered);
        for (int i = 0; i < ChildrenOrdered.Length; i++)
        {
            ChildrenOrdered[i].transform.SetSiblingIndex(i);
            ChildrenOrdered[i].CurrentRanking = i;
            ChildrenOrdered[i].RankingBubble.sprite = RankIcons[i];
        }
        NumberofElements = ChildrenEntries.Length;
        ShowArrows();
    }

    [ContextMenu("TestEntries")]
    public void TestEntries()
    {
        CreateScoreEntries("pussySlayerxx420", UnityEngine.Random.Range(1, 200), DefaultProfilePic);
    }
   

}
