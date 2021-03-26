using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Button))]
public class UpgradeButton : MonoBehaviour
{
    public string MyName;
    public string MyInfo;
    public int MyCost;
    public GameObject InfoBox;
    public PD.UpgradeFlags UpgradeFlag;
    public List<PD.UpgradeFlags> PrerequisiteUpgradeFlags = new List<PD.UpgradeFlags>();
    public GameObject PurchasedGlow;
    public Sprite VeiledImage;
    public Sprite Lockedimage;
    public Sprite UnlockedImage;

    public bool Purchased { get {
            return purchased;
        }
        set {
            purchased = value;
            //UpdateButton();
        }
    }
    private bool purchased;

    // bring up panel
    public void AskConfirmation()
    {
        InfoBox.SetActive(true);
        InfoBox.GetComponentInParent<Animator>().SetTrigger("Show");
        InfoPanel ThePanel = InfoBox.GetComponent<InfoPanel>();
        ThePanel.UpgradeName.text = MyName;
        ThePanel.UpgradeInfo.text = MyInfo;
        ThePanel.CandyCostText.text = MyCost.ToString();
        ThePanel.UpgradeCost = MyCost;
        ThePanel.DesiredUpgrade = this.gameObject.GetComponent<UpgradeButton>(); // for unlocking
        ThePanel.EnableButtons();
    }

    private void Start()
    {
        PD.Instance.UpgradeFlagChangedEvent.AddListener( OnUpgradeFlagChanged );
    }

    private void OnDestroy()
    {
        PD.Instance.UpgradeFlagChangedEvent.RemoveListener( OnUpgradeFlagChanged );
    }

    private void OnUpgradeFlagChanged(PD.UpgradeFlags flag, bool new_value)
    {
        if (PD.Instance.UpgradeUnlockMap.GetUnlock(UpgradeFlag))
            Purchased = true;
        UpdateButton();
    }

    private void OnEnable()
    {
        InfoPanel ThePanel = InfoBox.GetComponent<InfoPanel>();
        ThePanel.UpdatePlayerWealth();
        if( PD.Instance.UpgradeUnlockMap.GetUnlock( UpgradeFlag ))
        {
            Purchased = true;
            PurchasedGlow.SetActive(true);
        }
        else
        {
            PurchasedGlow.SetActive(false);
            Purchased = false;
        }

        UpdateButton();
    }

    public void EnableGlow() // Called by unlock animation
    {
        PurchasedGlow.SetActive(true);
        gameObject.GetComponent<Image>().sprite = UnlockedImage;
        PD.Instance.PlayerWealth.Set( PD.Instance.PlayerWealth.Get() - MyCost );
        PD.Instance.UpgradeUnlockMap.SetUnlock( UpgradeFlag, true );
    }

    private void UpdateButton()
    {

        if ( PrerequisiteUpgradeFlags.Any( f => !PD.Instance.UpgradeUnlockMap.GetUnlock( f ) ) ) 
        {
            // disable button if any pre-reqs not set
            PurchasedGlow.SetActive(false);
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
        }
    }

}
