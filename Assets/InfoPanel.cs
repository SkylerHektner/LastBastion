using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public TextMeshProUGUI UpgradeName;
    public TextMeshProUGUI UpgradeInfo;
    public TextMeshProUGUI CoinCost;

    public Button DenyButton;
    public Button PurchaseButton;


    public void ConfirmPurchase()
    {
        // purchase item
    }

    private void Awake()
    {
        EnableButtons();
    }

    public void DenyPurchase()
    {
        gameObject.SetActive(false);
        DenyButton.enabled = false;
        PurchaseButton.enabled = false;
    }

    public void EnableButtons()
    {
        DenyButton.enabled = true;
        PurchaseButton.enabled = true;
    }

    public void HideButtons()
    {
        DenyButton.enabled = false;
        PurchaseButton.enabled = false;
    }

}
