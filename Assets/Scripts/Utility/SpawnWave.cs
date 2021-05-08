using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SpawnWave
{
    public int CompletionReward = 0;
    public List<EnemyEnum> PassiveEnemies;
    public List<float> PassiveEnemySpawnCadence;
    public List<SpawnGroup> SpawnGroups;
    public List<float> SpawnGroupSpawnTimes;
    public bool collapsed = false; // Editor UI Only
    public string AnimationTrigger;

    public SpawnWave()
    {
        PassiveEnemies = new List<EnemyEnum>();
        PassiveEnemySpawnCadence = new List<float>();
        SpawnGroups = new List<SpawnGroup>();
        SpawnGroupSpawnTimes = new List<float>();
    }

    public SpawnWave( SpawnWave other )
    {
        PassiveEnemies = new List<EnemyEnum>();
        PassiveEnemySpawnCadence = new List<float>();
        SpawnGroups = new List<SpawnGroup>();
        SpawnGroupSpawnTimes = new List<float>();

        foreach( var e in other.PassiveEnemies )
            PassiveEnemies.Add( e );
        foreach( var e in other.PassiveEnemySpawnCadence )
            PassiveEnemySpawnCadence.Add( e );
        foreach( var e in other.SpawnGroups )
            SpawnGroups.Add( e );
        foreach( var e in other.SpawnGroupSpawnTimes )
            SpawnGroupSpawnTimes.Add( e );

        AnimationTrigger = other.AnimationTrigger;
    }
}
