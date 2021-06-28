using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// DEPRECATED
/// </summary>
public class InfoScroll : MonoBehaviour
{
    public bool Lightning;
    public bool Sawmageddon;
    public bool Anomaly;
    public bool Typhoon;

    public GameObject LockedIcon1;
    public GameObject LockedIcon2;
    public GameObject LockedIcon3;

    public Sprite Mystery;

    // Update is called once per frame
    void Update()
    {
        if( Lightning )
        {
            //// Lightning Rod
            if( PD.Instance.UnlockMap.Get( UnlockFlags.ChainLightningLightningRod, GameplayManager.Instance.Survival ) )
            {
                LockedIcon1.SetActive( false );
            }
            else
            { LockedIcon1.SetActive( true ); }

            //// Lightning Stun Duration
            if( PD.Instance.UnlockMap.Get( UnlockFlags.ChainLightningStunDuration, GameplayManager.Instance.Survival ) )
            {
                LockedIcon2.SetActive( false );
            }
            else
            { LockedIcon2.SetActive( true ); }

            //// Static Overload
            if( PD.Instance.UnlockMap.Get( UnlockFlags.ChainLightningStaticOverload, GameplayManager.Instance.Survival ) )
            {
                LockedIcon3.SetActive( false );
            }
            else
            { LockedIcon3.SetActive( true ); }
        }
        if( Sawmageddon )
        {
            //// Sawmageddon Duration
            if( PD.Instance.UnlockMap.Get( UnlockFlags.SawmageddonDuration, GameplayManager.Instance.Survival ) )
            {
                LockedIcon1.SetActive( false );
            }
            else
            { LockedIcon1.SetActive( true ); }

            //// Sawmageddon Projectiles
            if( PD.Instance.UnlockMap.Get( UnlockFlags.SawmageddonProjectiles, GameplayManager.Instance.Survival ) )
            {
                LockedIcon2.SetActive( false );
            }
            else
            { LockedIcon2.SetActive( true ); }

            //// Sawmageddon HP Steal
            if( PD.Instance.UnlockMap.Get( UnlockFlags.SawmageddonComboKiller, GameplayManager.Instance.Survival ) )
            {
                LockedIcon3.SetActive( false );
            }
            else
            { LockedIcon3.SetActive( true ); }
        }
        if( Anomaly )
        {
            //// Anomaly Ricochet
            if( PD.Instance.UnlockMap.Get( UnlockFlags.AnomalyRicochetSaws, GameplayManager.Instance.Survival ) )
            {
                LockedIcon1.SetActive( false );
            }
            else
            { LockedIcon1.SetActive( true ); }

            //// Anomaly Mirror Saw
            if( PD.Instance.UnlockMap.Get( UnlockFlags.AnomalySingularity, GameplayManager.Instance.Survival ) )
            {
                LockedIcon2.SetActive( false );
            }
            else
            { LockedIcon2.SetActive( true ); }

            //// Anomaly Stasis
            if( PD.Instance.UnlockMap.Get( UnlockFlags.AnomalyStasisCoating, GameplayManager.Instance.Survival ) )
            {
                LockedIcon3.SetActive( false );
            }
            else
            { LockedIcon3.SetActive( true ); }
        }
        if( Typhoon )
        {
            //// Extended BBQ
            if( PD.Instance.UnlockMap.Get( UnlockFlags.TyphoonExtendedBBQ, GameplayManager.Instance.Survival ) )
            {
                LockedIcon1.SetActive( false );
            }
            else
            { LockedIcon1.SetActive( true ); }

            //// Flame Saw
            if( PD.Instance.UnlockMap.Get( UnlockFlags.TyphoonFlameSaw, GameplayManager.Instance.Survival ) )
            {
                LockedIcon2.SetActive( false );
            }
            else // flame saw is also required for other perks to be discovered
            {
                LockedIcon1.GetComponent<Image>().sprite = Mystery;
                LockedIcon3.GetComponent<Image>().sprite = Mystery;
            }

            //// Roaring Flames
            if( PD.Instance.UnlockMap.Get( UnlockFlags.TyphoonRoaringFlames, GameplayManager.Instance.Survival ) )
            {
                LockedIcon3.SetActive( false );
            }
            else
            { LockedIcon3.SetActive( true ); }
        }
    }
}
