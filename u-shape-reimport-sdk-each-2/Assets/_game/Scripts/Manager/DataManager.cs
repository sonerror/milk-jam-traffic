using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Linq;
using Castle.Core.Internal;

[Serializable]
public class DataManager : Singleton<DataManager>
{
    public bool isLoaded = false;
    public PlayerData playerData;
    public const string PLAYER_DATA = "PLAYER_DATA";


    private void OnApplicationPause(bool pause) { SaveData(); }
    private void OnApplicationQuit() { SaveData(); }



    [Button]
    public void LoadData(bool isShowAOA = false)
    {
        Debug.Log("START LOAD DATA");
        string d = PlayerPrefs.GetString(PLAYER_DATA, "");
        if (d != "")
        {
            playerData = JsonUtility.FromJson<PlayerData>(d);
            //không phải lần đầu tiên vào game
            //if (isShowAOA && FirebaseManager.ins.GetValue_RemoteConfig_bool("isSessionStart")) AOAManager.ins.ShowAppOpenAd();
            if (DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalDays - playerData.timeLastOpen >= 1)
            {
                //nếu sang ngày mới

                playerData.daysPlayed += 1;

                playerData.timeLastOpen = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalDays;
            }
            else
            {
                //chưa qua ngày mới

            }
        }
        else
        {
            playerData = new PlayerData();
            FirstLoad();
            //lần đầu tiên vào game
            //if (isShowAOA && FirebaseManager.ins.GetValue_RemoteConfig_bool("isFirstOpen")) AOAManager.ins.ShowAppOpenAd();
        }
        LoadProgressTotalLists();
        playerData.greenCount = 0;
        // sau khi hoàn thành tất cả các bước load data ở trên
        isLoaded = true;
    }

    public void SaveData()
    {
        if (!isLoaded) return;
        string json = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString(PLAYER_DATA, json);
        Debug.Log("SAVE DATA");
    }

    // use when encountering error: UnityException: TrySetInt is not allowed to be called during serialization, call it from Awake or Start instead...
    void FirstLoad()
    {
        Debug.Log("first load");
        ShopItemData.SetState(playerData.skinType, UIShopItem.State.Unlocked);
        ShopItemData.SetState(playerData.stickerType, UIShopItem.State.Unlocked);
        ShopItemData.SetState(playerData.trailType, UIShopItem.State.Unlocked);
    }

    public IEnumerator CheckSession()
    {
        yield return new WaitForSecondsRealtime(10f);
        TimeSpan timeSpan = DateTime.Now - DateTime.Parse(playerData.lastExitTime);
        if (timeSpan.TotalMinutes > 30 && playerData.currentSession < 5)
        {
            // session += 1
            playerData.currentSession += 1;
            //firebase
            FirebaseManager.Ins.SendEvent("session_start_" + playerData.currentSession);
            //af
            //AFSendEvent.SendEvent("session_start_" + playerData.currentSession.ToString());
        }
    }

    public void LoadProgressTotalLists()
    {
        if(playerData.totalProgressItemList.IsNullOrEmpty())
        {
            playerData.totalProgressItemList = ProgressManager.Ins.totalProgressItemList;
        }
        if (playerData.totalProgressBoosterList.IsNullOrEmpty())
        {
            playerData.totalProgressBoosterList = ProgressManager.Ins.totalProgressBoosterList;
        }
    }
}

[Serializable]
public class BuyedItemId
{
    public List<int> list;
    public BuyedItemId()
    {
        list = new List<int>();
    }
}

[System.Serializable]
public class PlayerData
{
    [Header("------Chỉ số Game--------")]
    public double timeLastOpen;//days
    public double timeLastOpenHour;
    public int daysPlayed;
    public int sessionCount;

    [Header("-------CappingAds--------")]
    public int countCappingWinLose; // đủ 3 thì show

    /*[Header("--------- Game Setting ---------")]
    public bool isNew = true;
    public bool isMusic = true;
    public bool isSound = true;
    public bool isVibrate = true;
    public bool isNoAds = false;
    public int starRate = -1;*/


    [Header("--------- Game Params ---------")]
    public bool isPassedTutorialClick;
    public bool isPassedTutorialRotate;
    public bool isPassedTutorialZoom;
    public int currentStageIndex;
    public bool isRated;

    public SkinType skinType;
    public StickerType stickerType;
    public TrailType trailType;

    public int gold;

    [SerializeField]
    public int[] usingItemIds;
    [SerializeField]
    public List<BuyedItemId> buyedItemIdList;

    public int x2Count;
    public bool isUsedX2;
    public bool isShowedX2;

    public int bombCount;
    public bool isUsedBomb;
    public bool isShowedBomb;

    [SerializeField]
    public List<UserInfo> userInfoList;

    public bool isSoundOn;
    public bool isVibrateOn;


    public int greenCount;

    public int currentProgressItem;
    public int currentProgressBooster;

    public List<int> totalProgressItemList;
    public List<int> totalProgressBoosterList;

    public bool isFirstReceiveProgressBooster;

    public ItemType lastItemTypeReceivedInProgressChest;

    public Vector2 iconGiftPos;

    public bool isReceiveHeartSticker;

    public int challengeId;

    #region firebase
    public bool isOpenFirst = false;
    public string lastExitTime;
    public int currentSession;

    public int maxCheckPointStartIndex;
    public int maxCheckPointEndIndex;
    public int maxStageWinIndex;
    public int[] stagePlayTimes;
    #endregion


    public PlayerData()
    {
        timeLastOpen = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalDays;
        timeLastOpenHour = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMinutes;
        daysPlayed = 0;
        sessionCount = 1;

        isPassedTutorialClick = false;
        isPassedTutorialRotate = false; 
        isRated = false;

        currentStageIndex = 0;

        skinType = SkinType.None;
        stickerType = StickerType.Cycle;
        trailType = TrailType.None;
        gold = 1000;
        usingItemIds = new int[4];
        for (int i = 0; i < usingItemIds.Length; i++)
        {
            usingItemIds[i] = 0;
        }
        buyedItemIdList = new List<BuyedItemId>();
        buyedItemIdList.Add(new BuyedItemId());
        buyedItemIdList.Add(new BuyedItemId());
        buyedItemIdList.Add(new BuyedItemId());
        buyedItemIdList.Add(new BuyedItemId());
        buyedItemIdList[0].list.Add(0);
        buyedItemIdList[1].list.Add(0);
        buyedItemIdList[2].list.Add(0);

        x2Count = 1;
        isUsedX2 = false;
        isShowedX2 = false;

        bombCount = 1;
        isUsedBomb = false;
        isShowedBomb = false;

        userInfoList = new List<UserInfo>();

        isSoundOn = true;
        isVibrateOn = true;

        isPassedTutorialZoom = false;

        greenCount = 0;

        currentProgressItem = 0;
        currentProgressBooster = 0;

        isFirstReceiveProgressBooster = true;

        isReceiveHeartSticker = true;

        challengeId = 0;



    //fire base
        isOpenFirst = true;
        lastExitTime = new DateTime(1970, 1, 1).ToString();
        currentSession = 0;
        maxCheckPointStartIndex = -1;
        maxCheckPointEndIndex = -1;
        maxStageWinIndex = -1;
        stagePlayTimes = new int[200];
    }

    

}