using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeController : MonoBehaviour
{
    /// Must be attached to a gameobject with an audio source component
    AudioSource SoundSource;
    public bool MusicTrack;
    public AudioClip AlternateSound;
    public AudioClip ThirdSound;
    AudioClip DefaultSound;
    public bool PitchIncreaseOnAwake;
    public bool NeutralizePitchOnDestroy;
    public static float RecentPitch = 1f;
    public List<AudioClip> RandomlyPayedEffects;
    bool Toggled = false;
    float PitchWait;

    float last_frame_volume = -1.0f;

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

        float volume = default_volume * ( MusicTrack ? PD.Instance.StoredMusicVolume.Get() : PD.Instance.StoredSFXVolume.Get() );
        SoundSource.mute = volume == 0.0f;
        SoundSource.volume = volume;
        last_frame_volume = volume;
    }

    // Update is called once per frame
    void Update()
    {
        float volume = default_volume *  ( MusicTrack ? PD.Instance.StoredMusicVolume.Get() : PD.Instance.StoredSFXVolume.Get() );
        if(volume != last_frame_volume)
        {
            SoundSource.mute = volume == 0.0f;
            SoundSource.volume = volume;
            last_frame_volume = volume;
        }
    }
    private void FixedUpdate()
    {
        if (PitchWait > 0)
        {
            PitchWait -= Time.smoothDeltaTime;
        }
        else
        {
            PitchWait = 0f;
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
    public void PlayMyThirdSound()
    {
        SoundSource.clip = ThirdSound;
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
    [ContextMenu("PlayFromRandomPool")]
    public void PlayFromRandomPool()
    {
        int randInt = Random.Range(0, RandomlyPayedEffects.Count); // picks a random number from 0 to the count of the list
        SoundSource.clip = RandomlyPayedEffects[randInt];
        SoundSource.Play();
    }

    public void ToggleBetweenPitch()
    {
        if (Toggled) // quick fire
        {
            if (PitchWait > 0)
            {
                SoundSource.pitch = .9f;
            }
            else 
            {
                SoundSource.pitch = 1.1f;
                PitchWait = .9f;
            }
        }
        else  // wait a second before firing, so don't repeat the sound
        {
            SoundSource.pitch = 1.1f;
            PitchWait = .9f;
        }
        Toggled = !Toggled;

    }

}
