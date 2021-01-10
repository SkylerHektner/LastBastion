using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public string MyName;
    public string MyInfo;
    public int MyCost;
    public GameObject InfoBox;

    public bool Purchased { get {
            return purchased;
        }
        set {
            purchased = value;
            UpdateButtonColors();
        }
    }
    private bool purchased;


    // bring up panel
    public void AskConfirmation()
    {
        InfoBox.SetActive(true);
        InfoPanel ThePanel = InfoBox.GetComponent<InfoPanel>();
        ThePanel.UpgradeName.text = MyName;
        ThePanel.UpgradeInfo.text = MyInfo;
        ThePanel.CandyCostText.text = MyCost.ToString();
        ThePanel.UpgradeCost = MyCost;
        ThePanel.DesiredUpgrade = this.gameObject.GetComponent<UpgradeButton>(); // for unlocking
        ThePanel.EnableButtons();
    }

    private void OnEnable()
    {
        InfoPanel ThePanel = InfoBox.GetComponent<InfoPanel>();
        ThePanel.UpdatePlayerWealth();
        UpdateButtonColors();
    }
    private void UpdateButtonColors()
    {
        // disable button if purchased
        if( Purchased )
        {
            ColorBlock ButtonColor = gameObject.GetComponent<Button>().colors;
            Color Green = new Color( 0 / 255, 255 / 255, 31 / 255 );
            ButtonColor.normalColor = Green;
            ButtonColor.highlightedColor = ButtonColor.disabledColor;
            gameObject.GetComponent<Button>().colors = ButtonColor;
        }
        else
        {
            ColorBlock ButtonColor = gameObject.GetComponent<Button>().colors;
            ButtonColor.normalColor = new Color( 255, 255, 255 );
            gameObject.GetComponent<Button>().colors = ButtonColor;
        }
    }
}
