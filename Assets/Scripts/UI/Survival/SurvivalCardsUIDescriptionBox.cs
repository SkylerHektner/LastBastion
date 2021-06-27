using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SurvivalCardsUIDescriptionBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI NameText;
    [SerializeField] TextMeshProUGUI DescriptionText;

    public void Start()
    {
        Debug.Assert( NameText != null );
        Debug.Assert( DescriptionText != null );
    }

    public void SetTextFromUIInfo(UnlockFlagUIInformation Information)
    {
        NameText.text = Information.UnlockName;
        NameText.SetAllDirty();
        DescriptionText.text = Information.Description;
        DescriptionText.SetAllDirty();
    }
}
