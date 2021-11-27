using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrasButton : MonoBehaviour
{
    public List<Animator> ChainList;
    float ChainsSFXCooldown;
    bool ChainsCanRattle;

    public void RattleAllChains()
    {
        if (ChainsCanRattle)
        {
            foreach (Animator Chain in ChainList)
            {
                Chain.SetTrigger("Rattle");
                ChainsCanRattle = false;
                ChainsSFXCooldown = 1f;
            }
        }

    }

    public void FixedUpdate()
    {
        if (ChainsCanRattle == false) // delays the chains from being spammed
        {
            ChainsSFXCooldown -= Time.smoothDeltaTime;
            if (ChainsSFXCooldown <= 0)
            {
                ChainsSFXCooldown = 0; // no negatives pls
                ChainsCanRattle = true;
            }
        }
    }
}
