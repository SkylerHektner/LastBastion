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
    public bool PitchIncreaseOnAwake;
    public bool NeutralizePitchOnDestroy;
    public static float RecentPitch = 1f;

    private float default_volume;

    // Start is called before the first frame update
    void Awake()
    {
        SoundSource = gameObject.GetComponent<AudioSource>();
        DefaultSound = SoundSource.clip; // save for reference
        if (PitchIncreaseOnAwake)
        {
            RaisePitch();
        }
    }

    private void OnDestroy() // called when anomaly saws despawn
    {
        if (NeutralizePitchOnDestroy)
        {
            NeutralizePitch();
        }
    }
    private void Start()
    {
        SoundSource = gameObject.GetComponent<AudioSource>();
        default_volume = SoundSource.volume;
        //Debug.Log( default_volume );
    }

    // Update is called once per frame
    void Update()
    {
        if (MusicTrack)
        {
            SoundSource.volume = default_volume * PD.Instance.StoredMusicVolume.Get();
        }
        else
        {
            SoundSource.volume = default_volume * PD.Instance.StoredSFXVolume.Get(); // set elsewhere
        }
    }

    public void RandomizePitch()
    {
        float MaxValue = 1.5f;
        float MinValue = .8f;
        SoundSource.pitch = Random.Range(MinValue, MaxValue);
    }

    public void RaisePitch() // primarily used with Temporal Anomaly
    {
        SoundSource.pitch = RecentPitch; // pull last tone
        float RaiseAmount = .1f; // increase it
        SoundSource.pitch = SoundSource.pitch + RaiseAmount; // set it
        RecentPitch = SoundSource.pitch; // remember for next guy
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
    public void NeutralizePitch() // called when Anonaly gets turned on for the first time from neutral state
    {
        RecentPitch = 1f;
    }
    public void UpdateAllLowerPitches()
    {
        if (SoundSource.pitch < RecentPitch)
        {
            SoundSource.pitch = RecentPitch;
        }
    }

}
