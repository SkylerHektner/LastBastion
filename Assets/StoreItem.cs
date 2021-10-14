using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreItem : MonoBehaviour
{
    public Cosmetic CosmeticInformation;
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

    public virtual string GetInfoPrice()
    {
        return ("$" + CosmeticInformation.Price.ToString());
    }
    public virtual string GetInfoDescription()
    {
        return CosmeticInformation.Description;
    }

    [ContextMenu("TogglePurchase")]
    private void TogglePurchase()
    {
        ItemPurchased = !ItemPurchased;
    }
}
