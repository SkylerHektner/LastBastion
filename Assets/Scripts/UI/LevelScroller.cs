using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelScroller : MonoBehaviour, IBeginDragHandler, IEndDragHandler
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
    private List<LoadLevel> load_levels = new List<LoadLevel>();

    public static int LevelIndex;

    public Animator Portal;
    public Animator Door;

    public Image GlowFX;
    public Image PortalRim;
    public Color GlowColor;

    bool Dragging;

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
        load_levels.Clear();
        foreach( Transform level in LevelContainer.transform )
        {
            load_levels.Add( level.GetComponent<LoadLevel>() );
        }
        foreach( LoadLevel level in load_levels )
        {
            level.LockStatusChangedEvent.AddListener( OnLockedLevelImageChanged );
        }

        if (LevelIndex <= 1)
        {
            LevelIndex = 1;
        }
        if (Spectator.ReturningFromLevel == true)
        {
            Door.SetTrigger("Shortcut");
            LevelIndex = Spectator.LevelIndex;
            JumpToDesiredLevel(LevelIndex);
            Portal.SetTrigger("Shrink");
            Spectator.ReturningFromLevel = false;
            Debug.Log("Returning from level" + Spectator.LevelIndex);
        }
        else
        {
            LevelIndex = 1;
            JumpToDesiredLevel(LevelIndex);
        }
        Debug.Log("Level index" + LevelIndex);
    }

    //[ContextMenu("JumpToDesiredLevel")]
    public void JumpToDesiredLevel(int Index)
    {
        if (Index > 1)
        {
            LevelBar.transform.localPosition = new Vector2(-(BarStartPos + 250 * (Index - 1)), LevelBar.transform.localPosition.y);
        }
        else
        {
            LevelBar.transform.localPosition = new Vector2(BarStartPos, LevelBar.transform.localPosition.y);
        }
        DisplayLevelImage(Index);
    }

    private void OnLockedLevelImageChanged(int level_index)
    {
        if( level_index == LevelIndex )
            DisplayLevelImage( level_index );
    }

    public void DisplayLevelImage(int LevelIndex)
    {
        if(LevelIndex > load_levels.Count)
        {
            Debug.LogWarning( "Warning: Level Index Greater than number of load level buttons that are registered" );
            return;
        }
        DisplayImage.sprite = load_levels[LevelIndex - 1].LevelImage;
        SwapGlow(LevelIndex - 1);
        //Debug.Log("Displaying level  " + LevelIndex + "Image");
    }

    public void SwapGlow(int Index)
    {
        GlowColor.r = load_levels[Index].GlowRGB.x;
        GlowColor.g = load_levels[Index].GlowRGB.y;
        GlowColor.b = load_levels[Index].GlowRGB.z;
        GlowColor.a = load_levels[Index].GlowRGB.w;
        GlowFX.color = GlowColor;
        PortalRim.color = GlowColor;
        //Debug.Log("R   "+ GlowColor.r + " G   " + GlowColor.g + "B    " + GlowColor.b);
    }

    public void SetDragBuffer()
    {
        if (Dragging == false)
        {
            DragBuffer = .05f;
        }
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
                LevelIndex = i;
                if (LevelIndex <= 1)
                {
                    LevelIndex = 1;
                }
                break;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Dragging = false;
        SetDragBuffer();
        gameObject.GetComponent<ScrollRect>().velocity = Vector2.zero;
    }
}
