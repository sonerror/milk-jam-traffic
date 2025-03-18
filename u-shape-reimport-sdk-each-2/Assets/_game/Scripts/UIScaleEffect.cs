using DG.Tweening;
using System;
using UnityEngine;

public class UIScaleEffect : MonoBehaviour
{
    public Transform container;
    public bool isDoingEffect;

    public static float zoomInDuration = 0.25f;
    public static float zoomOutDuration = 0.4f;


    private void Awake()
    {
    }

    public void EffectOpen(Action onComplete)
    {
        isDoingEffect = true;

        Vector3 oldScale = Vector3.one * 0.3f;
        container.localScale = oldScale;

        Vector3 newScale = Vector3.one * 1f;
        container
            .DOScale(newScale, zoomInDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                isDoingEffect = false;
                onComplete?.Invoke();
            })
            ;
    }

    public void EffectClose(Action onComplete)
    {
        isDoingEffect = true;

        Vector3 oldScale = Vector3.one * 1f;
        container.localScale = oldScale;

        Vector3 newScale = Vector3.one * 0f;
        container
            .DOScale(newScale, zoomOutDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                isDoingEffect = false;
                onComplete?.Invoke();
            })
            ;
    }
}
