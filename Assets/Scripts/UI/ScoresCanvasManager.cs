using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScoresCanvasManager : MonoBehaviour
{

    public float DistanceBetweenElements;
    public GameObject ArrowL;
    public GameObject ArrowR;
    public RectTransform LevelBar;
    public int NumberofElements;
    public int CurrentIndex;

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

}
