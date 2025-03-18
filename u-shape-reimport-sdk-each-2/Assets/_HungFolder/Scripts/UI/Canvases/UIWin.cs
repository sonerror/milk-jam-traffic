using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIWin : UICanvas
{
    int curGold;
    int targetGold;

    #region LevelCompleted
    [Header("[Level completed phase]")]
    public GameObject levelCompletedGO;
    public TextMeshProUGUI goldText;
    int CurGold
    {
        set
        {
            curGold = value;
            goldText.text = value.ToString();
        }
    }
    public void OnInit(int gold)
    {
        CurGold = 0;
        targetGold = gold;
    }
    public void IncresingGold()
    {
        DOVirtual.Int(curGold, targetGold, 1, val => CurGold = val);
    }

    public void AdsButton()
    {

    }

    public void SkipButton()
    {

    }
    #endregion

    #region Amazing
    [Header("[Amazing phase]")]
    public GameObject amazingGO;

    public void ContinueButton()
    {

    }

    #endregion

    #region Claim Popup
    [Header("[Claim popup phase]")]
    public GameObject claimPopupGO;

    public void ClaimButton()
    {

    }
    #endregion
}
