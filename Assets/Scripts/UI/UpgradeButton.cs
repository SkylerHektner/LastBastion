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
    public PlayerData.UpgradeFlags UpgradeFlag;
    public List<PlayerData.UpgradeFlags> PrerequisiteUpgradeFlags = new List<PlayerData.UpgradeFlags>();
    public GameObject PurchasedGlow;
    public Sprite VeiledImage;
    public Sprite Lockedimage;
    public Sprite UnlockedImage;

    public bool Purchased { get {
            return purchased;
        }
        set {
            purchased = value;
            UpdateButton();
        }
    }
    private bool purchased;

    // bring up panel
    public void AskConfirmation()
    {
        InfoBox.SetActive(true);
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
        PlayerData.Instance.UpgradeFlagChangedEvent.AddListener( OnUpgradeFlagChanged );
    }

    private void OnDestroy()
    {
        PlayerData.Instance.UpgradeFlagChangedEvent.RemoveListener( OnUpgradeFlagChanged );
    }

    private void OnUpgradeFlagChanged(PlayerData.UpgradeFlags flag, bool new_value)
    {
        if (PlayerData.Instance.UpgradeUnlockMap.GetUnlock(UpgradeFlag))
            Purchased = true;
        UpdateButton();
    }

    private void OnEnable()
    {
        InfoPanel ThePanel = InfoBox.GetComponent<InfoPanel>();
        ThePanel.UpdatePlayerWealth();
        if( PlayerData.Instance.UpgradeUnlockMap.GetUnlock( UpgradeFlag ) )
            Purchased = true;
        UpdateButton();
    }
    private void UpdateButton()
    {
        if( PrerequisiteUpgradeFlags.Any( f => !PlayerData.Instance.UpgradeUnlockMap.GetUnlock( f ) ) ) 
        {
            // disable button if any pre-reqs not set
            PurchasedGlow.SetActive(false);
            gameObject.GetComponent<Image>().sprite = VeiledImage;
            GetComponent<Button>().enabled = false;
        }
        else if( Purchased )
        {
            //ColorBlock ButtonColor = gameObject.GetComponent<Button>().colors;
            //Color Green = new Color( 0 / 255, 255 / 255, 31 / 255 );
            //ButtonColor.normalColor = Green;
            //ButtonColor.highlightedColor = ButtonColor.disabledColor;
            //gameObject.GetComponent<Button>().colors = ButtonColor;
            PurchasedGlow.SetActive(true);
            gameObject.GetComponent<Image>().sprite = UnlockedImage;
            GetComponent<Button>().enabled = true;
        }
        else
        {
            //ColorBlock ButtonColor = gameObject.GetComponent<Button>().colors;
            //ButtonColor.normalColor = new Color( 255, 255, 255 );
            //gameObject.GetComponent<Button>().colors = ButtonColor;
            PurchasedGlow.SetActive(false);
            gameObject.GetComponent<Image>().sprite = Lockedimage;
            GetComponent<Button>().enabled = true;
        }
    }
}
