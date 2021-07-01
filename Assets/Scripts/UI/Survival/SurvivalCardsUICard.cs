using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalCardsUICard : MonoBehaviour
{
    [SerializeField] Image GlowImage;
    [SerializeField] Image ButtonImage;
    [SerializeField] Animator Anim;
    public bool CurseCard;
    [HideInInspector] public UnlockFlagUIInformation Information;

    public void Start()
    {
        GlowImage.color = Color.clear;
    }

    public void SetGlowColor(Color color)
    {
        GlowImage.color = color;
    }

    public void UpdateIcon()
    {
        Debug.Assert( Information != null );

        ButtonImage.sprite = Information.SurvivalIcon;
    }

    public void ShowLockAnimation()
    {
        Anim.SetTrigger( "Lock" );
    }
}
