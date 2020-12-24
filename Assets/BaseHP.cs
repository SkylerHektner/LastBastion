using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHP : MonoBehaviour
{
    public float CurrentHP;
    public float MaxHP;

    public HpBar CurrentHpBar;
    public HpBar DamageHpBar;

    float DamageDelay;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHP = MaxHP;
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
