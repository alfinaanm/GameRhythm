using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] Text valueText;
    [SerializeField] AudioSource scoreTick;

    public void SetValue(int value)
    {
        gameObject.SetActive(true);
        GetComponent<CanvasGroup>().DOFade(1f, 0.5f).From(0);
        GetComponent<RectTransform>().DOAnchorPosX(-320, 0.5f).From(true);

        DOVirtual.Int(0, value, 0.75f, i => valueText.text = i.ToString())
            .SetEase(Ease.Linear)
            .OnStart(() => scoreTick.Play())
            .OnComplete(() =>valueText.GetComponent<RectTransform>().DOPunchScale(Vector3.one * 0.25f, 0.125f));
    }
}
