using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ProgressManager : Singleton<ProgressManager>
{
    public List<int> totalProgressItemList;
    public List<int> totalProgressBoosterList;
    public ItemType itemType;
    public int itemId;

    #region Item
    public float GetPercentageItem()
    {
        return (float)DataManager.Ins.playerData.currentProgressItem / (float)DataManager.Ins.playerData.totalProgressItemList.First();
    }

    public bool InitItemTypeAndItemId()
    {
        if (DataManager.Ins.playerData.isReceiveHeartSticker)
        {
            itemType = ItemType.Sticker;
            itemId = 4;
            DataManager.Ins.playerData.isReceiveHeartSticker = false;
            return true;
        }

        int loopRandomTypeCount = 1000;

    RandomType:
        loopRandomTypeCount -= 1;
        itemType = (ItemType)UnityEngine.Random.Range(0, 3);
        int maxQuantity=0;
        switch (itemType)
        {
            case ItemType.Skin:
                maxQuantity = Enum.GetValues(typeof(SkinType)).Length;
                break;
            case ItemType.Sticker:
                maxQuantity = Enum.GetValues(typeof(StickerType)).Length;
                break;
            case ItemType.Trail:
                maxQuantity = Enum.GetValues(typeof(TrailType)).Length;
                break;
            default:
                break;
        }
        if (DataManager.Ins.playerData.lastItemTypeReceivedInProgressChest.Equals(itemType) || DataManager.Ins.playerData.buyedItemIdList[(int)itemType].list.Count == maxQuantity)
        {
            if(loopRandomTypeCount > 0)
                goto RandomType;
        }
        if(loopRandomTypeCount <= 0) // đã sở hữu hết item trong shop
        {
            return false;
        }
        RandomId:
        itemId = UnityEngine.Random.Range(0, maxQuantity);
        while (DataManager.Ins.playerData.buyedItemIdList[(int)itemType].list.Contains(itemId))
        {
            goto RandomId;
        }

        return true;
    }

    public bool IsProgressItemCollected()
    {
        return DataManager.Ins.playerData.currentProgressItem == DataManager.Ins.playerData.totalProgressItemList.First();
    }
    #endregion

    #region Booster
    public float GetPercentageBooster()
    {
        return (float)DataManager.Ins.playerData.currentProgressBooster / (float)DataManager.Ins.playerData.totalProgressBoosterList.First();
    }

    public bool IsProgressBoosterCollected()
    {
        return DataManager.Ins.playerData.currentProgressBooster == DataManager.Ins.playerData.totalProgressBoosterList.First();
    }
    #endregion
}

[Serializable]
public class ProgressChest
{
    public ItemType itemType;
    public int totalPassLevel;
    public int currentPassLevel;
    public bool isCollected => currentPassLevel == totalPassLevel;
    public int itemId;

    public float GetPercentage()
    {
        return (float)currentPassLevel / (float)totalPassLevel;
    }
}
