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

        if (SkullyBossHP > 75)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Healthy;
        }
        else if (SkullyBossHP <= 75 && SkullyBossHP > 50)
        {
            LaughSFX.SetActive(true);
            gameObject.GetComponent<SpriteRenderer>().sprite = Wounded;
        }
        else if (SkullyBossHP <= 50 && SkullyBossHP > 25)
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


}
