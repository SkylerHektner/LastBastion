using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBaseParts : MonoBehaviour
{
    public HpBar FuelBarR;
    public HpBar FuelBarL;

    public GameObject FuelBars;
    public GameObject LightningNubs;
    public GameObject AnomalyBars;
    public GameObject SawMaggeddonConnectors;
    public GameObject RecoverHPBar;
    public GameObject SawmageddonBoxes;
    public GameObject Overshield;

    public GameObject Turrets;


    // Update is called once per frame
    void FixedUpdate()
    {
        /// TYPHOON
        if (PD.Instance.UpgradeUnlockMap.GetUnlock(PD.UpgradeFlags.Typhoon)) // Typhoon purchased? Show parts
        {
            FuelBars.SetActive(true);
            if (TyphoonAbility.AnimatorDuration > 0)
            {
                FuelBarR.SetSize(TyphoonAbility.AnimatorDuration / 5); // replace 5 with fuel maximum amount
                FuelBarL.SetSize(TyphoonAbility.AnimatorDuration / 5);
            }
        }
        else
        {
            FuelBars.SetActive(false);
        }

        /// CHAIN LIGHTNING
        if (PD.Instance.UpgradeUnlockMap.GetUnlock(PD.UpgradeFlags.ChainLightning))
        {
            LightningNubs.SetActive(true);
        }
        else
        {
            LightningNubs.SetActive(false);
        }

        /// TEMPORAL ANOMALY
        if (PD.Instance.UpgradeUnlockMap.GetUnlock(PD.UpgradeFlags.Anomaly))
        {
            AnomalyBars.SetActive(true);
        }
        else
        {
            AnomalyBars.SetActive(false);
        }

        /// Sawmageddon HP Bar
        if (PD.Instance.UpgradeUnlockMap.GetUnlock(PD.UpgradeFlags.Sawmageddon))
        {
            SawmageddonBoxes.SetActive(true);
            if (SawmageddonAbility.AnimatorDuration > 0 && PD.Instance.UpgradeUnlockMap.GetUnlock(PD.UpgradeFlags.SawmageddonComboKiller)) // only show while active
            {
                SawMaggeddonConnectors.SetActive(true);
                RecoverHPBar.SetActive(true);
            }
            else
            {
                SawMaggeddonConnectors.SetActive(false);
                RecoverHPBar.SetActive(false);
            }
        }
        else
        {
            SawmageddonBoxes.SetActive(false);
        }

        // HP overshield
        if (PD.Instance.UpgradeUnlockMap.GetUnlock(PD.UpgradeFlags.BaseOvershield))
        {
            Overshield.SetActive(true);
        }
        else
        {
            Overshield.SetActive(false);
        }

        // Turrets
        if (PD.Instance.UpgradeUnlockMap.GetUnlock(PD.UpgradeFlags.Turrets))
        {
            Turrets.SetActive(true);
        }
        else
        {
            Turrets.SetActive(false);
        }
    }



}
