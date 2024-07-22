using UnityEngine;

[CreateAssetMenu(menuName = "Song Data")]
public class SongData : ScriptableObject
{
    public string title;
    public string artist;
    public int bpm = 120;

    public AudioClip song;

    public GameObject beatmap;

    public string savePath;
}
