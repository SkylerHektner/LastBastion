﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BaseHP : MonoBehaviour
{
    public static BaseHP Instance { get; private set; }
    public UnityEvent<int> DamageTakenEvent = new UnityEvent<int>();

    [ReadOnly] public float CurrentHP;
    [ReadOnly] public float CurrentMaxHP;
    public int BaseMaxHP;
    public int MaxHPUpgrade1;
    public int MaxHPUpgrade2;
    public int MaxHPUpgrade3;
    [ReadOnly] public float CurrentOvershield;
    public float MaxOvershield;

    public HpBar CurrentHpBar;
    public HpBar DamageHpBar;
    public HpBar OvershieldBar;
    public Animator OvershieldAnim;
    float ShieldRecoveryDelay;
    float ShieldRechargeRate = .1f;
    public GameObject BrokenGlass;
    public GameObject WoundedGlow;

    float DamageDelay;

    public DeathCanvas DeathCanvas;

    public GameObject BrokenHpBar;

    public GameObject ForceField;
    public GameObject HpExplosions;
    public GameObject DeathExplosions;
    public GameObject AbiltyManager;
    public GameObject SawCanvas;
    public GameObject PauseCanvas;

    // Start is called before the first frame update
    void Start()
    {
        PD.Instance.UnlockFlagChangedEvent.AddListener( OnUpgradeUnlockFlagChanged );
        UpdateMaxHP();
        CurrentHP = CurrentMaxHP;
        CurrentOvershield = MaxOvershield;
        Instance = this;

        if( GameplayManager.Instance.Survival
            && PD.Instance.SurvivalLimboResumeInformation.Active )
        {
            CurrentHP = PD.Instance.SurvivalLimboResumeInformation.Health;
        }
        else if( !GameplayManager.Instance.Survival
            && PD.Instance.CampaignLimboResumeInformation.Active
            && PD.Instance.CampaignLimboResumeInformation.SceneName == SceneManager.GetActiveScene().name )
        {
            CurrentHP = PD.Instance.CampaignLimboResumeInformation.Health;
        }
    }

    private void OnDestroy()
    {
        PD.Instance.UnlockFlagChangedEvent.RemoveListener( OnUpgradeUnlockFlagChanged );
    }

    private void OnUpgradeUnlockFlagChanged( UnlockFlag flag, bool value )
    {
        if( flag == UnlockFlag.BaseHP1
            || flag == UnlockFlag.BaseHP2
            || flag == UnlockFlag.BaseHP3 )
        {
            UpdateMaxHP();
            UpdateHPBar();
        }
    }

    private void UpdateMaxHP()
    {
        float delta = CurrentMaxHP - CurrentHP;

        CurrentMaxHP = BaseMaxHP;
        CurrentMaxHP += PD.Instance.UnlockMap.Get( UnlockFlag.BaseHP1 ) ? MaxHPUpgrade1 : 0;
        CurrentMaxHP += PD.Instance.UnlockMap.Get( UnlockFlag.BaseHP2 ) ? MaxHPUpgrade2 : 0;
        CurrentMaxHP += PD.Instance.UnlockMap.Get( UnlockFlag.BaseHP3 ) ? MaxHPUpgrade3 : 0;

        CurrentHP = CurrentMaxHP - delta;
    }

    [ContextMenu( "KillPlayer" )]
    public void KillPLayer()
    {
        ReduceHP( 1000 );
    }

    public void ReduceHP( int Damage )
    {
        // if somehow the player already won let's just ignore that...
        if( GameplayManager.State == GameplayManager.GameState.Won )
        {
#if UNITY_EDITOR
            Debug.LogWarning( "Player base took damage despite player already having won" );
#endif
            return;
        }

        DamageTakenEvent.Invoke( Damage );

        // if I have overshield, damage that instead
        if( CurrentOvershield > 0 && PD.Instance.UnlockMap.Get( UnlockFlag.BaseOvershield ) )
        {
            CurrentOvershield -= Damage;
            OvershieldAnim.SetBool( "Recovering", true );
            ShieldRecoveryDelay = 5f;
            Component[] Forcefields = ForceField.GetComponentsInChildren<Animator>();
            foreach( Animator Forcefield in Forcefields )
            {
                Forcefield.SetTrigger( "Damaged" );
            }
            if( CurrentOvershield <= 0 )
            {
                CurrentHP = CurrentHP - Mathf.Abs(CurrentOvershield); // if you took enough damage to the shield to go into the negatives, subtract that from the base HP
                CurrentOvershield = 0;
                OvershieldAnim.SetBool( "Broken", true );
                OvershieldAnim.SetBool( "Recovering", false );
                DamageDelay = 1f;
                BrokenGlass.SetActive( true );
                if (CurrentHP <= 0)
                {
                    DeathCanvas.DisplayDeathScreen();
                    CurrentHpBar.gameObject.SetActive(false);
                    DamageHpBar.gameObject.SetActive(false);
                    OvershieldBar.gameObject.SetActive(false);
                    BrokenHpBar.SetActive(true);
                    ForceField.SetActive(false);
                    AbiltyManager.SetActive(false);
                    SawCanvas.SetActive(false);
                    PauseCanvas.SetActive(false);
                    DeathExplosions.SetActive(true);
                    WoundedGlow.SetActive(false);
                    GameplayManager.State = GameplayManager.GameState.Lost;
                    GameplayManager.Instance.ResetLimbo();
                    PD.Instance.TotalFailures.Set(PD.Instance.TotalFailures.Get() + 1);
                }
                UpdateHPBar();
            }
            OvershieldBar.SetSize( CurrentOvershield / MaxOvershield );
        }
        else
        {
            ShieldRecoveryDelay = 8;
            CurrentHP -= Damage;
            if( CurrentHP <= 0 )
            {
                CurrentHP = 0;
            }
            DamageDelay = 1f;
            Component[] Explosions = HpExplosions.GetComponentsInChildren<Animator>();
            foreach( Animator Explosion in Explosions )
            {
                Explosion.SetTrigger( "Damaged" );
            }
            UpdateHPBar();
            if( CurrentHP <= 0 )
            {
                DeathCanvas.DisplayDeathScreen();
                CurrentHpBar.gameObject.SetActive( false );
                DamageHpBar.gameObject.SetActive( false );
                OvershieldBar.gameObject.SetActive( false );
                BrokenHpBar.SetActive( true );
                ForceField.SetActive( false );
                AbiltyManager.SetActive( false );
                SawCanvas.SetActive( false );
                PauseCanvas.SetActive( false );
                DeathExplosions.SetActive( true );
                WoundedGlow.SetActive( false );
                GameplayManager.State = GameplayManager.GameState.Lost;
                GameplayManager.Instance.ResetLimbo();
                PD.Instance.TotalFailures.Set( PD.Instance.TotalFailures.Get() + 1 );
            }
        }
    }

    public void Heal( int Amount )
    {
        CurrentHP = Mathf.Min( CurrentHP + Amount, CurrentMaxHP );
        UpdateHPBar();
        CurrentHpBar.PlayHealAnim();
    }

    private void UpdateHPBar()
    {
        CurrentHpBar.SetSize( CurrentHP / CurrentMaxHP );
        WoundedGlow.SetActive( CurrentHP <= 3 );
    }

    private void FixedUpdate()
    {
        if( DamageDelay > 0 )
        {
            DamageDelay -= Time.deltaTime * GameplayManager.TimeScale;
            if( DamageDelay <= 0 )
            {
                DamageHpBar.SetSize( CurrentHP / CurrentMaxHP );
            }
        }
        // player took damage, begin cooldown before recharging HP
        if( ShieldRecoveryDelay > 0 )
        {
            ShieldRecoveryDelay -= Time.deltaTime * GameplayManager.TimeScale;
            if( ShieldRecoveryDelay <= 0 )
            {
                OvershieldAnim.SetBool( "Recovering", false );
                OvershieldAnim.GetComponent<VolumeController>().PlayMySound();
            }
        }
        else if( CurrentOvershield < MaxOvershield && ShieldRecoveryDelay <= 0 && CurrentHP > 0) // don't recharge if dead
        {
            CurrentOvershield += ShieldRechargeRate * GameplayManager.TimeScale;
            BrokenGlass.SetActive( false );
            OvershieldBar.SetSize( CurrentOvershield / MaxOvershield );
            OvershieldAnim.SetBool("Broken", false);
            //OvershieldAnim.GetComponent<VolumeController>().PlayMySound(); // called when the overshield fully recharges
        }
        if( CurrentOvershield > MaxOvershield )
        {
            CurrentOvershield = MaxOvershield;
        }
    }
}


public class PlayerRegistry
{
    public static PlayerRegistry Instance { get; private set; }

    public List<GameObject> players;

    public void RegisterPlayer( GameObject p )
    {
        players.Add( p );
    }

    public void DeregisterPlayer( GameObject p )
    {
        players.Remove( p );
    }
}
