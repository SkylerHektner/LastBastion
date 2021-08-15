using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SoundManager : MonoBehaviour
{
    public GameObject MusicVolume;
    public GameObject SFXVolume;
    Slider MusicSlider;
    Slider SFXSlider;
    float DefaultMusicVolume = .4f;
    float DefaultSFXVolume = .6f;

    public AudioSource SFXClicks;
    public AudioSource MusicClicks;


    private void Start()
    {
        InitializeSound();
    }

    public void InitializeSound() // called at the start
    {
        MusicSlider = MusicVolume.GetComponent<Slider>();

        SFXSlider = SFXVolume.GetComponent<Slider>();
        if (PD.Instance.GameBegun.Get() == 0) // if the user has never booted up the game until now, use default noise levels
        {
            PD.Instance.GameBegun.Set(1);
            RevertToDefault();
        }
        else // if the preference was changed on a different session, remember it
        {
            SFXSlider.value = PD.Instance.StoredSFXVolume.Get();
            MusicSlider.value = PD.Instance.StoredMusicVolume.Get();
        }
    }

    public void RevertToDefault() // sets both volumes to default levels
    {
        InitializeSound();
        Spectator.GameMusic.volume = DefaultMusicVolume;
        MusicSlider.value = DefaultMusicVolume;
        //PlayerPrefs.SetFloat("MusicVolume", DefaultMusicVolume); // updates preferences with change
        PD.Instance.StoredMusicVolume.Set(DefaultMusicVolume);

        SFXSlider.value = DefaultSFXVolume;
        //PlayerPrefs.SetFloat("SFXVolume", DefaultSFXVolume);
        PD.Instance.StoredSFXVolume.Set(DefaultSFXVolume);

    }

    public void EditSFXVolume() // called each time the slider value changes
    {
        PD.Instance.StoredSFXVolume.Set(SFXSlider.value);
        //PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
        if (SFXClicks.gameObject.activeInHierarchy)
        {
            SFXClicks.Play();
        }
    }

    public void EditMusicVolume()
    {
        Spectator.GameMusic.volume = MusicSlider.value;
        PD.Instance.StoredMusicVolume.Set(Spectator.GameMusic.volume);
        //PlayerPrefs.SetFloat("MusicVolume", Spectator.GameMusic.volume); // updates preferences with change
        if (MusicClicks.gameObject.activeInHierarchy)
        {
            MusicClicks.Play();
        }
    }
}
