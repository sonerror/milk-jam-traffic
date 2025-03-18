using DG.Tweening;
using NSubstitute.Routing.Handlers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lose : UICanvas
{
    public UIScaleEffect uIScaleEffect;
    public TextMeshProUGUI goldText;
    public GameObject btnRetry;



    private void Awake()
    {
        uIScaleEffect = GetComponent<UIScaleEffect>();
    }

    private void Update()
    {
        goldText.text = DataManager.Ins.playerData.gold.ToString();
    }

    public override void Open()
    {
        base.Open();
        uIScaleEffect.EffectOpen(null);

        btnRetry.SetActive(false);
        DOVirtual.DelayedCall(2f, () =>
        {
            btnRetry.SetActive(true);
        });
        AudioManager.Ins.StopMusic();
        AudioManager.Ins.PlaySoundFX(SoundType.Lose);


        //fire base
        /*if (LevelManager.Ins.currentLevelIndex + 1 > DataManager.Ins.playerData.maxCheckPointEndLevel)
        {
            FirebaseManager.Ins.SendEvent("checkpoint_end_" + (LevelManager.Ins.currentLevelIndex + 1).ToString(),
                new Firebase.Analytics.Parameter("level", (LevelManager.Ins.currentLevelIndex + 1).ToString()));
            DataManager.Ins.playerData.maxCheckPointEndLevel = LevelManager.Ins.currentLevelIndex + 1;
            //Debug.Log("checkpoint_end_" + (LevelManager.Ins.currentLevelIndex + 1).ToString());
        }

        FirebaseManager.Ins.SendEvent("level_end",
            new Firebase.Analytics.Parameter("level_id", (LevelManager.Ins.currentLevelIndex + 1).ToString()),
            new Firebase.Analytics.Parameter("level_retry", (DataManager.Ins.playerData.levelPlayTimes[LevelManager.Ins.currentLevelIndex + 1]).ToString()),
            new Firebase.Analytics.Parameter("ingame_duration", (LevelManager.Ins.currentLevel.GetTimePlayed().ToString())),
            new Firebase.Analytics.Parameter("result", "lose"));

        FirebaseManager.Ins.SendEvent("level_fail",
            new Firebase.Analytics.Parameter("level_id", (LevelManager.Ins.currentLevelIndex + 1).ToString()),
            new Firebase.Analytics.Parameter("level_retry", (DataManager.Ins.playerData.levelPlayTimes[LevelManager.Ins.currentLevelIndex + 1]).ToString()),
            new Firebase.Analytics.Parameter("ingame_duration", (LevelManager.Ins.currentLevel.GetTimePlayed().ToString())));*/
    }

    /*public override void CloseDirectly()
    {
        uIScaleEffect.EffectClose(() =>
        {
            base.CloseDirectly();
        });
    }*/

    public void BtnHome()
    {
        if (uIScaleEffect.isDoingEffect) return;

        LevelManager.Ins.currentLevel.StopCountDown();
        SceneController.Ins.ChangeScene("Main", () => {
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<Home>();
            if (!LevelManager.Ins.IsStage1()) DataManager.Ins.playerData.currentStageIndex -= 1;
        }, true,true);
    }

    public void BtnContinue()
    {
        if (uIScaleEffect.isDoingEffect) return;

        //show reward
        MaxManager.ins.ShowReward("btn_continue", "lose", () =>
        {
            LevelManager.Ins.currentLevel.OnContinue(60f);
            UIManager.Ins.GetUI<Gameplay>().countDownText.transform.EffectBounce(1.6f);
            UIManager.Ins.CloseUI<Lose>();
            AudioManager.Ins.PlayMusic(SoundType.BG);
        });

        
    }

    public void BtnRetry()
    {
        if (uIScaleEffect.isDoingEffect) return;

        if (!LevelManager.Ins.IsStage1()) DataManager.Ins.playerData.currentStageIndex -= 1;
        SceneController.Ins.LoadCurrentScene(() =>
        {
            UIManager.Ins.CloseUI<Lose>();
            DoublePick.Ins.isActive = false;
            DoublePick.Ins.elapsedTime = -1;
            UIManager.Ins.GetUI<Gameplay>().UpdateBtnDoublePick();
        }, true,true);

        AudioManager.Ins.PlayMusic(SoundType.BG);
    }
}
