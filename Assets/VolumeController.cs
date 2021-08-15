using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeController : MonoBehaviour
{
    /// Must be attached to a gameobject with an audio source component
    AudioSource SoundSource;
    public bool MusicTrack;

    // Start is called before the first frame update
    void Awake()
    {
        SoundSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MusicTrack)
        {
            SoundSource.volume = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            SoundSource.volume = PlayerPrefs.GetFloat("SFXVolume"); // set elsewhere
        }
    }


}
