using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class ScoresCanvasManager : MonoBehaviour
{

    public float DistanceBetweenElements;
    public GameObject ArrowL;
    public GameObject ArrowR;
    public RectTransform LevelBar; // the entries are stored in this parent object as a list of children
    public int NumberofElements;
    public int CurrentIndex;

    public GameObject ScoreEntryPrefab;
    GameObject ScoreEntryClone;
    public List<ScoreboardEntryData> ListOfEntries = new List<ScoreboardEntryData>();
    public Sprite DefaultProfilePic;


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
        if (CurrentIndex <= 0)
        {
            ArrowL.SetActive(false);
        }
        else if (CurrentIndex == NumberofElements - 1)
        {
            ArrowR.SetActive(false);
        }
        else if (CurrentIndex >= 0 && CurrentIndex < NumberofElements - 1)
        {
            ArrowL.SetActive(true);
            ArrowR.SetActive(true);
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
        for (int i = 0; i < ChildrenOrdered.Length; i++)
        {
            ChildrenOrdered[i].transform.SetSiblingIndex(i);
        }
        NumberofElements = ChildrenEntries.Length;
        ShowArrows();
    }

    [ContextMenu("TestEntries")]
    public void TestEntries()
    {
        CreateScoreEntries("pussySlayerxx420", Random.Range(1, 200), DefaultProfilePic);
    }
   

}
