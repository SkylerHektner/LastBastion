using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

public class OffersCanvasManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI candy_bowl_tmp;
    [SerializeField] InfoViewer OfferInfoViewer;

    private int cur_candy_bowl_amount = 0;
    public Animator CandyBowlAnimator;
    public Animator ConfirmationPanel;
    public GameObject ClaimButton;
    public GameObject RightArrow;
    public GameObject LeftArrow;
    public Button HomeButton;
    public Animator WealthPlate;

    private void Awake()
    {
        candy_bowl_tmp.text = PD.Instance.AchievementPoints.Get().ToString();
        cur_candy_bowl_amount = PD.Instance.AchievementPoints.Get();
        CandyBowlAnimator.SetInteger("CandyValue", cur_candy_bowl_amount);
    }

    private void FixedUpdate()
    {
        if( cur_candy_bowl_amount != PD.Instance.AchievementPoints.Get() )
        {
            cur_candy_bowl_amount += (int)Mathf.Sign( PD.Instance.AchievementPoints.Get() - cur_candy_bowl_amount );
            candy_bowl_tmp.text = cur_candy_bowl_amount.ToString();
            CandyBowlAnimator.SetInteger("CandyValue", cur_candy_bowl_amount);
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

    public void EvaluateCurrency() // brings up the confirmation menu if the player has enough currency to even purchase the item. If not, play feedback anim
    {
        StoreItem current_store_item = OfferInfoViewer.GetItemAtCurrentIndex()?.GetComponent<StoreItem>(); // get current item in question, then compare prices
        if (PD.Instance.AchievementPoints.Get() >= current_store_item.CosmeticInformation.GetPrice()) // player is wealthy enough
        {
            ConfirmationPanel.SetTrigger("Show");
            ClaimButton.SetActive(false);
            RightArrow.SetActive(false);
            LeftArrow.SetActive(false);
            HomeButton.interactable = false;
        }
        else if (PD.Instance.AchievementPoints.Get() < current_store_item.CosmeticInformation.GetPrice()) // player is too poor, sad
        {
            WealthPlate.SetTrigger("Invalid");
        }
    }
}
