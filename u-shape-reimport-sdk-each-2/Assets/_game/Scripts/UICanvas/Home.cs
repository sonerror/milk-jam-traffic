using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Home : UICanvas
{
    public TextMeshProUGUI goldText;


    public override void Open()
    {
        base.Open();
        WinVFX.Ins?.Hide();
    }

    private void Update()
    {
        goldText.text = DataManager.Ins.playerData.gold.ToString();
    }

    public void BtnSettings()
    {
        UIManager.Ins.OpenUI<Settings>();
    }

    public void BtnPlay()
    {
        SceneController.Ins.ChangeScene("Main", () =>
        {
            UIManager.Ins.CloseUI<Home>();
            UIManager.Ins.OpenUI<Gameplay>();
            UIManager.Ins.bg.gameObject.SetActive(false);
            LevelManager.Ins.isLoadStage1 = true;
            AudioManager.Ins.PlayMusic(SoundType.BG);
        }, true, true);
    }

    public void BtnLeaderboard()
    {
        UIManager.Ins.OpenUI<Leaderboard>();
    }

    public void BtnShop()
    {
        UIManager.Ins.OpenUI<Shop>();
    }

}
