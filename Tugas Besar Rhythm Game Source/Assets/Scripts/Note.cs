using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Note : MonoBehaviour
{
    [SerializeField] Sprite[] noteTextures = new Sprite[4];
    [SerializeField] SpriteRenderer texture;

    void Awake() {
        // Texture Assignments
        if (transform.position.x < -2f) texture.sprite = noteTextures[0];
        else if (transform.position.x < 0.5f) texture.sprite = noteTextures[1];
        else if (transform.position.x < 1f) texture.sprite = noteTextures[2];
        else texture.sprite = noteTextures[3];
    }

    public int Judge(float threshold, float distance)
    {
        float great = threshold / 4, good = threshold / 2;
        Destroy(gameObject);

        if (distance >= 0f && distance < great)
        {
            GameData.great++;
            int score = Mathf.RoundToInt(math.remap(0, threshold, 1, 0, distance) * 10 * (10 + GameData.combo++));
            GameData.UpdateCombo();

            return score;
        }

        else if (distance >= great && distance < good)
        {
            GameData.good++;
            int score = Mathf.RoundToInt(math.remap(0, threshold, 1, 0, distance) * 10 * (10 + GameData.combo++));
            GameData.UpdateCombo();

            return score;
        }

        else if (distance >= good && distance < threshold)
        {
            GameData.combo = 0;
            GameData.poor++;
            return Mathf.RoundToInt(math.remap(0, threshold, 1, 0, distance) * 10 * (10 + GameData.combo));
        }

        GameData.bad++;
        GameData.combo = 0;
        return 0;
    }
}
