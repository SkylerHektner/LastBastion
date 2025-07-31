using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boombox : MonoBehaviour
{
    AudioSource CurrentSoundTrack;
    public bool DeathMusic;
    public bool VictoryMusic;
    public bool SurvivalTrack;
    public AudioClip MyClip;
    public bool DiscoverTrackOnStart;
    public string TrackID;

    public AudioClip GetCurrentTrack()
    {
        CurrentSoundTrack = GameObject.FindGameObjectWithTag("Spectator").GetComponent<AudioSource>();
        return CurrentSoundTrack.clip;
    }

    public void SwapTrack(AudioClip NextSong) // changes out the current music track in the spectator object
    {
        if (NextSong != null)
        {
            CurrentSoundTrack = GameObject.FindGameObjectWithTag("Spectator").GetComponent<AudioSource>();
            CurrentSoundTrack.clip = null;
            CurrentSoundTrack.clip = NextSong;
            CurrentSoundTrack.Play();
            Debug.Log("swapping tracks to " + NextSong.ToString());
        }

    }

    public void DelayedSwapTrack() // changes out the current music track in the spectator object, but delays it by a value
    {
        CurrentSoundTrack = GameObject.FindGameObjectWithTag("Spectator").GetComponent<AudioSource>();
        CurrentSoundTrack.clip = null;
        CurrentSoundTrack.clip = MyClip;
        CurrentSoundTrack.Play();
        Debug.Log("swapping tracks to " + MyClip.ToString());
    }

    public void PlayDelayedMusicTrack(float PlayDelay)
    {
      
        Invoke("DelayedSwapTrack", PlayDelay);
    }
    public void EmptyTrack()
    {
        CurrentSoundTrack = GameObject.FindGameObjectWithTag("Spectator").GetComponent<AudioSource>();
        CurrentSoundTrack.clip = null;
        CurrentSoundTrack.Play();
    }

    public void OnEnable() // when loading a level,find the level soundtrack and update the main boombox on the spectator
    {
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            CurrentSoundTrack = GameObject.FindGameObjectWithTag("Spectator").GetComponent<AudioSource>();
            if (DeathMusic || VictoryMusic)
            {
                CurrentSoundTrack.clip = null;
            }
            //AudioSource LevelMusicPlayer = gameObject.GetComponent<AudioSource>();
            SwapTrack(MyClip);
        }
        if (DiscoverTrackOnStart)
        {
            PD.Instance.LevelCompletionMap.SetLevelCompletion(TrackID, true);
        }


    }


}
