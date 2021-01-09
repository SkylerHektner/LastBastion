using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScroller : MonoBehaviour
{
    public RectTransform LevelBar;
    public float BarXPos;
    public float LevelBarLength;
    public float NumberofElements;
    public int JumpToLevel;
    float BarStartPos = 140;

    float DragBuffer;
    bool BarMoving;

    public Image DisplayImage;
    public GameObject LevelContainer;

    // Update is called once per frame
    void FixedUpdate()
    {
        LevelBarLength = LevelBar.GetComponent<RectTransform>().rect.width;
        NumberofElements = Mathf.Floor(LevelBarLength / 250f);
        BarXPos = LevelBar.transform.localPosition.x;

        if (DragBuffer > 0)
        {
            DragBuffer -= Time.smoothDeltaTime;
            if (DragBuffer <= 0)
            {
                DragBuffer = 0;
                RepositionBar();
            }
        }
    }


    // might have to remove this later on, as it may cause problems with returning from levels
    private void Awake()
    {
        JumpToLevel = 1;
        JumpToDesiredLevel();
    }

    [ContextMenu("JumpToDesiredLevel")]
    public void JumpToDesiredLevel()
    {
        if (JumpToLevel > 1)
        {
            LevelBar.transform.localPosition = new Vector2(-(BarStartPos + 250 * (JumpToLevel - 1)), LevelBar.transform.localPosition.y);
        }
        else
        {
            LevelBar.transform.localPosition = new Vector2(BarStartPos, LevelBar.transform.localPosition.y);
        }
        DisplayLevelImage(JumpToLevel);
    }

    public void DisplayLevelImage(int LevelIndex)
    {
        List<Sprite> LevelList = new List<Sprite>();
        foreach (Transform level in LevelContainer.transform)
        {
            LevelList.Add(level.GetComponent<LoadLevel>().LevelImage);
        }
        DisplayImage.sprite = LevelList[LevelIndex - 1];
    }

    public void SetDragBuffer()
    {
        DragBuffer = .2f;
    }

    public void RepositionBar()
    {
        for (int i = 1; i < NumberofElements + 1; i++)
        {
            float ReferencePoint = Mathf.Abs(BarStartPos) + 250 * (i - 1);
            if (Mathf.Abs(BarXPos) < ReferencePoint + 125 && Mathf.Abs(BarXPos) > ReferencePoint - 125) // falls within half of the distance to the next/previous element
            {
                LevelBar.transform.localPosition = new Vector2(-ReferencePoint, LevelBar.transform.localPosition.y);
                DisplayLevelImage(i);
                break;
            }
        }
    }
}
