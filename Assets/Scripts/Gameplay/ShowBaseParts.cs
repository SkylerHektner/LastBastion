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
        if( PD.Instance.UnlockMap.Get( UnlockFlag.Typhoon ) ) // Typhoon purchased? Show parts
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
        if( PD.Instance.UnlockMap.Get( UnlockFlag.ChainLightning ) )
        {
            LightningNubs.SetActive( true );
        }
        else
        {
            LightningNubs.SetActive( false );
        }

        /// TEMPORAL ANOMALY
        if( PD.Instance.UnlockMap.Get( UnlockFlag.Anomaly ) )
        {
            AnomalyBars.SetActive( true );
        }
        else
        {
            AnomalyBars.SetActive( false );
        }

        /// Sawmageddon HP Bar
        if( PD.Instance.UnlockMap.Get( UnlockFlag.Sawmageddon ) )
        {
            SawmageddonBoxes.SetActive( true );
            if( SawmageddonAbility.AnimatorDuration > 0 && PD.Instance.UnlockMap.Get( UnlockFlag.SawmageddonComboKiller, GameplayManager.Instance.Survival ) ) // only show while active
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
        if( PD.Instance.UnlockMap.Get( UnlockFlag.BaseOvershield ) )
        {
            Overshield.SetActive( true );
        }
        else
        {
            Overshield.SetActive( false );
        }

        // Turrets
        if( PD.Instance.UnlockMap.Get( UnlockFlag.Turrets ) )
        {
            Turrets.SetActive( true );
        }
        else
        {
            Turrets.SetActive( false );
        }

        // HP 1
        if( PD.Instance.UnlockMap.Get( UnlockFlag.BaseHP1 ) )
        {
            HP1Visuals.SetActive( true );
        }
        else
        {
            HP1Visuals.SetActive( false );
        }
        // HP 2
        if( PD.Instance.UnlockMap.Get( UnlockFlag.BaseHP2 ) )
        {
            HP2Visuals.SetActive( true );
        }
        else
        {
            HP2Visuals.SetActive( false );
        }
        // HP 3
        if( PD.Instance.UnlockMap.Get( UnlockFlag.BaseHP3 ) )
        {
            HP3Visuals.SetActive( true );
        }
        else
        {
            HP3Visuals.SetActive( false );
        }
    }



}
