using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class TradesCanvasManager : MonoBehaviour
{
    [SerializeField] InfoViewer TradesInfoViewer;

    public void OnConfirmPurchase()
    {
        StoreItem current_store_item = TradesInfoViewer.GetItemAtCurrentIndex()?.GetComponent<StoreItem>();
        if (current_store_item != null)
        {
            TryPurchaseOffer(current_store_item.CosmeticInformation);
        }
        else
        {
            Debug.LogError("Error: Info Viewer Item Missing Store Item Component In Offer Menu", this);
        }
    }

    public bool TryPurchaseOffer(CosmeticDisplayInterface trade_cosmetic)
    {
        if (PD.Instance.AchievementPoints.Get() >= trade_cosmetic.GetPrice())
        {
            trade_cosmetic.ApplyUnlocks();
            PD.Instance.AchievementPoints.Set(PD.Instance.AchievementPoints.Get() - (int)trade_cosmetic.GetPrice());
            return true;
        }

        return false;
    }
}
