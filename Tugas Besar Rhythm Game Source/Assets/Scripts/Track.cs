using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    float trackPosition = 0f;

    SongData songData;

    public AudioSource songPlayer;

    void Awake()
    {
        songData = GameData.selectedSong;
        songPlayer.clip = songData.song;

        // for (int i = 0; i < transform.childCount; i++)
        //     if (transform.GetChild(i).name != "Background")
        //         transform.GetChild(i).transform.rotation = Camera.main.transform.rotation;
    }

    // Update track position
    void Update()
    {
        trackPosition = (songPlayer.time / 60f) * songData.bpm;
    }

    // Get track position
    public Vector2 GetTrackPosition()
    {
        return Vector2.down * trackPosition;
    }

    // Play and start scrolling
    public void Play()
    {
        songPlayer.Play();
    }

    public bool isPlaying()
    {
        return songPlayer.isPlaying;
    }

    // Pause and stop scrolling
    public void Stop()
    {
        songPlayer.Pause();
    }
}
