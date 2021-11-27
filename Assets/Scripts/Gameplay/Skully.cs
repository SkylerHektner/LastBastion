using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skully : MonoBehaviour
{
    public Sprite Healthy;
    public Sprite Wounded;
    public Sprite HalfHP;
    public Sprite Critical;
    int SkullyBossHP;
    public GameObject LaughSFX;
    public GameObject HalfwaySFX;
    public GameObject WoundedSFX;
    public Animator InnerGlow;

    // Update is called once per frame
    void FixedUpdate()
    {
        SkullyBossHP = gameObject.GetComponent<Shaman>().CurrentHealth;
        gameObject.GetComponent<Animator>().SetFloat("CurrentHP", SkullyBossHP);

        if (SkullyBossHP > 175)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Healthy;
        }
        else if (SkullyBossHP <= 175 && SkullyBossHP > 150)
        {
            LaughSFX.SetActive(true);
            gameObject.GetComponent<SpriteRenderer>().sprite = Wounded;
        }
        else if (SkullyBossHP <= 100 && SkullyBossHP > 25)
        {
            HalfwaySFX.SetActive(true);
            InnerGlow.SetBool("Half", true);
            gameObject.GetComponent<SpriteRenderer>().sprite = HalfHP;
        }
        else if (SkullyBossHP <= 25 && SkullyBossHP >= 0)
        {
            WoundedSFX.SetActive(true);
            InnerGlow.SetBool("Half", false);
            InnerGlow.SetBool("Critical", true);
            gameObject.GetComponent<SpriteRenderer>().sprite = Critical;
        }
    }

    public void TriggerVaultTransition() // boss phase part 2
    {
        Animator MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();
        MainCamera.SetTrigger("ToggleVault");

        Animator VaultDoor = GameObject.FindGameObjectWithTag("VaultDoor").GetComponent<Animator>();
        VaultDoor.SetTrigger("VaultDoor");

        gameObject.GetComponent<Shaman>().DecreaseSpawnRate();
    }

    public void NeutralSpawnRate()
    {
        gameObject.GetComponent<Shaman>().IncreaseSpawnRate();
    }
    public void MaxspawnRate()
    {
        gameObject.GetComponent<Shaman>().MaximizeSpawnRate();
    }
    public void ExtremeSlowSpawnRate()
    {
        gameObject.GetComponent<Shaman>().ExtremeSlowSpawnRate();
    }


}
