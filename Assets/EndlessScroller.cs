using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EndlessScroller : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public RectTransform LevelBar;
    public float BarXPos;
    public float LevelBarLength;
    public float NumberofElements;
    public int JumpToLevel;
    float BarStartPos = 140;

    float DragBuffer;
    bool BarMoving;

    //public Image DisplayImage;
    public GameObject LevelContainer;
    private List<SurvivalDoor> load_levels = new List<SurvivalDoor>();

    public static int EndlessLevelIndex;

    ///public Animator Portal;
    //public Animator Door;

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
        foreach (Transform level in LevelContainer.transform)
        {
            load_levels.Add(level.GetComponent<SurvivalDoor>());
        }
        foreach (SurvivalDoor level in load_levels)
        {
            //level.LockStatusChangedEvent.AddListener(OnLockedLevelImageChanged);
        }

        if (EndlessLevelIndex <= 1)
        {
            EndlessLevelIndex = 1;
        }
        if (Spectator.ReturningFromLevel == true)
        {
            //Door.SetTrigger("Shortcut");
            //EndlessLevelIndex = Spectator.LevelIndex;
            JumpToDesiredLevel(EndlessLevelIndex);
            Spectator.ReturningFromLevel = false;
#if UNITY_EDITOR
            Debug.Log("Returning from endless" + Spectator.LevelIndex);
#endif
        }
        else
        {
            JumpToDesiredLevel(EndlessLevelIndex);
            //LevelIndex = PD.Instance.StoredLimboLevelIndex.Get();
            //JumpToDesiredLevel(PD.Instance.StoredLimboLevelIndex.Get());
#if UNITY_EDITOR
            Debug.Log("Not returning from endless");
#endif
        }
        UpdateAnimators(EndlessLevelIndex);
#if UNITY_EDITOR
        Debug.Log("Endless index" + EndlessLevelIndex);
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
            ArrowL.SetActive(false);
        }
        else if (Index == NumberofElements)
        {
            ArrowR.SetActive(false);
        }
        else if (Index >= 1 && Index < NumberofElements)
        {
            ArrowL.SetActive(true);
            ArrowR.SetActive(true);
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
                LevelList[i].GetComponentInChildren<SurvivalDoor>().OpenDoor();
                LevelList[i].GetComponentInChildren<SurvivalDoor>().MyButton.interactable = true;
            }
            else
            {
                LevelList[i].GetComponentInChildren<SurvivalDoor>().CloseDoor();
                LevelList[i].GetComponentInChildren<SurvivalDoor>().MyButton.interactable = false;

            }
        }
    }


    private void OnLockedLevelImageChanged(int level_index)
    {
        if (level_index == EndlessLevelIndex)
            DisplayLevelImage(level_index);
    }

    public void DisplayLevelImage(int LevelIndex)
    {
        if (LevelIndex > load_levels.Count)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Warning: Level Index Greater than number of load level buttons that are registered");
#endif
            return;
        }
        if (LevelIndex <= 0)
        {
            LevelIndex = 1;
        }
        //DisplayImage.sprite = load_levels[LevelIndex - 1].LevelImage;
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
        if (EndlessLevelIndex < NumberofElements)
        {
            EndlessLevelIndex = EndlessLevelIndex + 1;
            JumpToDesiredLevel(EndlessLevelIndex);
        }
        Debug.Log(EndlessLevelIndex);

    }
    public void ShiftPreviousLevel()
    {
        EndlessLevelIndex = EndlessLevelIndex - 1;
        if (EndlessLevelIndex < 0)
        {
            EndlessLevelIndex = 0;
        }
        JumpToDesiredLevel(EndlessLevelIndex);
        Debug.Log(EndlessLevelIndex);
    }


    public void RepositionBar()
    {
        for (int i = 1; i < NumberofElements + 1; i++)
        {
            float ReferencePoint = Mathf.Abs(BarStartPos) + DistanceBetweenElements * (i - 1);
            if (Mathf.Abs(BarXPos) < ReferencePoint + DistanceBetweenElements / 2 && Mathf.Abs(BarXPos) > ReferencePoint - DistanceBetweenElements / 2) // falls within half of the distance to the next/previous element
            {
                LevelBar.transform.localPosition = new Vector2(-ReferencePoint, LevelBar.transform.localPosition.y);
                DisplayLevelImage(i);
                EndlessLevelIndex = i;
                if (EndlessLevelIndex <= 1)
                {
                    EndlessLevelIndex = 1;
                }
#if UNITY_EDITOR
                Debug.Log("This is endless level " + EndlessLevelIndex + " selected");
#endif
                //Spectator.LevelIndex = EndlessLevelIndex;
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
