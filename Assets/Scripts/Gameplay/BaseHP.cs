﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHP : MonoBehaviour
{
    public static BaseHP Instance { get; private set; }

    public float CurrentHP;
    public float MaxHP;

    public HpBar CurrentHpBar;
    public HpBar DamageHpBar;

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
        Instance = this;
    }


    public void ReduceHP(int Damage)
    {
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
