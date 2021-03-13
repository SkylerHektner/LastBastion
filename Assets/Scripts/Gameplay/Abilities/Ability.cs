using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public static long NextAbilityID = 1;
    public long AbilityID {
        get {
            if( abilityID == 0 )
            {
                abilityID = NextAbilityID++;
                if( NextAbilityID == long.MaxValue )
                    NextAbilityID = 0;
            }
            return abilityID;
        }
    }
    public string name;
    private long abilityID = 0;


    public AbilityManager AM;
    public AbilityEnum ability;

    public virtual void Start()
    {

    }

    public virtual void Update( float delta_time )
    {

    }

    public virtual void Finish()
    {
        AM.AbilityFinished( AbilityID );
    }

    // return true if we should proceed with construction of new ability instance
    // return false if we should cancel new ability construction
    public virtual bool OnAbilityUsedWhileAlreadyActive()
    {
        return true;
    }

    public virtual void OnSceneExit()
    {

    }
}
