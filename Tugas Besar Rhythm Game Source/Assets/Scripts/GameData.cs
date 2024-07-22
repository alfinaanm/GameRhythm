using UnityEngine;

public class GameData : MonoBehaviour
{
    // Level Selection
    public static SongData selectedSong;

    // Scoring
    public static int great;
    public static int good;
    public static int poor;
    public static int bad;
    public static int score;

    public static int combo;
    public static int maxCombo;

    public static float accuracy
    {
        get => (float)(great + good) / (float)(great + good + poor + bad);
    }

    public static void Reset()
    {
        great = good = poor = bad = score = combo = maxCombo = 0;
    }

    public static void UpdateCombo() {
        if (combo > maxCombo) maxCombo = combo;
    }
}
