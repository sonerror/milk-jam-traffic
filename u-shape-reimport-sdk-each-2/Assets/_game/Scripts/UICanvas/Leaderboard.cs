using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Leaderboard : UICanvas
{
    public List<UserInfoUI> userInfoUIList = new List<UserInfoUI>();
    public UserInfoUI youUserInfoUI;
    public UIScaleEffect uIScaleEffect;
    public RectTransform userContainer;

    public Sprite[] medals;

  



    private void Awake()
    {
        uIScaleEffect = GetComponent<UIScaleEffect>();
    }

    public override void Open()
    {
        base.Open();
        uIScaleEffect.EffectOpen(null);
        UserInfoManager.Ins.SortUserInfos();
        ShowUserInfoUIs();
        userContainer.localPosition = new Vector3(0, -5754.12f, 0);
    }

    /*public override void CloseDirectly()
    {
        uIScaleEffect.EffectClose(() =>
        {
            base.CloseDirectly();
        });
    }*/

    public void ShowUserInfoUIs()
    {
        for (int i = 0; i < userInfoUIList.Count; i++)
        {
            UserInfoUI uiui = userInfoUIList[i];
            UserInfo ui = DataManager.Ins.playerData.userInfoList[i];
            uiui.SetValue(ui);
            uiui.ShowMedal(ui);
        }
        youUserInfoUI.SetValue(UserInfoManager.Ins.youUi);
        youUserInfoUI.ShowMedal(UserInfoManager.Ins.youUi);
    }

    public void BtnClose()
    {
        if (uIScaleEffect.isDoingEffect) return;
        UIManager.Ins.CloseUI<Leaderboard>();
    }
}
