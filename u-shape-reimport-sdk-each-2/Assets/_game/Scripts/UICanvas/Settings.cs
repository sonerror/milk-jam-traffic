using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Settings : UICanvas
{
    public UIScaleEffect uIScaleEffect;

    public GameObject soundOnObj;
    public GameObject soundOffObj;
    public GameObject vibrateOnObj;
    public GameObject vibrateOffObj;

    public UMP_Button uMP_Button;



    private void Awake()
    {
        uIScaleEffect = GetComponent<UIScaleEffect>();
    }

    public override void Open()
    {
        base.Open();
        uIScaleEffect.EffectOpen(null);
        ShowObjs();
        uMP_Button.CheckButton();
    }

    public void ShowObjs()
    {
        //sound
        if (DataManager.Ins.playerData.isSoundOn)
        {
            soundOnObj.SetActive(true);
            soundOffObj.SetActive(false);
        }
        else
        {
            soundOnObj.SetActive(false);
            soundOffObj.SetActive(true);
        }
        //vibrate
        if (DataManager.Ins.playerData.isVibrateOn)
        {
            vibrateOnObj.SetActive(true);
            vibrateOffObj.SetActive(false);
        }
        else
        {
            vibrateOnObj.SetActive(false);
            vibrateOffObj.SetActive(true);
        }
    }

    public void BtnSound()
    {
        DataManager.Ins.playerData.isSoundOn = !DataManager.Ins.playerData.isSoundOn;
        ShowObjs();
    }

    public void BtnVibrate()
    {
        DataManager.Ins.playerData.isVibrateOn = !DataManager.Ins.playerData.isVibrateOn;
        ShowObjs();
    }

    public void BtnClose() {
        UIManager.Ins.CloseUI<Settings>();
    }
}
