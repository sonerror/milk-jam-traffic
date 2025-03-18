using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISharedCanvas : Singleton<UISharedCanvas>
{
    public TextMeshProUGUI goldText;
    int curGold;
    int Gold
    {
        set
        {
            curGold = value;
            goldText.text = value.ToString();
        }
    }

    Tween goldTextTween;

    private void Awake()
    {
        Gold = DataManager.Ins.playerData.gold;
    }
    [Button]
    public void UpdateGoldText()
    {
        if (goldTextTween != null && goldTextTween.IsActive())
        {
            goldTextTween.Kill();
        }
        goldTextTween = DOVirtual.Int(curGold, DataManager.Ins.playerData.gold, .5f, val => Gold = val);
    }
}
