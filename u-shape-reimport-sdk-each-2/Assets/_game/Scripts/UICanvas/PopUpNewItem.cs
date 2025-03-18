using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpNewItem : UICanvas
{
    public TextMeshProUGUI title;
    public Image itemImg;
    public UIScaleEffect uIScaleEffect;
    public static Action OnClaim;

    private void Awake()
    {
        uIScaleEffect = GetComponent<UIScaleEffect>();
    }

    public override void Open()
    {
        base.Open();
        uIScaleEffect.EffectOpen(null);
        AudioManager.Ins.PlaySoundFX(SoundType.NewItem);
    }

    /*public override void CloseDirectly()
    {
        uIScaleEffect.EffectClose(() =>
        {
            base.CloseDirectly();
        });
    }*/

    public void SetValue(ItemType itemType, Sprite sprite)
    {
        this.title.text = "New " + itemType.ToString();
        this.itemImg.sprite = sprite;
    }

    public void BtnClaim()
    {
        if (uIScaleEffect.isDoingEffect) return;
        OnClaim?.Invoke();
        UIManager.Ins.CloseUI<PopUpNewItem>();
    }
}
