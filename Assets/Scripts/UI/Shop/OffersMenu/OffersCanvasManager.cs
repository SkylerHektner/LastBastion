using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class OffersCanvasManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI candy_bowl_tmp;
    [SerializeField] InfoViewer OfferInfoViewer;

    private int cur_candy_bowl_amount = 0;

    private void Awake()
    {
        candy_bowl_tmp.text = PD.Instance.AchievementPoints.Get().ToString();
        cur_candy_bowl_amount = PD.Instance.AchievementPoints.Get();
    }

    private void FixedUpdate()
    {
        if( cur_candy_bowl_amount != PD.Instance.AchievementPoints.Get() )
        {
            cur_candy_bowl_amount += (int)Mathf.Sign( PD.Instance.AchievementPoints.Get() - cur_candy_bowl_amount );
            candy_bowl_tmp.text = cur_candy_bowl_amount.ToString();
        }
    }

    public void OnConfirmPurchase()
    {
        StoreItem current_store_item = OfferInfoViewer.GetItemAtCurrentIndex()?.GetComponent<StoreItem>();
        if( current_store_item != null)
        {
            TryPurchaseOffer( current_store_item.CosmeticInformation );
        }
        else
        {
            Debug.LogError( "Error: Info Viewer Item Missing Store Item Component In Offer Menu", this );
        }
    }

    public bool TryPurchaseOffer( CosmeticDisplayInterface offer_cosmetic )
    {
        if( PD.Instance.AchievementPoints.Get() >= offer_cosmetic.GetPrice() )
        {
            offer_cosmetic.ApplyUnlocks();
            PD.Instance.AchievementPoints.Set( PD.Instance.AchievementPoints.Get() - (int)offer_cosmetic.GetPrice() );
            return true;
        }

        return false;
    }
}
