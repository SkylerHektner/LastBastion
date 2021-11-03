using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{
    public CosmeticDisplayInterface CosmeticInformation;
    public bool PremiumItem;
    //public GameObject PurchasedSymbol;
    //public Button PurchaseButton;
    public bool ItemPurchased
    {
        get
        {
            return purchased;
        }
        set
        {
            purchased = value;

            if (purchased)
            {
                //PurchasedSymbol.SetActive(true);
                //PurchaseButton.enabled = false;
            }
            else
            {
                //PurchasedSymbol.SetActive(false);
                //PurchaseButton.enabled = true;
            }
        }
    }
    private bool purchased;

    public string GetInfoPrice()
    {
        if (PremiumItem)
        {
            return ("$" + CosmeticInformation.GetPrice().ToString());
        }
        else
        {
            return (CosmeticInformation.GetPrice().ToString());
        }
    }
    public string GetInfoDescription()
    {
        return CosmeticInformation.GetDescription();
    }
    public string GetInfoName()
    {
        return CosmeticInformation.GetName();
    }

    [ContextMenu("TogglePurchase")]
    private void TogglePurchase()
    {
        ItemPurchased = !ItemPurchased;
    }

    public void Start()
    {
        PD.Instance.UnlockFlagChangedEvent.AddListener( OnUnlockFlagChanged );
    }

    public void OnDestroy()
    {
        PD.Instance.UnlockFlagChangedEvent.RemoveListener( OnUnlockFlagChanged );
    }

    public void Awake()
    {
        UpdatePurchased();
    }

    public void OnUnlockFlagChanged(UnlockFlag flag, bool new_value)
    {
        UpdatePurchased();
    }

    private void UpdatePurchased()
    {
        ItemPurchased = CosmeticInformation.IsUnlocked();
    }
}
