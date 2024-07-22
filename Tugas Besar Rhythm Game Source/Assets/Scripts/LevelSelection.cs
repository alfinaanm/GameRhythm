using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LevelSelection : MonoBehaviour
{
    private int selectedIndex = 0;

    [SerializeField] GameObject scrollBar;
    [SerializeField] GameObject scrollContent;

    [SerializeField] AudioSource cursor;
    [SerializeField] AudioSource pressStart;
    [SerializeField] AudioSource bgm;

    [SerializeField] RectTransform mainContainer;
    [SerializeField] CanvasGroup whiteTransition;

    int songsCount;

    float targetScroll = 1f;
    float[] itemsPosition;

    bool isInputEnabled = false;

    void Awake()
    {
        songsCount = scrollContent.transform.childCount;
    }

    void Start()
    {
        mainContainer.DOScale(1.5f, 1f)
        .From()
        .SetEase(Ease.OutCirc);

        whiteTransition.DOFade(1, 1f)
        .From()
        .SetEase(Ease.OutCirc)
        .OnComplete(() =>
            {
                isInputEnabled = true;
                Scroll();
            }
        );
    }

    void Update()
    {
        // Items
        itemsPosition = new float[songsCount];
        float distance = 1f / (itemsPosition.Length - 1f);

        for (int i = 0; i < itemsPosition.Length; i++)
            itemsPosition[i] = distance * i;

        // Keyboard input
        if (isInputEnabled)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedIndex = --selectedIndex < 0 ? (songsCount - 1) : selectedIndex;
                targetScroll = 1 - itemsPosition[selectedIndex];

                cursor.Play();
                Scroll();
            }

            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedIndex = ++selectedIndex % songsCount;
                targetScroll = 1 - itemsPosition[selectedIndex];

                cursor.Play();
                Scroll();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameData.selectedSong = scrollContent.transform.GetChild(selectedIndex)
                    .GetComponent<SongItem>().songData;

                pressStart.Play();
                bgm.DOFade(0,1);
                mainContainer.DOScale(1.5f, 1f)
                .SetEase(Ease.InCirc)
                .OnStart(() => isInputEnabled = false);
                whiteTransition.DOFade(1, 1f)
                .SetEase(Ease.InCirc)
                .OnComplete(() => StartCoroutine(LoadScene()));
            }

            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitWhile(() => pressStart.isPlaying);
        DOTween.KillAll();
        SceneManager.LoadSceneAsync("Scenes/Play Area");
    }

    void Scroll()
    {
        // Animate scroll position
        DOVirtual.Float(
            scrollBar.GetComponent<Scrollbar>().value,
            targetScroll,
            0.25f,
            x => scrollBar.GetComponent<Scrollbar>().value = x
        )
        .SetEase(Ease.InOutSine);

        // Set selected item bigger in scale
        for (int i = 0; i < scrollContent.transform.childCount; i++)
        {
            if (i == selectedIndex)
                scrollContent.transform.GetChild(i)
                    .DOScale(1.25f, 0.25f)
                    .SetEase(Ease.InOutSine);

            else
                scrollContent.transform.GetChild(i)
                    .DOScale(1, 0.25f)
                    .SetEase(Ease.InOutSine);
        }
    }
}
