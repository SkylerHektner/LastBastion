using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// gameplay manager responsible or maintaining one of these
public class ChallengesTracker
{
    public bool DamageTaken { get; private set; } = false;
    public bool CrystalsUsed { get; private set; } = false;
    public bool SawMuddied { get; private set; } = false;
    public int NumZappedEnemiesKilled { get; private set; } = 0;
    public float TotalLevelTimePassed { get; private set; } = 0.0f;

    private List<long> TrackedEnemies = new List<long>();

    public void Start()
    {
        BaseHP.Instance.DamageTakenEvent.AddListener( OnDamageTaken );
        AbilityManager.Instance.AbilityUsedEvent.AddListener( OnCrystalUsed );
        Saw.Instance.SawMuddiedEvent.AddListener( OnSawMuddied );
        SpawnManager.Instance.EnemySpawnedEvent.AddListener( OnEnemySpawned );
    }

    public void Tick( float deltaTime )
    {
        if( GameplayManager.PlayerWinState == GameplayManager.PlayerState.Active )
            TotalLevelTimePassed += deltaTime;
    }

    public void End()
    {
        BaseHP.Instance?.DamageTakenEvent?.RemoveListener( OnDamageTaken );
        AbilityManager.Instance?.AbilityUsedEvent?.RemoveListener( OnCrystalUsed );
        Saw.Instance?.SawMuddiedEvent?.RemoveListener( OnSawMuddied );
        SpawnManager.Instance?.EnemySpawnedEvent?.RemoveListener( OnEnemySpawned );
        foreach( long ID in TrackedEnemies )
        {
            SpawnManager.Instance?.TryGetEnemyByID( ID )?.DeathEvent.RemoveListener( OnEnemyDied );
        }
        TrackedEnemies.Clear();
    }

    public bool EvaluateChallengeSuccess( Challenge challenge )
    {
        Debug.Log( "DamageTaken" + DamageTaken );
        Debug.Log( "CrystalsUsed" + CrystalsUsed );
        Debug.Log( "SawMuddied" + SawMuddied );
        Debug.Log( "NumEnemiesKilledWhileEffectedByPowerups" + NumZappedEnemiesKilled );
        Debug.Log( "TotalLevelTimePassed" + TotalLevelTimePassed );

        bool success = true;
        success &= !( challenge.CannotTakeDamage && DamageTaken );
        success &= !( challenge.CannotUseCrystals && CrystalsUsed );
        success &= !( challenge.CannotMuddySaw && SawMuddied );
        success &= !( challenge.MustKillXZappedEnemys > NumZappedEnemiesKilled );
        success &= !( challenge.LevelTimeLimit < TotalLevelTimePassed );
        return success;
    }

    private void OnDamageTaken( int damage )
    {
        DamageTaken = true;
    }

    private void OnCrystalUsed( AbilityEnum ability )
    {
        CrystalsUsed = true;
    }

    private void OnSawMuddied()
    {
        SawMuddied = true;
    }

    private void OnEnemyDied( Enemy en )
    {
        Debug.Assert( en != null );
        if( en.Zapped )
        {
            ++NumZappedEnemiesKilled;
            en.DeathEvent.RemoveListener( OnEnemyDied );
            TrackedEnemies.Remove( en.EnemyID );
        }
    }

    private void OnEnemySpawned( Enemy en )
    {
        en.DeathEvent.AddListener( OnEnemyDied );
        TrackedEnemies.Add( en.EnemyID );
    }
}
