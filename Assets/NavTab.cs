﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavTab : MonoBehaviour
{

    public GameObject UpgradesSwapper;
    public GameObject HelpSwapper;
    public GameObject SettingsSwapper;

    public GameObject SettingsContent;
    public GameObject UpgradesContent;
    public GameObject HelpContent;


    public void HideButtons()
    {
        UpgradesSwapper.SetActive(false);
        HelpSwapper.SetActive(false);
        SettingsSwapper.SetActive(false);
    }

    public void ShowUpgrades()
    {
        UpgradesContent.SetActive(true);
        SettingsContent.SetActive(false);
        HelpContent.SetActive(false);
    }

    public void ShowHelp()
    {
        UpgradesContent.SetActive(false);
        SettingsContent.SetActive(false);
        HelpContent.SetActive(true);
    }

    public void ShowSettings()
    {
        UpgradesContent.SetActive(false);
        SettingsContent.SetActive(true);
        HelpContent.SetActive(false);
    }

    public void HideAllContent()
    {
        UpgradesContent.SetActive(false);
        SettingsContent.SetActive(false);
        HelpContent.SetActive(false);
    }
}
