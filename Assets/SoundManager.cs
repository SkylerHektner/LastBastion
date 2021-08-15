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
        if (PlayerPrefs.GetFloat("GameBegun") == 0)
        {
            PlayerPrefs.SetFloat("GameBegun", 1);
            RevertToDefault();
        }
        else // if the preference was changed on a different session, remember it
        {
            SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
    }

    public void RevertToDefault() // sets both volumes to default levels
    {
        InitializeSound();
        Spectator.GameMusic.volume = DefaultMusicVolume;
        MusicSlider.value = DefaultMusicVolume;
        PlayerPrefs.SetFloat("MusicVolume", DefaultMusicVolume); // updates preferences with change

        SFXSlider.value = DefaultSFXVolume;
        PlayerPrefs.SetFloat("SFXVolume", DefaultSFXVolume);

    }

    public void EditSFXVolume() // called each time the slider value changes
    {
        PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
        if (SFXClicks.gameObject.activeInHierarchy)
        {
            SFXClicks.Play();
        }
    }

    public void EditMusicVolume()
    {
        Spectator.GameMusic.volume = MusicSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", Spectator.GameMusic.volume); // updates preferences with change
        if (MusicClicks.gameObject.activeInHierarchy)
        {
            MusicClicks.Play();
        }
    }
}
