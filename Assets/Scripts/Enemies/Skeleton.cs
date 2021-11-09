using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{
    public float ShieldSkeletonUpgradeCurseMovespeedMultiplier = 0.7f;
    public GameObject BlueCrystal;
    public GameObject RedCrystal;
    public GameObject GreenCrystal;
    public GameObject PurpleCrystal;

    public override void Hit( Vector3 hit_direction, bool can_dodge, DamageSource source, out bool died, out bool dodged, int damage = 1 )
    {
        base.Hit( hit_direction, can_dodge, source, out died, out dodged, damage );

        if( !dodged &&
            !died &&
            source == DamageSource.Saw &&
            Saw.Instance.Moving &&
            PD.Instance.UnlockMap.Get( UnlockFlag.ShieldSkeletonUpgradeCurse ) )
        {
            Saw.Instance.SetShieldBreakMovespeedMultiplier( ShieldSkeletonUpgradeCurseMovespeedMultiplier );
        }
    }

    public void DropCrystalOnDeath() // used by golden skeletons only
    {
        int RandInt = Random.Range(0, 4);
        if (RandInt == 0) // Blue Crystal
        {
            Instantiate(BlueCrystal).transform.position = transform.position;
        }
        else if (RandInt == 1) // Red Crystal
        {
            Instantiate(RedCrystal).transform.position = transform.position;
        }
        else if (RandInt == 2) // Green Crystal
        {
            Instantiate(GreenCrystal).transform.position = transform.position;
        }
        else if (RandInt == 3) // Purple Crystal
        {
            Instantiate(PurpleCrystal).transform.position = transform.position;
        }
        Debug.Log(RandInt);
    }
}
