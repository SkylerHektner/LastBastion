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


    // bring up panel
    public void AskConfirmation()
    {
        InfoBox.SetActive(true);
        InfoBox.GetComponent<InfoPanel>().UpgradeName.text = MyName;
        InfoBox.GetComponent<InfoPanel>().UpgradeInfo.text = MyInfo;
        InfoBox.GetComponent<InfoPanel>().CoinCost.text = MyCost.ToString();
        InfoBox.GetComponent<InfoPanel>().EnableButtons();
    }
}
