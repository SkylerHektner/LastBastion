using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu( fileName = "UnlockFlagUIInformation", menuName = "ScriptableObjects/UnlockFlagUIInformation", order = 1 )]
[System.Serializable]
public class UnlockFlagUIInformation : ScriptableObject
{
    public PD.UnlockFlags UnlockFlag;
    public string UnlockName;
    [Multiline( 5 )]
    public string Description;
    public int CampaignCost;
    public Sprite SurvivalIcon;

    // colors for icon glow!
    public bool Red;
    public bool Green;
    public bool Blue;
    public bool Orange;
    public bool Purple;
    public bool Yellow;
    public bool White;
    public bool Opaque;

    Vector4 RedColor = new Vector4(1, 0, 0, 1);
    Vector4 GreenColor = new Vector4(0, 1, 0, 1);
    Vector4 BlueColor = new Vector4(.25f, 0.7f, 1,1);
    Vector4 PurpleColor = new Vector4(1, 0.3f, 1, 1);
    Vector4 YellowColor = new Vector4(1, 1, 0, 1);
    Vector4 OrangeColor = new Vector4(1, .7f, 0, 1);
    Vector4 WhiteColor = new Vector4(1, 1, 1, 1);
    Vector4 OpaqueColor = new Vector4(1, 1, 1, 0);

    Color ActiveGlowRGB;

    [ContextMenu("UpdateGlowColor")]
    public void UpdateGlowColor()
    {
        if (Red)
        {
            ActiveGlowRGB = RedColor;
        }
    }

}
