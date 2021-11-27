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

    public void TryPurchaseOffer(CosmeticDisplayInterface trade_cosmetic)
    {
        if( trade_cosmetic.GetIsPremium() )
        {
            Spectator.Instance.UnityIAP.BuyProductID( trade_cosmetic.GetProductID() );
        }
        else
        {
            Debug.LogError( $"ERROR: Trying to purchase cosmetic {trade_cosmetic.GetName()} as an offer that is not premium!" );
        }
    }
}
