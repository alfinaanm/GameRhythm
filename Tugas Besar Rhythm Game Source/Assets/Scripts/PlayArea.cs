using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using DG.Tweening;

public class PlayArea : MonoBehaviour
{
    [SerializeField] Text scoreText, comboText, healthText;
    [SerializeField] RectTransform healthPanel;

    [SerializeField] Vector2 startingPosition = Vector2.zero;
    [SerializeField] GameObject hitAreaD, hitAreaF, hitAreaJ, hitAreaK, noteVoid;

    [SerializeField] CanvasGroup whiteTransition;

    [SerializeField] Animator character;

    GameObject beatmap, track;

    bool isInputEnabled = false;

    void Awake()
    {
        GameData.Reset();
        beatmap = GameData.selectedSong.beatmap;

        track = Instantiate(beatmap, startingPosition, Quaternion.identity);
    }

    void Start()
    {
        Intro();
    }

    void Update()
    {
        // If track is instantiated
        if (track != null)
        {
            track.transform.position = startingPosition + track.GetComponent<Track>().GetTrackPosition();
            NoteVoid();
        }

        // Keyboard input
        if (isInputEnabled)
        {
            // Hit Area
            if (Input.GetKeyDown(KeyCode.D))
            {
                hitAreaD.GetComponent<SpriteRenderer>().color = new Color32(255, 23, 68, 255);
                Hit(hitAreaD.transform.position);
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                hitAreaF.GetComponent<SpriteRenderer>().color = new Color32(255, 234, 0, 255);
                Hit(hitAreaF.transform.position);
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                hitAreaJ.GetComponent<SpriteRenderer>().color = new Color32(102, 187, 106, 255);
                Hit(hitAreaJ.transform.position);
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                hitAreaK.GetComponent<SpriteRenderer>().color = new Color32(61, 90, 254, 255);
                Hit(hitAreaK.transform.position);
            }
            if (Input.GetKeyDown(KeyCode.Escape)) Exit();
        }

        // Hit Feedback
        if (Input.GetKeyUp(KeyCode.D)) hitAreaD.GetComponent<SpriteRenderer>().color = new Color32(183, 28, 28, 255);
        if (Input.GetKeyUp(KeyCode.F)) hitAreaF.GetComponent<SpriteRenderer>().color = new Color32(245, 127, 23, 255);
        if (Input.GetKeyUp(KeyCode.J)) hitAreaJ.GetComponent<SpriteRenderer>().color = new Color32(27, 94, 32, 255);
        if (Input.GetKeyUp(KeyCode.K)) hitAreaK.GetComponent<SpriteRenderer>().color = new Color32(13, 71, 161, 255);
    }

    // Hit Judgment
    void Hit(Vector2 hitAreaPos)
    {
        Collider2D note = Physics2D.BoxCast(hitAreaPos, new Vector2(1.5f, 0.5f), 0, Vector2.up, 8).collider;

        // If a note is hit
        if (note != null && note.name != "End")
        {
            Vector2 notePos = note.transform.position;
            float distance = Vector2.Distance(notePos, hitAreaPos);

            // Score Category
            int scoreValue = note.GetComponent<Note>().Judge(0.5f, distance);

            if (scoreValue == 0)
                Miss();
            else
            {
                character.SetBool("Mood", true);
                GameData.score += scoreValue;

                if (GameData.combo != 0) comboText.text = GameData.combo + "<size=25>Combo!</size>";
                else comboText.text = "";

                DOVirtual.Int(int.Parse(scoreText.text), GameData.score, 0.125f, (x) => scoreText.text = x.ToString())
                    .SetEase(Ease.OutCirc);

                comboText.rectTransform.localScale = Vector3.one;
                comboText.rectTransform.DOPunchScale(Vector3.one * 0.125f, 0.125f).SetEase(Ease.InOutCirc);
            }
        }
    }

    // Start Level
    void Intro()
    {
        whiteTransition.DOFade(1f, 1f).From()
        .OnComplete(() =>
        {
            isInputEnabled = true;
            track.GetComponent<Track>().Play();
        });
    }

    // End level
    void Outro(bool isWin = false)
    {
        track.GetComponent<Track>().songPlayer.DOFade(0,1);
        whiteTransition.DOFade(1f, 1f)
        .OnStart(() =>
        {
            isInputEnabled = false;
            if (!isWin) track.GetComponent<Track>().Stop();
        })
        .OnComplete(() =>
        {
            DOTween.KillAll();
            SceneManager.LoadScene("Scenes/Result Screen");
        });
    }

    void Exit()
    {
        whiteTransition.DOFade(1f, 1f)
        .OnStart(() =>
        {
            isInputEnabled = false;
            track.GetComponent<Track>().Stop();
        })
        .OnComplete(() =>
        {
            DOTween.KillAll();
            SceneManager.LoadScene("Scenes/Level Selection");
        });
    }

    // If 30 bads, exit
    void Miss()
    {
        character.SetBool("Mood", false);
        comboText.text = "";

        healthText.text = math.remap(0, 30, 1, 0, GameData.bad).ToString("0%");
        healthPanel.transform.GetChild(0).GetComponent<Image>().fillAmount = math.remap(0, 30, 1, 0, GameData.bad);

        healthPanel.anchoredPosition = new Vector2Int(145, -50);
        healthPanel.DOShakeAnchorPos(0.25f, 20, 250);

        if (GameData.bad >= 30)
            Outro();
    }

    // Overflow
    void NoteVoid()
    {
        Collider2D note = Physics2D.OverlapBox(noteVoid.transform.position, new Vector2(6f, 0.5f), 0);
        if (note != null)
        {
            if (note.name != "End")
            {
                note.GetComponent<Note>().Judge(0.5f, 1);
                Miss();
            }
            else
            {
                Outro(true);
            }
        }
    }
}
