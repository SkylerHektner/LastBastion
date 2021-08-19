using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// gameplay manager responsible or maintaining one of these
public class ChallengesTracker
{
    public bool DamageTaken { get; private set; } = false;
    public int CrystalsUsed { get; private set; } = 0;
    public bool SawMuddied { get; private set; } = false;
    public int NumZappedEnemiesKilled { get; private set; } = 0;
    public int NumEnemiesKilledWithFire { get; private set; } = 0;
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
        if( GameplayManager.State == GameplayManager.GameState.Active )
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
        bool success = true;
        success &= !( challenge.CannotTakeDamage && DamageTaken );
        success &= !( challenge.CannotUseCrystals && ( CrystalsUsed > 0 ) );
        success &= !( challenge.CannotMuddySaw && SawMuddied );
        success &= !( challenge.MustKillXZappedEnemys > NumZappedEnemiesKilled );
        success &= !( challenge.MustKillXEnemysWithFire > NumEnemiesKilledWithFire );
        success &= !( challenge.MustUseXCrystals > CrystalsUsed );
        success &= !( challenge.LevelTimeLimit < TotalLevelTimePassed );
        return success;
    }

    private void OnDamageTaken( int damage )
    {
        DamageTaken = true;
    }

    private void OnCrystalUsed( AbilityEnum ability )
    {
        ++CrystalsUsed;
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
        }

        if( en.DeathSource == DamageSource.FlamingSaw
            || en.DeathSource == DamageSource.Typhoon
            || en.DeathSource == DamageSource.TyphoonFlamingCorpse )
        {
            ++NumEnemiesKilledWithFire;
        }

        TrackedEnemies.Remove( en.EnemyID );
        en.DeathEvent.RemoveListener( OnEnemyDied );
    }

    private void OnEnemySpawned( Enemy en )
    {
        en.DeathEvent.AddListener( OnEnemyDied );
        TrackedEnemies.Add( en.EnemyID );
    }
}
