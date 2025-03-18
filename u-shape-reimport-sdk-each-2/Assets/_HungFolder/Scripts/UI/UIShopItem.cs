using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopItem : RectUnit
{
    public enum State
    {
        Lock,
        Unlocked,
        Selecting,
    }
    public CanvasGroup canvasGroup;
    [Header("[BG]")]
    public GameObject lockBG;
    public GameObject unlockedBG;
    public GameObject selectedBG;
    [Header("[Unlock]")]
    public GameObject unlockGoldGO;
    public GameObject unlockGoldOkGO;
    public GameObject unlockAdsGO;
    [Header("[Text]")]
    public TextMeshProUGUI unlockGoldText;
    public TextMeshProUGUI unlockGoldOkText;

    public Image itemImage;

    State itemState;
    ShopItemData data;
    UIShop shop;

    string CostText
    {
        set
        {
            unlockGoldText.text = value;
            unlockGoldOkText.text = value;
        }
    }
    public State ItemState
    {
        set
        {
            itemState = value;
            OnReset();
            switch (value)
            {
                case State.Lock:
                    itemImage.color = Color.gray;
                    lockBG.SetActive(true);
                    switch (data.purchaseMethod)
                    {
                        case PurchaseMethod.Gold:
                            CostText = data.cost.ToString();
                            (data.cost > DataManager.Ins.playerData.gold ? unlockGoldGO : unlockGoldOkGO)
                                .SetActive(true);
                            break;
                        case PurchaseMethod.Ads:
                            CostText = data.cost.ToString();
                            unlockAdsGO.SetActive(true);
                            break;
                    }
                    break;
                case State.Unlocked:
                    itemImage.color = Color.white;
                    unlockedBG.SetActive(true);
                    break;
                case State.Selecting:
                    itemImage.color = Color.white;
                    selectedBG.SetActive(true);
                    break;
            }
        }
    }
    private void OnEnable()
    {
        Show(false);
    }
    void OnReset()
    {
        lockBG.SetActive(false);
        unlockedBG.SetActive(false);
        selectedBG.SetActive(false);

        unlockGoldGO.SetActive(false);
        unlockGoldOkGO.SetActive(false);
        unlockAdsGO.SetActive(false);
    }
    public void OnInit(UIShop shop, ShopItemData shopItemData)
    {
        this.shop = shop;
        data = shopItemData;
        itemImage.sprite = shopItemData.sprite;
        if (
            shop.CurShop == ShopType.Skin && (SkinType)shopItemData.ItemType == DataManager.Ins.playerData.skinType ||
            shop.CurShop == ShopType.Trail && (TrailType)shopItemData.ItemType == DataManager.Ins.playerData.trailType ||
            shop.CurShop == ShopType.Sticker && (StickerType)shopItemData.ItemType == DataManager.Ins.playerData.stickerType
            )
        {
            ItemState = State.Selecting;
        }
        else
        {
            ItemState = shopItemData.State;
        }
    }
    public void Show(bool value)
    {
        if (value)
        {
            DOVirtual.Float(0, 1, 0.5f, val => canvasGroup.alpha = val);
        }
        else
        {
            canvasGroup.alpha = 0;
        }
    }
    public void OnResetState()
    {
        ItemState = data.State;
    }
    public void OnSelected()
    {
        shop.OnSelectItem(this);
    }
    public void UnlockButton()
    {
        switch (data.purchaseMethod)
        {
            case PurchaseMethod.Gold:
                DataManager.Ins.playerData.gold -= data.cost;
                UISharedCanvas.Ins.UpdateGoldText();
                break;
            case PurchaseMethod.Ads:
                break;
        }

        OnReset();
        data.State = State.Unlocked;
        OnSelected();
    }
    public void AdUnlockButton()
    {

        OnSelected();
    }
}