using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBomb : Singleton<HandBomb>
{
    public GameObject hand;
    public RectTransform rectTransform;
    public UShape ushape;
    public static bool isActive;



    public void Awake()
    {
        hand.SetActive(false);
    }

    private void OnEnable()
    {
        //EventManager.OnLoadNewScene += Show;
    }

    private void OnDisable()
    {
        //EventManager.OnLoadNewScene -= Show;
    }

    public void Show()
    {
        if (DataManager.Ins.playerData.isUsedBomb) return;
        if (DataManager.Ins.playerData.bombCount == 0) return;
        hand.SetActive(true);
        Effect();
        UIManager.Ins.GetUI<Gameplay>().ShowCutOutBomb(true);
        LevelManager.Ins.currentLevel.StopCountDown();
        isActive = true;
    }

    public void Hide()
    {
        DataManager.Ins.playerData.isUsedBomb = true;
        hand.SetActive(false);
        isActive = false;
        AFSendEvent.SendEvent("af_tutorial_completion", new Dictionary<string, string>()
        {
                                {"af_success", "true"},
                                {"af_tutorial_id", "4" }
        });
    }

    public void Effect()
    {
        hand.transform.DOKill();
        hand.transform
            .DOScale(0.8f, 0.5f)
            .SetEase(Ease.InSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void MoveTo(Vector3 screenPos)
    {
        rectTransform.anchoredPosition = screenPos;
    }

    public void MoveToUshape()
    {
        if (hand.activeInHierarchy)
        {
            this.transform.SetParent(UIManager.Ins.GetUI<Gameplay>().transform);
            MoveTo(Vector3.zero);
        }
    }
}
