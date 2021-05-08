using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSpawnCadenceProfile : ScriptableObject
{
    public abstract SpawnWave GetWave( int wave_number );
    public abstract int GetWaveCount();
    public abstract string GetName();
    public abstract string GetLevelIdentifier();
    public abstract Challenge GetChallenge();
}
