using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu( fileName = "UnlockFlagUIInformation", menuName = "ScriptableObjects/UnlockFlagUIInformation", order = 1 )]
[System.Serializable]
public class UnlockFlagUIInformation : ScriptableObject
{
    public UnlockFlag UnlockFlag;
    public string UnlockName;
    [Multiline( 5 )]
    public string Description;
    public int CampaignCost;
    public Sprite SurvivalIcon;
    public Color GlowColor;
}
