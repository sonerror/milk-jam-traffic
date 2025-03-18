using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Page
{
    public ItemType itemType;
    public Button[] buttons;
}

public class Shop : UICanvas
{
    public TextMeshProUGUI titleText;
    public Button[] tabBtns;
    public CanvasGroup[] tabBtnCanvasGroups;
    public GameObject[] itemContainers;
    public Page[] pages;
    public UIScaleEffect uIScaleEffect;
    public TextMeshProUGUI goldText;

    public Sprite usingItemSprite;
    public Sprite buyedItemSprite;
    public Sprite notBuyedItemSprite;

    public Sprite whiteTabSprite;
    public Sprite orangeTabSprite;

    [Header("Buy Buttons")]
    public GameObject buyButtonsObj;
    public TextMeshProUGUI priceText;
    public Sprite greenButtonBg;
    public Sprite greyButtonBg;
    public Image buttonBg;
    public Button buttonBuy;

    public ItemButton choosingItemButton;
    public ItemType choosingItemType;
    public int choosingItemId;





    private void Awake()
    {
        uIScaleEffect = GetComponent<UIScaleEffect>();
        AddListenerToTabButtons();
        AddListenerToItemButtons();
    }

    private void Update()
    {
        goldText.text = DataManager.Ins.playerData.gold.ToString();
    }

    /*public override void CloseDirectly()
    {
        uIScaleEffect.EffectClose(() =>
        {
            base.CloseDirectly();
        });
    }*/

    public override void Open()
    {
        base.Open();
        uIScaleEffect.EffectOpen(null);
        choosingItemType = ItemType.Trail;
        ShowContainer(0);
    }

    public void AddListenerToTabButtons()
    {
        for (int i = 0; i < tabBtns.Length; i++)
        {
            int index = i;
            Button b = tabBtns[index];
            b.onClick.AddListener(() =>
            {
                if (uIScaleEffect.isDoingEffect) return;

                ShowContainer(index);
                ChangeTitleText(index);
            });
        }
    }

    public void ShowContainer(int containerIndex)
    {
        if (choosingItemType == (ItemType)containerIndex) return;
        //tab buttons
        for (int i = 0; i < tabBtns.Length; i++)
        {
            Image tabButtonIcon = tabBtns[i].GetComponentInChildren<Image>();
            tabButtonIcon.sprite = orangeTabSprite;
        }

        Image tabButtonIconChoosing = tabBtns[containerIndex].GetComponentInChildren<Image>();
        tabButtonIconChoosing.sprite = whiteTabSprite;

        //item containers;
        for (int i = 0; i < itemContainers.Length; i++)
        {
            itemContainers[i].gameObject.SetActive(false);
        }
        itemContainers[containerIndex].gameObject.SetActive(true);

        //show selected
        for (int i = 0; i < pages[(int)containerIndex].buttons.Length; i++)
        {
            Image img = pages[(int)containerIndex].buttons[i].GetComponent<Image>();
            img.transform.localScale = Vector3.one;
            img.color = Color.white;
        }
        //hide labels of buyed items
        HideLabelsAndSetBgs();
        SetItemBgs((ItemType)containerIndex, DataManager.Ins.playerData.usingItemIds[containerIndex]);
        HideAllSelected((ItemType)containerIndex);
        ShowSelected((ItemType)containerIndex, DataManager.Ins.playerData.usingItemIds[containerIndex]);

        buyButtonsObj.SetActive(false);
        choosingItemButton = null;
        choosingItemType = (ItemType)containerIndex;
        choosingItemId = -1;
    }

    public void ChangeTitleText(int tabIdx)
    {
        switch(tabIdx)
        {
            case 0:
                titleText.text = "Skins";
                break;
            case 1:
                titleText.text = "Stickers";
                break;
            case 2:
                titleText.text = "Trails";
                break;
            case 3:
                titleText.text = "Remove Ads";
                break;
            default:break;
        }
    }

    public void BtnBack()
    {
        UIManager.Ins.CloseUI<Shop>();
    }

    public void AddListenerToItemButtons()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            int tmpi = i;
            Page p = pages[tmpi];
            ItemType itemType = p.itemType;
            Button[] buttons = p.buttons;
            ItemButton[] itemButtons = new ItemButton[buttons.Length]; 
            for (int j = 0; j < buttons.Length; j++)
            {
                int tmpj = j;
                itemButtons[tmpj] = buttons[tmpj].GetComponent<ItemButton>();
                ItemButton ib = itemButtons[tmpj];
                
                Button b = buttons[tmpj];
                b.onClick.AddListener(() =>
                {
                    if (uIScaleEffect.isDoingEffect) return;

                    choosingItemButton = ib;
                    choosingItemId = tmpj;

                    Action ApplyItemsToUShapeModel = () =>
                    {
                        switch (itemType)
                        {
                            case ItemType.Skin:
                                DataManager.Ins.playerData.skinType = (SkinType)tmpj;
                                SkinManager.Ins.ChangeSkin();
                                break;
                            case ItemType.Sticker:
                                DataManager.Ins.playerData.stickerType = (StickerType)tmpj;
                                StickerManager.Ins.ChangeSticker();
                                break;
                            case ItemType.Trail:
                                DataManager.Ins.playerData.trailType = (TrailType)tmpj;
                                TrailManager.Ins.ChangeTrail();
                                break;
                            case ItemType.RemoveAds:
                                break;
                            default: break;
                        }
                    };

                    bool isBuyed = DataManager.Ins.playerData.buyedItemIdList[(int)itemType].list.Contains(tmpj);
                    if (isBuyed) {
                        DataManager.Ins.playerData.usingItemIds[(int)itemType] = tmpj;
                        HideLabelsAndSetBgs();
                        SetItemBgs(itemType, tmpj);
                        ApplyItemsToUShapeModel.Invoke();
                        buyButtonsObj.SetActive(false);
                    }
                    else
                    {
                        Apply = ApplyItemsToUShapeModel;
                        buyButtonsObj.SetActive(true);
                        priceText.text = ib.cost.ToString();
                        if(DataManager.Ins.playerData.gold >= ib.cost)
                        {
                            buttonBg.sprite = greenButtonBg;
                            buttonBuy.gameObject.GetComponent<ButtonEffect>().enabled = true;
                        }
                        else
                        {
                            buttonBg.sprite = greyButtonBg;
                            buttonBuy.gameObject.GetComponent<ButtonEffect>().enabled = false;
                        }
                    }

                    ShowSelected(itemType, tmpj);

                    /*if (ib.buyType == BuyType.Ads)
                    {
                        //show reward
                        MaxManager.ins.ShowReward("btn_item", "shop", () =>
                        {
                            OnFinishBuyItem?.Invoke();
                        });
                    }*/
                });
            }
        }
    }

    public void SetItemBgs(ItemType itemType, int itemId)
    {
        if (itemId < 0) return;
        for (int i = 0; i < pages[(int)itemType].buttons.Length; i++)
        {
            int index = i;
            ItemButton ib = pages[(int)itemType].buttons[index].GetComponent<ItemButton>();
            if (DataManager.Ins.playerData.buyedItemIdList[(int)itemType].list.Contains(index))
            {
                ib.SetItemBg(buyedItemSprite);
            }
        }
        pages[(int)itemType].buttons[itemId].GetComponent<ItemButton>().SetItemBg(usingItemSprite);
    }

    public void ShowSelected(ItemType itemType, int itemId)
    {
        if (itemId < 0) return;
        HideAllSelected(itemType);
        pages[(int)itemType].buttons[itemId].GetComponent<Image>().color = Color.white;
    }

    public void HideAllSelected(ItemType itemType)
    {
        for (int i = 0; i < pages[(int)itemType].buttons.Length; i++)
        {
            int index = i;
            Image img = pages[(int)itemType].buttons[index].GetComponent<Image>();
            img.color = new Color(255, 255, 255, 0.01f);
        }
    }

    public void HideLabelsAndSetBgs()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            int tmpi = i;
            Page p = pages[tmpi];
            Button[] buttons = p.buttons;
            ItemButton[] itemButtons = new ItemButton[buttons.Length];
            for (int j = 0; j < buttons.Length; j++)
            {
                int tmpj = j;
                itemButtons[tmpj] = buttons[tmpj].GetComponent<ItemButton>();
                ItemButton ib = itemButtons[tmpj];
                if (DataManager.Ins.playerData.buyedItemIdList[i].list.Contains(tmpj))
                {
                    ib.HideLabel();
                    ib.SetItemBg(buyedItemSprite);
                }
                else
                {
                    ib.SetItemBg(notBuyedItemSprite);
                }
            }
        }
    }

    public Action Apply;
    public void FinishBuyItem(ItemButton ib, ItemType type, int itemId)
    {
        ib.SetItemBg(usingItemSprite);
        DataManager.Ins.playerData.buyedItemIdList[(int)type].list.Add(itemId);
        DataManager.Ins.playerData.usingItemIds[(int)type] = itemId;
        HideLabelsAndSetBgs();
        SetItemBgs(type, itemId);
        buyButtonsObj.SetActive(false);
        UIManager.Ins.OpenUI<PopUpNewItem>();
        UIManager.Ins.GetUI<PopUpNewItem>().SetValue(type, ib.itemIcon.sprite);
        Apply?.Invoke();
        //ApplyItemsToUShapeModel.Invoke();
    }

    public void BtnBuyAds()
    {
        MaxManager.ins.ShowReward("btn_buy_item", "shop", () =>
        {
            FinishBuyItem(choosingItemButton, choosingItemType, choosingItemId);
        });
    }

    public void BtnBuyGold()
    {
        if (DataManager.Ins.playerData.gold >= choosingItemButton.cost)
        {
            DataManager.Ins.playerData.gold -= choosingItemButton.cost;
            FinishBuyItem(choosingItemButton, choosingItemType, choosingItemId);
        }
    }

}
