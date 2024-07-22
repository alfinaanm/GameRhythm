using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongItem : MonoBehaviour
{
    public SongData songData;
    public Text titleText;
    public Text artistText;
    public Text highScoreText;
 
    void Start()
    {
        titleText.text = songData.title.ToUpper();
        artistText.text = songData.artist;
        highScoreText.text = SaveSystem.Load(songData.savePath).ToString();
    }

    void HighScore(int value) {
        highScoreText.text = value.ToString();
    }
}
