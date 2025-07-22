using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

public class TradesCanvasManager : MonoBehaviour
{
    [SerializeField] InfoViewer TradesInfoViewer;
    public Animator CandyBucketAnimator;
    public bool FailPurchase;
    public Animator DeniedAnimator;
    public VolumeController ConfirmationPopup;
    public Button TradesBuyButton;
    public Sprite TradesOwnedImage;
    public bool DLC_Owned;

    public void OnEnable()
    {
        // Spectator.Instance.UnityIAP.PurchaseFailedEvent.AddListener( OnPurchaseFailed );
        // Spectator.Instance.UnityIAP.PurchaseCompletedEvent.AddListener( OnPurchaseCompleted );
        if (DLC_Owned)
        {
            DisableTradesPurchaseButton();
        }
    }

    public void OnDestroy()
    {
        // Spectator.Instance.UnityIAP.PurchaseFailedEvent.RemoveListener( OnPurchaseFailed );
        // Spectator.Instance.UnityIAP.PurchaseCompletedEvent.RemoveListener( OnPurchaseCompleted );
    }

    public void OnConfirmPurchase()
    {
        StoreItem current_store_item = TradesInfoViewer.GetItemAtCurrentIndex()?.GetComponent<StoreItem>();
        if (current_store_item != null)
        {
            if (FailPurchase)
            {
                OnPurchaseFailed();
            }
            else
            {
                TryPurchaseOffer(current_store_item.CosmeticInformation);
            }
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
            //Spectator.Instance.UnityIAP.BuyProductID( trade_cosmetic.GetProductID() );
        }
        else
        {
            Debug.LogError( $"ERROR: Trying to purchase cosmetic {trade_cosmetic.GetName()} as an offer that is not premium!" );
        }
    }

    [ContextMenu( "OnPurchaseFailed" )]
    private void OnPurchaseFailed()
    {
        Debug.Log("Failed to purchase item! Go get your moms credit card");
        CandyBucketAnimator.SetTrigger("Denied");
        DeniedAnimator.SetTrigger("Invalid");
        DeniedAnimator.GetComponent<VolumeController>().PlayMyAlternateSound();
    }

    [ContextMenu( "OnPurchaseCompleted" )]
    private void OnPurchaseCompleted()
    {
        TradesInfoViewer.RefreshStoreItem();
        CandyBucketAnimator.SetTrigger("Thanks");
        DeniedAnimator.SetTrigger("Success");
        ConfirmationPopup.PlayMySound();
    }
    
    [ContextMenu("DisableTradesPurchaseButton")]
    public void DisableTradesPurchaseButton()
    {
        TradesBuyButton.enabled = false;
        TradesBuyButton.image.sprite = TradesOwnedImage; // swap sprite!
        CandyBucketAnimator.SetBool("DLC_Owned", true);
    }
}
