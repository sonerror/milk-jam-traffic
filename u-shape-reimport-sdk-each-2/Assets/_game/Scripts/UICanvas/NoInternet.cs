using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoInternet : UICanvas
{
    public UIScaleEffect uIScaleEffect;
    public static bool isConnectInternet;


    private void Awake()
    {
        uIScaleEffect = GetComponent<UIScaleEffect>();
    }

    public override void Open()
    {
        base.Open();
        uIScaleEffect.EffectOpen(null);
    }

    public void BtnOK()
    {
        Application.Quit();
    }

    public static void SubscribeEvent()
    {
        EventManager.OnLoadNewScene += CheckInternet;
    }

    public static void CheckInternet()
    {
        isConnectInternet = !(Application.internetReachability == NetworkReachability.NotReachable);
        if (!isConnectInternet)
        {
            UIManager.Ins.OpenUI<NoInternet>();
        }
    }
}
