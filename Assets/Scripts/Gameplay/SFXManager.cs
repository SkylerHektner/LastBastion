using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    [SerializeField] private AudioClip EnemyHitSFX;
    [SerializeField] private float EnemyHitSFXCooldown = 0.2f;
    [SerializeField] private int EnemyHitCooldownOverrideCount = 2;

    private AudioSource audioSource;
    private float curEnemyHitSFXCooldown;
    private int curEnemyHitCooldownOverrideCount;
    private int curEnemyHitCooldownOverrideCountNeeded;

    private void Start()
    {
        Instance = this;
        curEnemyHitSFXCooldown = EnemyHitSFXCooldown;
        curEnemyHitCooldownOverrideCount = 0;
        curEnemyHitCooldownOverrideCountNeeded = EnemyHitCooldownOverrideCount;
        transform.position = Vector3.zero;
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if( curEnemyHitSFXCooldown > 0.0f )
            curEnemyHitSFXCooldown -= Time.deltaTime;
    }
    public void PlayEnemyHitSFX()
    {
        curEnemyHitCooldownOverrideCount++;
        if( curEnemyHitSFXCooldown <= 0.0f
            || curEnemyHitCooldownOverrideCount >= curEnemyHitCooldownOverrideCountNeeded )
        { 
            audioSource.PlayOneShot( EnemyHitSFX );

            // kinda confusing - if the sfx was triggered again before the cooldown expired due to enemy hit override count we want it to take 1 more enemy to trigger again that way
            // unless the total cooldown resets. This is to avoid absurd spam by making it harder and harder to bypass the cooldown the more times we bypass it.
            if( curEnemyHitSFXCooldown <= 0.0f )
                curEnemyHitCooldownOverrideCountNeeded = EnemyHitCooldownOverrideCount;
            else
                curEnemyHitCooldownOverrideCountNeeded++;

            curEnemyHitSFXCooldown = EnemyHitSFXCooldown;
            curEnemyHitCooldownOverrideCount = 0;
        }
    }
}
