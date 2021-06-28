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
    public GameObject HP1Visuals;
    public GameObject HP2Visuals;
    public GameObject HP3Visuals;



    // Update is called once per frame
    void FixedUpdate()
    {
        /// TYPHOON
        if( PD.Instance.UnlockMap.Get( UnlockFlags.Typhoon, GameplayManager.Instance.Survival ) ) // Typhoon purchased? Show parts
        {
            FuelBars.SetActive( true );
            if( TyphoonAbility.AnimatorDuration > 0 )
            {
                FuelBarR.SetSize( TyphoonAbility.AnimatorDuration / 5 ); // replace 5 with fuel maximum amount
                FuelBarL.SetSize( TyphoonAbility.AnimatorDuration / 5 );
            }
        }
        else
        {
            FuelBars.SetActive( false );
        }

        /// CHAIN LIGHTNING
        if( PD.Instance.UnlockMap.Get( UnlockFlags.ChainLightning, GameplayManager.Instance.Survival ) )
        {
            LightningNubs.SetActive( true );
        }
        else
        {
            LightningNubs.SetActive( false );
        }

        /// TEMPORAL ANOMALY
        if( PD.Instance.UnlockMap.Get( UnlockFlags.Anomaly, GameplayManager.Instance.Survival ) )
        {
            AnomalyBars.SetActive( true );
        }
        else
        {
            AnomalyBars.SetActive( false );
        }

        /// Sawmageddon HP Bar
        if( PD.Instance.UnlockMap.Get( UnlockFlags.Sawmageddon, GameplayManager.Instance.Survival ) )
        {
            SawmageddonBoxes.SetActive( true );
            if( SawmageddonAbility.AnimatorDuration > 0 && PD.Instance.UnlockMap.Get( UnlockFlags.SawmageddonComboKiller, GameplayManager.Instance.Survival ) ) // only show while active
            {
                SawMaggeddonConnectors.SetActive( true );
                RecoverHPBar.SetActive( true );
            }
            else
            {
                SawMaggeddonConnectors.SetActive( false );
                RecoverHPBar.SetActive( false );
            }
        }
        else
        {
            SawmageddonBoxes.SetActive( false );
        }

        // HP overshield
        if( PD.Instance.UnlockMap.Get( UnlockFlags.BaseOvershield, GameplayManager.Instance.Survival ) )
        {
            Overshield.SetActive( true );
        }
        else
        {
            Overshield.SetActive( false );
        }

        // Turrets
        if( PD.Instance.UnlockMap.Get( UnlockFlags.Turrets, GameplayManager.Instance.Survival ) )
        {
            Turrets.SetActive( true );
        }
        else
        {
            Turrets.SetActive( false );
        }

        // HP 1
        if( PD.Instance.UnlockMap.Get( UnlockFlags.BaseHP1, GameplayManager.Instance.Survival ) )
        {
            HP1Visuals.SetActive( true );
        }
        else
        {
            HP1Visuals.SetActive( false );
        }
        // HP 2
        if( PD.Instance.UnlockMap.Get( UnlockFlags.BaseHP2, GameplayManager.Instance.Survival ) )
        {
            HP2Visuals.SetActive( true );
        }
        else
        {
            HP2Visuals.SetActive( false );
        }
        // HP 3
        if( PD.Instance.UnlockMap.Get( UnlockFlags.BaseHP3, GameplayManager.Instance.Survival ) )
        {
            HP3Visuals.SetActive( true );
        }
        else
        {
            HP3Visuals.SetActive( false );
        }
    }



}
