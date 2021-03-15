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
            gameObject.GetComponent<SpriteRenderer>().sprite = Wounded;
        }
        else if (SkullyBossHP <= 50 && SkullyBossHP > 25)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = HalfHP;
        }
        else if (SkullyBossHP <= 25 && SkullyBossHP >= 0)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Critical;
        }
    }
}
