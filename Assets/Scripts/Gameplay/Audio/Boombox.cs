using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boombox : MonoBehaviour
{
    AudioSource CurrentSoundTrack;
    public bool DeathMusic;
    public bool VictoryMusic;
    public bool SurvivalTrack;
    public AudioClip MyClip;


    public void SwapTrack(AudioClip NextSong) // changes out the current music track in the spectator object
    {
        CurrentSoundTrack = GameObject.FindGameObjectWithTag("Spectator").GetComponent<AudioSource>();
        CurrentSoundTrack.clip = null;
        CurrentSoundTrack.clip = NextSong;
        CurrentSoundTrack.Play();
        Debug.Log("swapping tracks to " + NextSong.ToString());
    }

    public void OnEnable() // when loading a level,find the level soundtrack and update the main boombox on the spectator
    {
        CurrentSoundTrack = GameObject.FindGameObjectWithTag("Spectator").GetComponent<AudioSource>();
        if (DeathMusic || VictoryMusic)
        {
            CurrentSoundTrack.clip = null;
        }
        //AudioSource LevelMusicPlayer = gameObject.GetComponent<AudioSource>();
        SwapTrack(MyClip);

    }
}
