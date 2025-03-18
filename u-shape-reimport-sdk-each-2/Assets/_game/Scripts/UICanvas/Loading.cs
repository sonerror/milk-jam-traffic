using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : UICanvas
{
    public Slider slider;

    public override void Open()
    {
        base.Open();
        RunSlider();
    }

    public void RunSlider()
    {
        float duration = 2f;
        DOVirtual.Float(0, 1, duration, (value) =>
        {
            slider.value = value;
        }).SetEase(Ease.InCubic)
        .OnComplete(() =>
        {
            CacheSomeUIs();
            slider.value = 1f;
            AOAManager.ins.isDropAOA = true;
            DOVirtual.DelayedCall(0.5f, () =>
            {
                UIManager.Ins.CloseUI<Loading>();
                UIManager.Ins.OpenUI<Home>();
            });
        });
    }

    public void CacheSomeUIs()
    {
        UIManager.Ins.OpenUI<Leaderboard>();
        UIManager.Ins.CloseUI<Leaderboard>();
        UIManager.Ins.OpenUI<Shop>();
        UIManager.Ins.GetUI<Shop>().ShowContainer(2);
        UIManager.Ins.GetUI<Shop>().ShowContainer(1);
        UIManager.Ins.GetUI<Shop>().ShowContainer(0);
        UIManager.Ins.CloseUI<Shop>();
        UIManager.Ins.OpenUI<Settings>();
        UIManager.Ins.CloseUI<Settings>();
    }
}
