using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boombox : MonoBehaviour
{
    AudioSource CurrentSoundTrack;
    public bool DeathMusic;
    public bool VictoryMusic;


    public void SwapTrack(AudioClip NextSong) // changes out the current music track in the spectator object
    {
        Spectator.GameMusic = gameObject.GetComponent<AudioSource>();
        Spectator.GameMusic.clip = NextSong;
        Spectator.GameMusic.Play();
        Debug.Log("swapping");
    }

    public void Awake() // when loading a level,find the level soundtrack and update the main boombox
    {
        if (DeathMusic || VictoryMusic)
        {
            Spectator.GameMusic.clip = null;
        }
        AudioSource LevelMusicPlayer = gameObject.GetComponent<AudioSource>();
        SwapTrack(LevelMusicPlayer.clip);
    }

}
