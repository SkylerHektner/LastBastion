using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent( typeof( Button ) )]
public class UpgradeButton : MonoBehaviour
{
    public UnlockFlagUIInformation UnlockFlagInformation;
    public GameObject InfoBox;
    public UnlockFlag UpgradeFlag;
    public GameObject PurchasedGlow;
    public Sprite VeiledImage;
    public Sprite Lockedimage;
    public Sprite UnlockedImage;
    public GameObject AvailableGlow;

    public bool Purchased
    {
        get
        {
            return purchased;
        }
        set
        {
            purchased = value;
            //UpdateButton();
        }
    }
    private bool purchased;

    // bring up panel
    public void AskConfirmation()
    {
        InfoBox.SetActive( true );
        InfoBox.GetComponentInParent<Animator>().SetTrigger( "Show" );
        InfoPanel ThePanel = InfoBox.GetComponent<InfoPanel>();
        ThePanel.UpgradeName.text = UnlockFlagInformation.UnlockName;
        ThePanel.UpgradeInfo.text = UnlockFlagInformation.Description;
        ThePanel.CandyCostText.text = UnlockFlagInformation.CampaignCost.ToString();
        ThePanel.UpgradeCost = UnlockFlagInformation.CampaignCost;
        ThePanel.DesiredUpgrade = this.gameObject.GetComponent<UpgradeButton>(); // for unlocking
        ThePanel.EnableButtons();
        ThePanel.LastClickedUpgrade = GetComponent<Button>();
        if (Purchased)
        {
            ThePanel.PurchaseButton.enabled = false;
            ThePanel.PurchaseButton.image.color = new Color(0.3372549f, 0.5882353f, 0.3529412f, 0.6352941f);
            ThePanel.UpgradeDenyButton.Select();
        }
        else
        {
            ThePanel.PurchaseButton.enabled = true;
            ThePanel.PurchaseButton.image.color = new Color(0f, 1f, 0.04879761f, 1f);
            ThePanel.PurchaseButton.Select();
        }
    }

    private void Start()
    {
        PD.Instance.UnlockFlagChangedEvent.AddListener( OnUpgradeFlagChanged );
    }

    private void OnDestroy()
    {
        PD.Instance.UnlockFlagChangedEvent.RemoveListener( OnUpgradeFlagChanged );
    }

    private void OnUpgradeFlagChanged( UnlockFlag flag, bool new_value )
    {
        if( PD.Instance.UnlockMap.Get( UpgradeFlag, false ) )
            Purchased = true;
        UpdateButton();
    }

    private void OnEnable()
    {
        InfoPanel ThePanel = InfoBox.GetComponent<InfoPanel>();
        ThePanel.UpdatePlayerWealth();
        if( PD.Instance.UnlockMap.Get( UpgradeFlag, false ) )
        {
            Purchased = true;
            PurchasedGlow.SetActive( true );
        }
        else
        {
            PurchasedGlow.SetActive( false );
            Purchased = false;
        }

        UpdateButton();
    }

    public void EnableGlow() // Called by unlock animation
    {
        PurchasedGlow.SetActive( true );
        gameObject.GetComponent<Image>().sprite = UnlockedImage;
        PD.Instance.PlayerWealth.Set( PD.Instance.PlayerWealth.Get() - UnlockFlagInformation.CampaignCost );
        PD.Instance.UnlockMap.Set( UpgradeFlag, true, false );
        InfoPanel ThePanel = InfoBox.GetComponent<InfoPanel>();
        ThePanel.UpdatePlayerWealth();
        AvailableGlow.SetActive( false );
    }

    private void UpdateButton()
    {

        if( PD.Instance.UnlockFlagDependencyMap[UpgradeFlag].Any( f => !PD.Instance.UnlockMap.Get( f, false ) ) )
        {
            // disable button if any pre-reqs not set
            PurchasedGlow.SetActive( false );
            gameObject.GetComponent<Image>().sprite = VeiledImage;
            GetComponent<Button>().enabled = false;
        }
        else if( Purchased )
        {
            //PurchasedGlow.SetActive(true);
            gameObject.GetComponent<Image>().sprite = UnlockedImage;
            GetComponent<Button>().enabled = true;
        }
        else
        {
            //PurchasedGlow.SetActive(false);
            gameObject.GetComponent<Image>().sprite = Lockedimage;
            GetComponent<Button>().enabled = true;
            AvailableGlow.SetActive( true );

        }
    }

}
