using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScroller : MonoBehaviour
{
    public RectTransform LevelBar;
    public float BarXPos;
    public float LevelBarLength;
    public float NumberofElements;
    public int JumpToLevel;
    float BarStartPos = 140;

    // Update is called once per frame
    void FixedUpdate()
    {
        LevelBarLength = LevelBar.GetComponent<RectTransform>().rect.width;
        NumberofElements = Mathf.Floor(LevelBarLength / 250f);
        BarXPos = LevelBar.transform.localPosition.x;

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpToDesiredLevel();
        }
    }

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
    }
}
