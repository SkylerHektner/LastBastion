using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavTab : MonoBehaviour
{

    public GameObject UpgradesSwapper;

    public GameObject UpgradesContent;
    public InfoPanel InfoPanel;

    public void ShowUpgrades()
    {
        UpgradesContent.SetActive(true);
        InfoPanel.DenyPurchase();
    }

}
