using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Win : UICanvas
{
    public UIScaleEffect uIScaleEffect;
    public TextMeshProUGUI goldText;

    public ProgressItemUI itemChestUI;
    public ProgressItemUI boosterChestUI;

    public bool isCanSkipWatchProgress;



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
        isCanSkipWatchProgress = true;
        RunProgress();
        UserInfoManager.Ins.youUi.score += UnityEngine.Random.Range(4000, 5000);
        UserInfoManager.Ins.BoostUsersScore();

        itemChestUI.OnOpen();
        boosterChestUI.OnOpen();

        ComboBar.Ins.StopAllCoroutines();

        //fire base
        /*if (LevelManager.Ins.currentLevelIndex > DataManager.Ins.playerData.maxCheckPointEndLevel)
        {
            FirebaseManager.Ins.SendEvent("checkpoint_end_" + (LevelManager.Ins.currentLevelIndex).ToString(),
                new Firebase.Analytics.Parameter("level", (LevelManager.Ins.currentLevelIndex).ToString()));
            DataManager.Ins.playerData.maxCheckPointEndLevel = LevelManager.Ins.currentLevelIndex;
            Debug.Log("checkpoint_end_" + (LevelManager.Ins.currentLevelIndex).ToString());
        }

        FirebaseManager.Ins.SendEvent("level_end",
            new Firebase.Analytics.Parameter("level_id", (LevelManager.Ins.currentLevelIndex).ToString()),
            new Firebase.Analytics.Parameter("level_retry", (DataManager.Ins.playerData.levelPlayTimes[LevelManager.Ins.currentLevelIndex]).ToString()),
            new Firebase.Analytics.Parameter("ingame_duration", (LevelManager.Ins.currentLevel.GetTimePlayed().ToString())),
            new Firebase.Analytics.Parameter("result", "win"));

        FirebaseManager.Ins.SendEvent("level_win",
            new Firebase.Analytics.Parameter("level_id", (LevelManager.Ins.currentLevelIndex).ToString()),
            new Firebase.Analytics.Parameter("level_retry", (DataManager.Ins.playerData.levelPlayTimes[LevelManager.Ins.currentLevelIndex]).ToString()),
            new Firebase.Analytics.Parameter("ingame_duration", (LevelManager.Ins.currentLevel.GetTimePlayed().ToString())));*/

        /*FirebaseManager.Ins.SendEvent("level_achieved",
            new Firebase.Analytics.Parameter("level", (LevelManager.Ins.currentLevelIndex).ToString()));*/
        
        //firebase
        if(DataManager.Ins.playerData.currentStageIndex/2 > DataManager.Ins.playerData.maxCheckPointEndIndex)
        {
            string level = (DataManager.Ins.playerData.currentStageIndex / 2 + 1).ToString();
            int durationStage1 = (int)LevelManager.Ins.durationStage1;
            int durationStage2 = (int)LevelManager.Ins.currentLevel.GetTimePlayed();
            int totalDuration = durationStage1 + durationStage2;
            int retryLevel = DataManager.Ins.playerData.stagePlayTimes[LevelManager.Ins.currentStageIndex-1]-1;
            FirebaseManager.Ins.SendEvent("checkpoint_end",
                    new Firebase.Analytics.Parameter("level", level),
                    new Firebase.Analytics.Parameter("duration_stage_1", durationStage1),
                    new Firebase.Analytics.Parameter("duration_stage_2", durationStage2),
                    new Firebase.Analytics.Parameter("total_duration", totalDuration),
                    new Firebase.Analytics.Parameter("retry_level", retryLevel)
                );
            Debug.Log("checkpoint_end level " + level + " durationStage1 " + durationStage1 + " durationStage2 " + durationStage2 + " totalDuration " + totalDuration + " retryLevel " + retryLevel);
            DataManager.Ins.playerData.maxCheckPointEndIndex = DataManager.Ins.playerData.currentStageIndex / 2;
        }
        
        //af
        AFSendEvent.SendEvent("af_level_achieved", new Dictionary<string, string>()
        {
            {"af_level", (LevelManager.Ins.currentLevelIndex + 1).ToString()},
            {"score", ""}
        });

/*
        switch (LevelManager.Ins.currentLevelIndex)
        {
            case 1:
                FirebaseManager.Ins.SendEvent("level_achieved_1");
                AFSendEvent.SendEvent("level_achieved_1");
                break;
            case 5:
                FirebaseManager.Ins.SendEvent("level_achieved_5");
                AFSendEvent.SendEvent("level_achieved_5");
                break;
            case 10:
                FirebaseManager.Ins.SendEvent("level_achieved_10");
                AFSendEvent.SendEvent("level_achieved_10");
                break;
            case 15:
                FirebaseManager.Ins.SendEvent("level_achieved_15");
                AFSendEvent.SendEvent("level_achieved_15");
                break;
            case 20:
                FirebaseManager.Ins.SendEvent("level_achieved_20");
                AFSendEvent.SendEvent("level_achieved_20");
                break;
            case 25:
                FirebaseManager.Ins.SendEvent("level_achieved_25");
                AFSendEvent.SendEvent("level_achieved_25");
                break;
            case 30:
                FirebaseManager.Ins.SendEvent("level_achieved_30");
                AFSendEvent.SendEvent("level_achieved_30");
                break;
            default: break;
        }
*/
    }

    private void RunProgress()
    {
        SetUpProgressItem();
        SetUpProgressBooster();
    }

    Func<ItemType, int, int> FuncClaimProgressItem = (itemType, itemId) =>
    {
        DataManager.Ins.playerData.currentProgressItem = 0;
        DataManager.Ins.playerData.lastItemTypeReceivedInProgressChest = itemType;
        DataManager.Ins.playerData.usingItemIds[(int)itemType] = itemId;
        DataManager.Ins.playerData.buyedItemIdList[(int)itemType].list.Add(itemId);
        if(DataManager.Ins.playerData.totalProgressItemList.Count > 1)
        {
            DataManager.Ins.playerData.totalProgressItemList.RemoveAt(0);
        }
        switch (itemType)
        {
            case ItemType.Skin:
                DataManager.Ins.playerData.skinType = (SkinType)itemId;
                //SkinManager.Ins.ChangeSkin();
                break;
            case ItemType.Sticker:
                DataManager.Ins.playerData.stickerType = (StickerType)itemId;
                //StickerManager.Ins.ChangeSticker();
                break;
            case ItemType.Trail:
                DataManager.Ins.playerData.trailType = (TrailType)itemId;
                //TrailManager.Ins.ChangeTrail();
                break;
            case ItemType.RemoveAds:
                break;
            default: break;
        }
        return 0;
    };

    public void SetUpProgressItem()
    {
        ProgressItemUI piUI = itemChestUI;
        piUI.slider.value = ProgressManager.Ins.GetPercentageItem();
        piUI.ChangeProgressText(true);
        DataManager.Ins.playerData.currentProgressItem += 1;
        isCanSkipWatchProgress &= !ProgressManager.Ins.IsProgressItemCollected();
        DOVirtual.DelayedCall(0.6f, () =>
        {
            piUI.RunProgress(ProgressManager.Ins.GetPercentageItem(), () =>
            {
                piUI.ChangeProgressText(true);
                if (ProgressManager.Ins.IsProgressItemCollected())
                {
                    UIManager.Ins.OpenUI<PopUpNewItem>();
                    isCanSkipWatchProgress = true;

                    bool isItemAvailable = ProgressManager.Ins.InitItemTypeAndItemId();
                    ItemType itemType = ProgressManager.Ins.itemType;
                    int itemId = ProgressManager.Ins.itemId;

                    FuncClaimProgressItem(itemType, itemId);

                    Sprite itemSprite = UIManager.Ins.GetUI<Shop>().pages[(int)itemType].buttons[itemId].GetComponent<ItemButton>().itemIcon.sprite;
                    UIManager.Ins.GetUI<PopUpNewItem>().SetValue(itemType, itemSprite);
                    UIManager.Ins.CloseUI<Shop>();

                    if (!isItemAvailable)
                    {
                        UIManager.Ins.CloseUI<PopUpNewItem>();
                    }

                    AFSendEvent.SendEvent("af_achievement_unlocked", new Dictionary<string, string>()
                    {
                        {"content_id", DataManager.Ins.playerData.challengeId.ToString()},
                        {"af_level", (LevelManager.Ins.currentLevelIndex).ToString() }
                    });
                    DataManager.Ins.playerData.challengeId += 1;
                }
            });
        });
    }

    public void SetUpProgressBooster()
    {
        ProgressItemUI piUI = boosterChestUI;
        piUI.slider.value = ProgressManager.Ins.GetPercentageBooster();
        piUI.ChangeProgressText(false);
        DataManager.Ins.playerData.currentProgressBooster += 1;
        isCanSkipWatchProgress &= !ProgressManager.Ins.IsProgressBoosterCollected();
        DOVirtual.DelayedCall(0.7f, () =>
        {
            piUI.RunProgress(ProgressManager.Ins.GetPercentageBooster(), () =>
            {
                piUI.ChangeProgressText(false);
                if (ProgressManager.Ins.IsProgressBoosterCollected())
                {
                    Action ShowPopUpBoosterChest = () =>
                    {
                        DataManager.Ins.playerData.currentProgressBooster = 0;
                        int x2Count = UnityEngine.Random.Range(0, 2);
                        int bombCount = 2 - x2Count;
                        UIManager.Ins.OpenUI<PopUpComboAwardGift>();
                        if (DataManager.Ins.playerData.isFirstReceiveProgressBooster)
                        {
                            UIManager.Ins.GetUI<PopUpComboAwardGift>().SetGridValue(1, 0);
                            DataManager.Ins.playerData.isFirstReceiveProgressBooster = false;
                        }
                        else
                        {
                            UIManager.Ins.GetUI<PopUpComboAwardGift>().SetGridValue(x2Count, bombCount);
                        }
                        UIManager.Ins.GetUI<PopUpComboAwardGift>().SetTitle("BOOSTER CHEST");
                        isCanSkipWatchProgress = true;
                    };
                    if (UIManager.Ins.IsOpened<PopUpNewItem>())
                    {
                        PopUpNewItem.OnClaim = ShowPopUpBoosterChest;
                    }
                    else
                    {
                        ShowPopUpBoosterChest?.Invoke();
                    }

                    if(DataManager.Ins.playerData.totalProgressBoosterList.Count > 1)
                    {
                        DataManager.Ins.playerData.totalProgressBoosterList.RemoveAt(0);
                    }

                    //af
                    AFSendEvent.SendEvent("af_achievement_unlocked", new Dictionary<string, string>()
                    {
                        {"content_id", DataManager.Ins.playerData.challengeId.ToString()},
                        {"af_level", (LevelManager.Ins.currentLevelIndex).ToString() }
                    });
                    DataManager.Ins.playerData.challengeId += 1;
                }
            });
        });
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
        if (ProgressItemUI.isRunningProgress) return;

        LevelManager.Ins.currentLevel.StopCountDown();
        SceneController.Ins.ChangeScene("Main", () => {
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<Home>();
            if (!LevelManager.Ins.IsStage1()) DataManager.Ins.playerData.currentStageIndex -= 1;
        }, true, true);
    }

    public void BtnContinue()
    {
        if (uIScaleEffect.isDoingEffect) return;
        if (!isCanSkipWatchProgress) return;
        //if (ProgressItemUI.isRunningProgress) return;

        SceneController.Ins.LoadCurrentScene(()=> {
            UIManager.Ins.CloseUI<Win>();
            AudioManager.Ins.PlayMusic(SoundType.BG);
        }, true, true);
    }
}
