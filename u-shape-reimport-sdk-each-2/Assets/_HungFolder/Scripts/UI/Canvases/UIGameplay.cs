using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGameplay : UICanvas
{
    [Header("[Params]")]
    public int level;
    public int moves;

    [Header("[References]")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI moveText;

    int Level => Level;
    int Moves => moves;
    int LevelText { set { levelText.text = "Level " + value; } }
    int MoveText { set { moveText.text = value + " Moves"; } }

    public override void Open()
    {
        base.Open();
        LevelText = level;
        MoveText = moves;
    }
    public void ShopButton()
    {
        UIManager.Ins.OpenUI<UIShop>();
    }
    public void SettingButton()
    {

    }
    public void ReplayButton()
    {

    }
}
