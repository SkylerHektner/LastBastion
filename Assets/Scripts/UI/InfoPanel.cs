using System.Collections;
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
    public int UpgradeCost;

    public Animator GoodBubble;
    public Animator BadBubble;
    public Animator Skull;

    Spectator Spectator;


    public void ConfirmPurchase()
    {
        int cur_player_wealth = PD.Instance.PlayerWealth.Get();
        if ( cur_player_wealth >= UpgradeCost)
        {
            PD.Instance.PlayerWealth.Set( cur_player_wealth - UpgradeCost );
            PD.Instance.UpgradeUnlockMap.SetUnlock( DesiredUpgrade.UpgradeFlag, true );

            DesiredUpgrade.Purchased = true;
            DesiredUpgrade.GetComponent<Animator>().SetTrigger("Unlock"); // play cool unlock animation when purchased
            //gameObject.SetActive(false);
            //gameObject.GetComponentInParent<Animator>().SetTrigger("Hide");
            DenyButton.enabled = false;
            PurchaseButton.enabled = false;
            UpdatePlayerWealth();
            GoodBubble.SetTrigger("Grow");
            Skull.SetTrigger("Speak");
        }
        else
        {
            BadBubble.SetTrigger("Grow");
            Skull.SetTrigger("Speak");
        }
    }

    private void Awake()
    {
        //EnableButtons();
    }



    public void DenyPurchase()
    {
        //gameObject.SetActive(false);
        DenyButton.enabled = false;
        PurchaseButton.enabled = false;
        //gameObject.GetComponentInParent<Animator>().SetTrigger("Hide");

    }

    public void UpdatePlayerWealth()
    {
        Spectator = GameObject.FindGameObjectWithTag("Spectator").GetComponent<Spectator>();
        PlayerWealthText.text = PD.Instance.PlayerWealth.Get().ToString();
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
