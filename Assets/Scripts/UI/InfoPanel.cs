﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public TextMeshProUGUI UpgradeName;
    public TextMeshProUGUI UpgradeInfo;
    public TextMeshProUGUI CandyCostText;
    public TextMeshProUGUI PlayerWealthText;

    public Button DenyButton;
    public Button PurchaseButton;

    public UpgradeButton DesiredUpgrade;
    public int PlayerWealth;
    public int UpgradeCost;

    public Animator GoodBubble;
    public Animator BadBubble;


    public void ConfirmPurchase()
    {
        if (PlayerWealth >= UpgradeCost)
        {
            PlayerWealth -= UpgradeCost;
            if (PlayerWealth <= 0)
            {
                PlayerWealth = 0;
            }
            DesiredUpgrade.Purchased = true;
            gameObject.SetActive(false);
            DenyButton.enabled = false;
            PurchaseButton.enabled = false;
            UpdatePlayerWealth();
            GoodBubble.SetTrigger("Grow");
        }
        else
        {
            BadBubble.SetTrigger("Grow");
        }
    }

    private void Awake()
    {
        //EnableButtons();
    }



    public void DenyPurchase()
    {
        gameObject.SetActive(false);
        DenyButton.enabled = false;
        PurchaseButton.enabled = false;
    }

    public void UpdatePlayerWealth()
    {
        PlayerWealthText.text = PlayerWealth.ToString();
    }

    public void EnableButtons()
    {
        UpdatePlayerWealth();
        DenyButton.enabled = true;
        PurchaseButton.enabled = true;

        // view purchased upgrades, but don't allow a double buy
        if (DesiredUpgrade.Purchased)
        {
            PurchaseButton.interactable = false;
        }
        else
        {
            PurchaseButton.interactable = true;
        }
    }

    public void HideButtons()
    {
        DenyButton.enabled = false;
        PurchaseButton.enabled = false;
    }

}