using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] AudioSource pressStart;

    [SerializeField] RectTransform mainContainer;
    [SerializeField] CanvasGroup whiteTransition;

    bool isInputEnabled = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isInputEnabled)
        {
            pressStart.Play();
            mainContainer.DOScale(1.5f, 1f)
            .SetEase(Ease.InCirc)
            .OnStart(()=>isInputEnabled = false);

            whiteTransition.DOFade(1, 1f)
            .SetEase(Ease.InCirc)
            .OnComplete(
                () => StartCoroutine(LoadScene())
            );
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitWhile(() => pressStart.isPlaying);
        DOTween.KillAll();
        SceneManager.LoadSceneAsync("Scenes/Level Selection");
    }
}
