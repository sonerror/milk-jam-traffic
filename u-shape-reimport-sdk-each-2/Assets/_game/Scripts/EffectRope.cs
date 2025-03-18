using DG.Tweening;
using System;
using UnityEngine;

public class EffectRope : MonoBehaviour
{
    public RectTransform container;
    public float targetY;
    public bool isActive;
    public bool isDoingEffect;

    public static float zoomInDuration = 0.15f;
    public static float zoomOutDuration = 0.15f;


    private void Awake()
    {
        isActive = true;
    }

    public void SetContainer(Transform c)
    {
        container = c.GetComponent<RectTransform>();
    }

    public void EffectOpen(Action onComplete)
    {
        if (!isActive)
        {
            onComplete?.Invoke();
            return;
        }

        isDoingEffect = true;


        Vector2 oldPos = new Vector2(0, 1800f/3);
        container.localPosition = oldPos;
        //Vector3 oldScale = Vector3.one * 0.3f;
        //container.localScale = oldScale;

        Vector2 newPos = new Vector2(0, targetY);
        container
            .DOAnchorPos(newPos, zoomInDuration)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                isDoingEffect = false;
                onComplete?.Invoke();
            })
            ;
    }

    public void EffectClose(Action onComplete)
    {
        if (!isActive)
        {
            onComplete?.Invoke();
            return;
        }

        isDoingEffect = true;

        Vector2 newPos = new Vector2(0, 1800f);
        container
            .DOAnchorPos(newPos, zoomOutDuration)
            .SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                isDoingEffect = false;
                onComplete?.Invoke();
            })
            ;
    }

}
