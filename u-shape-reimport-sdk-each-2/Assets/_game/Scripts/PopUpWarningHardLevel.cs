using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PopUpWarningHardLevel : UICanvas
{
    public UIScaleEffect uIScaleEffect;



    private void Awake()
    {
        uIScaleEffect = GetComponent<UIScaleEffect>();
    }

    public override void Open()
    {
        base.Open();
        uIScaleEffect.EffectOpen(null);
        AudioManager.Ins.PlaySoundFX(SoundType.Warning);
        
    }

    public override void CloseDirectly()
    {
        AudioManager.Ins.PlayMusic(SoundType.BG);
        UIManager.Ins.GetUI<Gameplay>().ShowOrHideBtnDoublePick();
        UIManager.Ins.GetUI<Gameplay>().ShowOrHideBtnBomb();
        base.CloseDirectly();
    }

    /*public override void CloseDirectly()
    {
        effectRope.EffectClose(() =>
        {
            base.CloseDirectly();
        });
    }*/

}
