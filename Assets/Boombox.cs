using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boombox : MonoBehaviour
{
    AudioSource CurrentSoundTrack;


    public void SwapTrack(AudioClip NextSong) // changes out the current music track
    {
        Spectator.GameMusic = gameObject.GetComponent<AudioSource>();
        Spectator.GameMusic.clip = NextSong;
        Spectator.GameMusic.Play();
        Debug.Log("swapping");
    }

    public void Awake() // when loading a level,find the level soundtrack and update the main boombox
    {
        AudioSource LevelMusicPlayer = gameObject.GetComponent<AudioSource>();
        SwapTrack(LevelMusicPlayer.clip);
    }

}
