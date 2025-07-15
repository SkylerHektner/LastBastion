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
    ////public Animator Door;

    public Image GlowFX;
    public Image PortalRim;
    public Color GlowColor;

    bool Dragging;
    public float DistanceBetweenElements;
    public List<GameObject> LevelList;
    public GameObject ArrowL;
    public GameObject ArrowR;

    // Update is called once per frame
    void FixedUpdate()
    {
        LevelBarLength = LevelBar.GetComponent<RectTransform>().rect.width;
        NumberofElements = Mathf.Floor(LevelBarLength / DistanceBetweenElements);
        BarXPos = LevelBar.transform.localPosition.x;

        //if (DragBuffer > 0)
        //{
        //    DragBuffer -= Time.smoothDeltaTime;
        //    if (DragBuffer <= 0)
        //    {
        //        DragBuffer = 0;
        //        RepositionBar();
        //    }
        //}
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

        if (Spectator.ReturningFromLevel == true)
        {
            ////Door.SetTrigger("Shortcut");
            LevelIndex = Spectator.LevelIndex;

            if (LevelIndex <= 1)
            {
                LevelIndex = 1;
            }
            JumpToDesiredLevel(LevelIndex);
            Portal.SetTrigger("Shrink");
            Spectator.ReturningFromLevel = false;
            UpdateAnimators(LevelIndex);

#if UNITY_EDITOR
            Debug.Log("Returning from level" + Spectator.LevelIndex);

#endif
        }
        else
        {
            if (LevelIndex <= 1)
            {
                LevelIndex = 1;
            }
            JumpToDesiredLevel(LevelIndex);
            UpdateAnimators(LevelIndex);

#if UNITY_EDITOR
            Debug.Log("Not returning from a level");
#endif
        }
#if UNITY_EDITOR
        Debug.Log("Level index" + LevelIndex);
#endif
    }

    //[ContextMenu("JumpToDesiredLevel")]
    public void JumpToDesiredLevel(int Index)
    {
        if (Index > 0)
        {
            LevelBar.transform.localPosition = new Vector2(-(BarStartPos + DistanceBetweenElements * (Index - 1)), LevelBar.transform.localPosition.y);
        }
        if (Index <= 1)
        {
            ArrowL.GetComponent<Button>().enabled = false;
        }
        else if (Index >= LevelList.Count)
        {
            ArrowR.GetComponent<Button>().enabled = false;
        }
        else if (Index >= 1 && Index < LevelList.Count)
        {
            ArrowL.GetComponent<Button>().enabled = true;
            ArrowR.GetComponent<Button>().enabled = true;
        }
        DisplayLevelImage(Index);
        UpdateAnimators(Index);
    }

    public void UpdateAnimators(int Index)
    {
        Index = Index - 1;
        for (int i = 0; i < LevelList.Count; i++)
        {
            if (i == Index)
            {
                LevelList[i].GetComponent<Animator>().SetBool("Active", true);
                LevelList[i].GetComponent<Button>().interactable = true;
            }
            else
            {
                LevelList[i].GetComponent<Animator>().SetBool("Active", false);
                LevelList[i].GetComponent<Button>().interactable = false;

            }
        }
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
#if UNITY_EDITOR
            Debug.LogWarning( "Warning: Level Index Greater than number of load level buttons that are registered" );
#endif
            return;
        }
        if (LevelIndex <= 0)
        {
            LevelIndex = 1;
        }
        DisplayImage.sprite = load_levels[LevelIndex - 1].LevelImage;
        SwapGlow(LevelIndex - 1);
    }

    public void SwapGlow(int Index)
    {
        GlowColor.r = load_levels[Index].GlowRGB.x;
        GlowColor.g = load_levels[Index].GlowRGB.y;
        GlowColor.b = load_levels[Index].GlowRGB.z;
        GlowColor.a = load_levels[Index].GlowRGB.w;
        GlowFX.color = GlowColor;
        PortalRim.color = GlowColor;
    }

    public void SetDragBuffer()
    {
        if (Dragging == false)
        {
            DragBuffer = .05f;
        }
    }

    public void ShiftNextLevel()
    {
        LevelIndex = LevelIndex + 1;

        if (LevelIndex == LevelList.Count + 1)
        {
            LevelIndex = 1;
        }
        JumpToDesiredLevel(LevelIndex);

        Debug.Log(LevelIndex);

    }
    public void ShiftPreviousLevel()
    {
        LevelIndex = LevelIndex - 1;
        if (LevelIndex == 0)
        {
            LevelIndex = LevelList.Count;
        }
        JumpToDesiredLevel(LevelIndex);
        Debug.Log(LevelIndex);
    }


    public void RepositionBar()
    {
        for (int i = 1; i < NumberofElements + 1; i++)
        {
            float ReferencePoint = Mathf.Abs(BarStartPos) + DistanceBetweenElements * (i - 1);
            if (Mathf.Abs(BarXPos) < ReferencePoint + DistanceBetweenElements/2 && Mathf.Abs(BarXPos) > ReferencePoint - DistanceBetweenElements/2) // falls within half of the distance to the next/previous element
            {
                LevelBar.transform.localPosition = new Vector2(-ReferencePoint, LevelBar.transform.localPosition.y);
                DisplayLevelImage(i);
                LevelIndex = i;
                if (LevelIndex <= 1)
                {
                    LevelIndex = 1;
                }
#if UNITY_EDITOR
                Debug.Log("This is level " + LevelIndex + " selected");
#endif
                Spectator.LevelIndex = LevelIndex;
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
