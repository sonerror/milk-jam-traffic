using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandX2 : Singleton<HandX2>
{
    public GameObject hand;
    public static  bool isActive;



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
        if (DataManager.Ins.playerData.isUsedX2) return;
        if (DataManager.Ins.playerData.x2Count == 0) return;
        hand.SetActive(true);
        Effect();
        UIManager.Ins.GetUI<Gameplay>().ShowCutOutX2(true);
        LevelManager.Ins.currentLevel.StopCountDown();
        isActive = true;
    }

    public void Hide()
    {
        DataManager.Ins.playerData.isUsedX2 = true;
        hand.SetActive(false);
        isActive = false;
        AFSendEvent.SendEvent("af_tutorial_completion", new Dictionary<string, string>()
        {
                                {"af_success", "true"},
                                {"af_tutorial_id", "3" }
        });
    }

    public void Effect()
    {
        this.transform.DOKill();
        this.transform
            .DOScale(0.8f, 0.5f)
            .SetEase(Ease.InSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
