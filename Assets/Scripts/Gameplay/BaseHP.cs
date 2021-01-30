using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHP : MonoBehaviour
{
    public static BaseHP Instance { get; private set; }

    public float CurrentHP;
    public float MaxHP;
    public float CurrentOvershield;
    public float MaxOvershield;

    public HpBar CurrentHpBar;
    public HpBar DamageHpBar;
    public HpBar OvershieldBar;
    public Animator OvershieldAnim;
    float ShieldRecoveryDelay;
    float ShieldRechargeRate = .1f;
    public GameObject BrokenGlass;

    float DamageDelay;

    public DeathCanvas DeathCanvas;

    public GameObject HPcanvas;
    public GameObject ForceField;
    public GameObject DeathExplosions;
    public GameObject AbiltyManager;
    public GameObject SawCanvas;
    public GameObject PauseCanvas;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHP = MaxHP;
        CurrentOvershield = MaxOvershield;
        Instance = this;
    }


    public void ReduceHP(int Damage)
    {
        // if I have overshield, damage that instead
        if (CurrentOvershield > 0)
        {
            CurrentOvershield -= Damage;
            OvershieldAnim.SetBool("Recovering", true);
            ShieldRecoveryDelay = 5f;
            if (CurrentOvershield <= 0)
            {
                CurrentOvershield = 0;
                OvershieldAnim.SetBool("Broken", true);
                OvershieldAnim.SetBool("Recovering", false);
                BrokenGlass.SetActive(true);
            }
            OvershieldBar.SetSize(CurrentOvershield / MaxOvershield);
        }
        else
        {
            ShieldRecoveryDelay = 8;
            CurrentHP -= Damage;
            CurrentHpBar.SetSize(CurrentHP / MaxHP);
            DamageDelay = 1f;
            Component[] Forcefields = GetComponentsInChildren<Animator>();
            foreach (Animator Forcefield in Forcefields)
            {
                Forcefield.SetTrigger("Damaged");
            }
            if (CurrentHP <= 0)
            {
                DeathCanvas.DisplayDeathScreen();
                HPcanvas.SetActive(false);
                ForceField.SetActive(false);
                AbiltyManager.SetActive(false);
                SawCanvas.SetActive(false);
                PauseCanvas.SetActive(false);
                DeathExplosions.SetActive(true);
            }
        }


    }

    private void FixedUpdate()
    {
        if (DamageDelay > 0)
        {
            DamageDelay -= Time.smoothDeltaTime;
            if (DamageDelay <= 0)
            {
                DamageHpBar.SetSize(CurrentHP / MaxHP);
            }
        }
        // player took damage, begin cooldown before recharging HP
        if (ShieldRecoveryDelay > 0)
        {
            ShieldRecoveryDelay -= Time.smoothDeltaTime;
            if (ShieldRecoveryDelay <= 0)
            {
                OvershieldAnim.SetBool("Recovering", false);
            }
        }
        else if (CurrentOvershield < MaxOvershield && ShieldRecoveryDelay <= 0)
        {
            CurrentOvershield += ShieldRechargeRate;
            BrokenGlass.SetActive(false);
            OvershieldBar.SetSize(CurrentOvershield / MaxOvershield);
            OvershieldAnim.SetBool("Broken", false);
        }
        if (CurrentOvershield > MaxOvershield)
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

    public void DeregisterPlayer(GameObject p)
    {
        players.Remove( p );
    }
}
