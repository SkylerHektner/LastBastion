﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningAbility : Ability
{
    public static readonly float TIME_STEP = 0.1f;

    struct PendingZap
    {
        public float time;
        public Vector3 position;
        public Vector3 last_position;
        public long EnemyID;
    }

    private List<PendingZap> pending_zaps = new List<PendingZap>();
    private float cur_time = 0.0f;
    private int cur_zap_index = 0;

    public override void Start()
    {
        base.Start();

        List<Enemy> enemies_on_field = SpawnManager.Instance.AllSpawnedEnemies;
        if( enemies_on_field.Count == 0 )
        {
            Finish();
            return;
        }

        enemies_on_field.Sort( ( a, b ) => a.gameObject.transform.position.y.CompareTo( b.transform.position.y ) );
        pending_zaps.Add(new PendingZap()
        {
            time = 0.0f,
            position = enemies_on_field[0].transform.position,
            last_position = AM.BaseCenter,
            EnemyID = enemies_on_field[0].EnemyID,
        } );

        int t = 0;
        int h = 0;
        float time = TIME_STEP;
        while(h < enemies_on_field.Count)
        {
            ++h;
            if( h >= enemies_on_field.Count )
                break;
            pending_zaps.Add( new PendingZap()
            {
                time = time,
                position = enemies_on_field[h].transform.position,
                last_position = enemies_on_field[t].transform.position,
                EnemyID = enemies_on_field[h].EnemyID,
            } );

            ++h;
            if( h >= enemies_on_field.Count )
                break;
            pending_zaps.Add( new PendingZap()
            {
                time = time,
                position = enemies_on_field[h].transform.position,
                last_position = enemies_on_field[t].transform.position,
                EnemyID = enemies_on_field[h].EnemyID,
            } );

            ++t;
            time += TIME_STEP;
        }
    }

    public override void Update( float delta_time )
    {
        base.Update( delta_time );
        cur_time += delta_time * GameplayManager.GamePlayTimeScale;

        while(pending_zaps[cur_zap_index].time <= cur_time)
        {
            DoZap( pending_zaps[cur_zap_index] );
            cur_zap_index++;

            if(cur_zap_index == pending_zaps.Count)
            {
                Finish();
                return;
            }
        }
    }

    private void DoZap(PendingZap zap)
    {

    }
}


public class ChainLightningAbilityData : ScriptableObject
{

}