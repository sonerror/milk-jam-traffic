using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PopUpComboAwardGift : UICanvas
{
    public UIScaleEffect uIScaleEffect;
    public GameObject x2_1;
    public GameObject bomb_1;
    public TextMeshProUGUI x2CountText;
    public TextMeshProUGUI bombCountText;
    public int x2Count;
    public int bombCount;
    public TextMeshProUGUI title;
    public GameObject btnClaim;



    private void Awake()
    {
        uIScaleEffect = GetComponent<UIScaleEffect>();
    }

    public override void Open()
    {
        base.Open();
        uIScaleEffect.EffectOpen(null);
        btnClaim.SetActive(false);
        DOVirtual.DelayedCall(2f, () =>
        {
            btnClaim.SetActive(true);
        });
        AudioManager.Ins.PlaySoundFX(SoundType.NewItem);
    }

    /*public override void CloseDirectly()
    {
        uIScaleEffect.EffectClose(() =>
        {
            base.CloseDirectly();
        });
    }*/

    public void SetTitle(string title)
    {
        this.title.text = title; 
    }

    public void SetGridValue(int x2Count, int bombCount)
    {
        if (x2Count == 0)
        {
            x2_1.SetActive(false);
            bomb_1.SetActive(true);
        }
        else if (bombCount == 0)
        {
            x2_1.SetActive(true);
            bomb_1.SetActive(false);
        }
        else
        {
            x2_1.SetActive(true);
            bomb_1.SetActive(true);
        }

        x2CountText.text = x2Count.ToString();
        bombCountText.text = bombCount.ToString();

        this.x2Count = x2Count;
        this.bombCount = bombCount;
    }

    public void BtnX2()
    {
        MaxManager.ins.ShowReward("btn_x2", "popup_combo_award_gift", () =>
        {
            x2Count *= 2;
            bombCount *= 2;
            DataManager.Ins.playerData.x2Count += x2Count;
            DataManager.Ins.playerData.bombCount += bombCount;
            DataManager.Ins.playerData.greenCount = 0;
            ComboManager.Ins.ResetComboLabels();
            ComboManager.Ins.isStopCalculateGreenCountAfterBomb = true;
            ComboManager.Ins.StopAllCoroutines();
            UIManager.Ins.GetUI<Gameplay>().ShowX2CountOrAds();
            if (x2Count > 0)
            {
                UIManager.Ins.GetUI<Gameplay>().x2CountContainer.EffectBounce(1.4f);
            }
            UIManager.Ins.GetUI<Gameplay>().ShowBombCountOrAds();
            if (bombCount > 0)
            {
                UIManager.Ins.GetUI<Gameplay>().bombCountContainer.EffectBounce(1.4f);
            }
            ComboBar.Ins.ResetIconGift();
            LevelManager.Ins.currentLevel.OnContinue(0f);
            UIManager.Ins.CloseUI<PopUpComboAwardGift>();
        });
    }

    public void BtnClaim()
    {
        if (uIScaleEffect.isDoingEffect) return;
        DataManager.Ins.playerData.x2Count += x2Count;
        DataManager.Ins.playerData.bombCount += bombCount;
        DataManager.Ins.playerData.greenCount = 0;
        ComboManager.Ins.ResetComboLabels();
        ComboManager.Ins.isStopCalculateGreenCountAfterBomb = true;
        ComboManager.Ins.StopAllCoroutines();
        UIManager.Ins.GetUI<Gameplay>().ShowX2CountOrAds();
        if(x2Count > 0)
        {
            UIManager.Ins.GetUI<Gameplay>().x2CountContainer.EffectBounce(1.4f);
        }
        UIManager.Ins.GetUI<Gameplay>().ShowBombCountOrAds();
        if(bombCount > 0)
        {
            UIManager.Ins.GetUI<Gameplay>().bombCountContainer.EffectBounce(1.4f);
        }
        ComboBar.Ins.ResetIconGift();
        LevelManager.Ins.currentLevel.OnContinue(0f);
        UIManager.Ins.CloseUI<PopUpComboAwardGift>();
    }
}
