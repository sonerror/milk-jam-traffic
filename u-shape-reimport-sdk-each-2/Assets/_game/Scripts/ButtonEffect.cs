using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    private bool isScaling = false;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isScaling)
        {
            isScaling = true;
            transform.DOScale(0.9f, 0.05f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isScaling = false;
        transform.DOScale(originalScale, 0.2f);
    }
}
