using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeController : MonoBehaviour
{
    /// Must be attached to a gameobject with an audio source component
    AudioSource SoundSource;
    public bool MusicTrack;
    public AudioClip AlternateSound;
    AudioClip DefaultSound;

    // Start is called before the first frame update
    void Awake()
    {
        SoundSource = gameObject.GetComponent<AudioSource>();
        DefaultSound = SoundSource.clip; // save for reference
    }

    // Update is called once per frame
    void Update()
    {
        if (MusicTrack)
        {
            SoundSource.volume = PD.Instance.StoredMusicVolume.Get();
        }
        else
        {
            SoundSource.volume = PD.Instance.StoredSFXVolume.Get(); // set elsewhere
        }
    }

    public void RandomizePitch()
    {
        float MaxValue = 1.5f;
        float MinValue = .8f;
        SoundSource.pitch = Random.Range(MinValue, MaxValue);
    }

    public void PlayMySound()
    {
        SoundSource.clip = DefaultSound;
        SoundSource.Play();
    }
    public void PlayMyAlternateSound()
    {
        SoundSource.clip = AlternateSound;
        SoundSource.Play();
    }


}
