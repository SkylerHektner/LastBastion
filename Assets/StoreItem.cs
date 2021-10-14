using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreItem : MonoBehaviour
{
    public CosmeticDisplayInterface CosmeticInformation;
    public bool PremiumItem;
    public GameObject PurchasedSymbol;
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
                PurchasedSymbol.SetActive(true);
            }
            else
            {
                PurchasedSymbol.SetActive(false);
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
}
