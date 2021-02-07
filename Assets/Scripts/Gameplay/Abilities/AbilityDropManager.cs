using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AbilityDropManager : MonoBehaviour
{
    [SerializeField] int AbilityDropTarget = 100;
    [SerializeField] int AbilityDropRandomMax = 110;
    [SerializeField] float AbilitySelectionExpansionFactor = 4.0f;
    [SerializeField] AbilityDrop ChainLightningDrop;
    [SerializeField] AbilityDrop TemporalAnomalyDrop;
    [SerializeField] AbilityDrop SawmageddonDrop;
    [SerializeField] AbilityDrop TyphoonDrop;

    private int cur_drop_bias = 0;
    private List<float> powerup_drop_biases = new List<float>();

    public void Start()
    {
        SpawnManager.Instance.EnemySpawnedEvent.AddListener( OnEnemySpawned );
        for( int x = 0; x < (int)AbilityEnum.NUM_ABILITIES; ++x )
        {
            powerup_drop_biases.Add( 1.0f );
        }
    }

    private void OnEnemySpawned( Enemy enemy )
    {
        enemy.OnDeath.AddListener( OnEnemyDied );
    }

    private void OnEnemyDied( long ID )
    {
        Enemy dead_enemy = SpawnManager.Instance.TryGetEnemyByID( ID );
        if( dead_enemy )
        {
            dead_enemy.OnDeath.RemoveListener( OnEnemyDied );
            Debug.Log( dead_enemy.PowerupDropValue );
            for( int x = 0; x < dead_enemy.PowerupDropValue; ++x )
            {
                ++cur_drop_bias;
                // TODO: Allow multiple powerups to drop from one enemy?
                if( TryDropPowerup( dead_enemy.transform.position ) )
                    break;
            }
        }
        else
        {
            Debug.LogError( "ERROR: dead enemy not found in Ability Drop Manager" );
        }
    }

    private bool TryDropPowerup( Vector3 pos )
    {
        int seed = Random.Range( 0, AbilityDropRandomMax );
        seed += cur_drop_bias;
        if( seed >= AbilityDropTarget )
        {
            // pick an ability
            float min_roll = float.MaxValue;
            int selected_index = 0;
            for( int x = 0; x < (int)AbilityEnum.NUM_ABILITIES; ++x )
            {
                float roll = Random.Range( 0.0f, powerup_drop_biases[x] );
                if( roll < min_roll )
                {
                    min_roll = roll;
                    selected_index = x;
                }
            }

            // apply new bias
            powerup_drop_biases[selected_index] *= AbilitySelectionExpansionFactor;

            // reduce drop biases
            if( powerup_drop_biases.TrueForAll( ( float f ) => f < 1.0f ) )
                for( int x = 0; x < powerup_drop_biases.Count; ++x )
                    powerup_drop_biases[x] *= 0.5f;
            cur_drop_bias = 0;

            // drop ability
            AbilityEnum ability = (AbilityEnum)selected_index;

            AbilityDrop ab = null;
            switch( ability )
            {
                case AbilityEnum.TemporalAnomaly:
                    if( PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.Anomaly ) )
                        ab = TemporalAnomalyDrop;
                    break;
                case AbilityEnum.ChainLightning:
                    if( PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.ChainLightning ) )
                        ab = ChainLightningDrop;
                    break;
                case AbilityEnum.Typhoon:
                    if( PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.Typhoon ) )
                        ab = TyphoonDrop;
                    break;
                case AbilityEnum.Sawmageddon:
                    if( PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.Sawmageddon ) )
                        ab = SawmageddonDrop;
                    break;
            }
            if( ab )
            {
                Instantiate( ab ).transform.position = pos;
                return true;
            }
        }
        return false;
    }
}
