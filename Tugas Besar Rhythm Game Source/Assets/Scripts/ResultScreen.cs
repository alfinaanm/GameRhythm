using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] GameObject greatPanel, goodPanel, poorPanel, badPanel, maxComboPanel;
    [SerializeField] GameObject accuracyPanel, scorePanel;
    [SerializeField] GameObject statusText;

    [SerializeField] CanvasGroup whiteTransition;
    [SerializeField] RectTransform mainContainer;

    [SerializeField] AudioSource drumRoll, next, scoreTick;

    [SerializeField] Animation blink;
    [SerializeField] Animator character;

    Sequence tl;

    int great, good, poor, bad, maxCombo, score;
    float accuracy;

    bool isInputEnabled = false;

    void Awake()
    {
        great = GameData.great;
        good = GameData.good;
        poor = GameData.poor;
        bad = GameData.bad;
        maxCombo = GameData.maxCombo;

        accuracy = GameData.accuracy;
        score = GameData.score;

        tl = DOTween.Sequence();
    }

    void Start()
    {
        Intro();
        if (GameData.bad < 30) ShowAccuracy();
        ShowStatus();
        ShowScore();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isInputEnabled)
        {
            next.Play();
            mainContainer.DOScale(1.5f, 1f)
            .SetEase(Ease.InCirc)
            .OnStart(() => isInputEnabled = false);

            whiteTransition.DOFade(1, 1f)
            .SetEase(Ease.InCirc)
            .OnComplete(
                () => StartCoroutine(LoadScene())
            );
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitWhile(() => next.isPlaying);
        SceneManager.LoadSceneAsync("Scenes/Level Selection");
    }

    void HighScoreCheck()
    {
        if (score > SaveSystem.Load(GameData.selectedSong.savePath))
            SaveSystem.Save(GameData.selectedSong.savePath, score);
    }

    void Intro()
    {
        // 4 Judgment Panels
        tl.Append(whiteTransition.DOFade(1, 1f).From());
        tl.Join(mainContainer.DOScale(1.5f, 1f).From());

        tl.AppendCallback(() => greatPanel.GetComponent<ResultPanel>().SetValue(great));
        tl.AppendInterval(0.75f);
        tl.AppendCallback(() => goodPanel.GetComponent<ResultPanel>().SetValue(good));
        tl.AppendInterval(0.75f);
        tl.AppendCallback(() => poorPanel.GetComponent<ResultPanel>().SetValue(poor));
        tl.AppendInterval(0.75f);
        tl.AppendCallback(() => badPanel.GetComponent<ResultPanel>().SetValue(bad));
        tl.AppendInterval(0.75f);
        tl.AppendCallback(() => maxComboPanel.GetComponent<ResultPanel>().SetValue(maxCombo));
        tl.AppendInterval(0.75f);
    }

    void ShowAccuracy()
    {
        // Accuracy
        tl.Append(DOVirtual.Float(0, accuracy, 3f,
                x => accuracyPanel.transform.GetChild(1).GetComponent<Text>().text = x.ToString("0.00%")
            )
            .SetEase(Ease.InCirc)
            .OnStart(() =>
            {
                accuracyPanel.gameObject.SetActive(true);
                accuracyPanel.GetComponent<CanvasGroup>().DOFade(1f, 0.5f).From(0);
                accuracyPanel.GetComponent<RectTransform>().DOAnchorPosX(-320, 0.5f).From(true);
                drumRoll.Play();
            })
            .OnComplete(
                () => accuracyPanel.transform.GetChild(1).GetComponent<RectTransform>().DOPunchScale(Vector3.one * 0.25f, 0.125f)
            ));
    }

    void ShowStatus()
    {
        // Status
        tl.AppendCallback(() =>
        {
            statusText.GetComponent<CanvasGroup>().DOFade(0, 0.5f).From();

            if (accuracy < 0.4f || GameData.bad >= 30)
            {
                character.SetBool("Mood", false);
                statusText.GetComponent<Text>().text = "<color=#FF1744>FAILED!</color>";
            }
            else
            {
                HighScoreCheck();
                character.SetBool("Mood", true);
                statusText.GetComponent<Text>().text = "<color=#00E676>PASSED!</color>";
            }

            statusText.transform.DOScale(2f, 0.5f).From()
                .SetEase(Ease.InCirc)
                .OnComplete(() => statusText.GetComponent<RectTransform>().DOShakeAnchorPos(0.5f, 20, 100));
        });
    }

    void ShowScore()
    {
        // Score
        tl.Append(DOVirtual.Int(0, score, 5f, x => scorePanel.transform.GetChild(1).GetComponent<Text>().text = x.ToString())
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                scorePanel.gameObject.SetActive(true);
                scorePanel.GetComponent<CanvasGroup>().DOFade(1f, 0.5f).From(0);
                scorePanel.GetComponent<RectTransform>().DOAnchorPosX(-320, 0.5f).From(true);
                scoreTick.Play();
                scoreTick.DOPitch(2f, 5f).SetEase(Ease.InCirc);
            })
            .OnComplete(() =>
            {
                scorePanel.transform.GetChild(1).GetComponent<RectTransform>().DOPunchScale(Vector3.one * 0.25f, 0.125f);
                scoreTick.Stop();

                isInputEnabled = true;
                blink.Play();
            })
            );
    }
}
