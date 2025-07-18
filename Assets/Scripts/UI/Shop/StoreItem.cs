using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{
    public CosmeticDisplayInterface CosmeticInformation;
    public bool PremiumItem;
    public bool ItemPurchased { get; private set; }
    public string FallbackDescription;

    public string GetInfoPrice()
    {
        if (PremiumItem)
        {
            return "Get\nDLC";
            //return ("$" + CosmeticInformation.GetPrice().ToString());
        }
        else
        {
            return (CosmeticInformation.GetPrice().ToString());
        }
    }
    public string GetInfoDescription()
    {
        if (CosmeticInformation == null)
        {
            return GetFallBackDescription();
        }
        else
            return CosmeticInformation.GetDescription();
    }

    public string GetFallBackDescription()
    {
        return FallbackDescription;
    }

    public string GetInfoName()
    {
        return CosmeticInformation.GetName();
    }

#if UNITY_EDITOR
    [ContextMenu("TogglePurchase")]
    private void TogglePurchase()
    {
        ItemPurchased = !ItemPurchased;
    }
#endif

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
        if (CosmeticInformation == null)
        {
            return;
        }
        ItemPurchased = CosmeticInformation.IsUnlocked();
    }


}
