using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        PD.Instance.UpgradeFlagChangedEvent.AddListener( OnUpgradeUnlockFlagChanged );
        UpdateMaxHP();
        CurrentHP = CurrentMaxHP;
        CurrentOvershield = MaxOvershield;
        Instance = this;
    }

    private void OnDestroy()
    {
        PD.Instance.UpgradeFlagChangedEvent.RemoveListener( OnUpgradeUnlockFlagChanged );
    }


    private void OnUpgradeUnlockFlagChanged( PD.UpgradeFlags flag, bool value )
    {
        if( flag == PD.UpgradeFlags.BaseHP1
            || flag == PD.UpgradeFlags.BaseHP2
            || flag == PD.UpgradeFlags.BaseHP3 )
        {
            UpdateMaxHP();
        }
    }

    private void UpdateMaxHP()
    {
        CurrentMaxHP = BaseMaxHP;
        CurrentMaxHP += PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.BaseHP1, GameplayManager.Instance.Survival ) ? MaxHPUpgrade1 : 0;
        CurrentMaxHP += PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.BaseHP2, GameplayManager.Instance.Survival ) ? MaxHPUpgrade2 : 0;
        CurrentMaxHP += PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.BaseHP3, GameplayManager.Instance.Survival ) ? MaxHPUpgrade3 : 0;
    }

    [ContextMenu( "KillPlayer" )]
    public void KillPLayer()
    {
        ReduceHP( 1000 );
    }

    public void ReduceHP( int Damage )
    {
        // if somehow the player already won let's just ignore that...
        if( GameplayManager.PlayerWinState == GameplayManager.PlayerState.Won )
        {
#if UNITY_EDITOR
            Debug.LogWarning( "Player base took damage despite player already having won" );
#endif
            return;
        }

        DamageTakenEvent.Invoke( Damage );

        // if I have overshield, damage that instead
        if( CurrentOvershield > 0 && PD.Instance.UpgradeUnlockMap.GetUnlock( PD.UpgradeFlags.BaseOvershield, GameplayManager.Instance.Survival ) )
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
                CurrentOvershield = 0;
                OvershieldAnim.SetBool( "Broken", true );
                OvershieldAnim.SetBool( "Recovering", false );
                BrokenGlass.SetActive( true );
            }
            OvershieldBar.SetSize( CurrentOvershield / MaxOvershield );
        }
        else
        {
            ShieldRecoveryDelay = 8;
            CurrentHP -= Damage;
            if (CurrentHP <= 0)
            {
                CurrentHP = 0;
            }
            CurrentHpBar.SetSize( CurrentHP / CurrentMaxHP );
            DamageDelay = 1f;
            Component[] Explosions = HpExplosions.GetComponentsInChildren<Animator>();
            foreach( Animator Explosion in Explosions )
            {
                Explosion.SetTrigger( "Damaged" );
            }
            WoundedGlow.SetActive( CurrentHP <= 3 );
            if( CurrentHP <= 0 )
            {
                DeathCanvas.DisplayDeathScreen();
                CurrentHpBar.gameObject.SetActive(false);
                DamageHpBar.gameObject.SetActive(false);
                OvershieldBar.gameObject.SetActive(false);
                BrokenHpBar.SetActive( true );
                ForceField.SetActive( false );
                AbiltyManager.SetActive( false );
                SawCanvas.SetActive( false );
                PauseCanvas.SetActive( false );
                DeathExplosions.SetActive( true );
                WoundedGlow.SetActive( false );
                GameplayManager.PlayerWinState = GameplayManager.PlayerState.Lost;
            }
        }
    }

    public void Heal(int Amount)
    {
        CurrentHP = Mathf.Min( CurrentHP + Amount, CurrentMaxHP );
        CurrentHpBar.SetSize( CurrentHP / CurrentMaxHP );
        CurrentHpBar.PlayHealAnim();
        WoundedGlow.SetActive( CurrentHP <= 3 ); // I like this :)
        // play cool effect here
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
            }
        }
        else if( CurrentOvershield < MaxOvershield && ShieldRecoveryDelay <= 0 )
        {
            CurrentOvershield += ShieldRechargeRate * GameplayManager.TimeScale;
            BrokenGlass.SetActive( false );
            OvershieldBar.SetSize( CurrentOvershield / MaxOvershield );
            OvershieldAnim.SetBool( "Broken", false );
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
