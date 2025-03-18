using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopType
{
    Skin,
    Trail,
    Sticker
}

public class UIShop : UICanvas
{
    public UIShopItem shopItemPrefab;
    public Transform itemParent;
    public MiniPool<UIShopItem> itemPool;

    Coroutine initCoroutine;
    public ShopType CurShop { get; private set; }

    private void Awake()
    {
        itemPool = new MiniPool<UIShopItem>();
        itemPool.OnInit(shopItemPrefab, 10, RTF);
    }
    private void OnEnable()
    {
        CollectItems();
    }
    UIShopItem SpawnItem()
    {
        UIShopItem item = itemPool.Spawn();
        item.RTF.SetParent(itemParent);
        return item;
    }
    void CollectItems()
    {
        foreach (var item in itemPool.Activating)
        {
            item.RTF.SetParent(RTF);
        }
        itemPool.Collect();
    }
    public override void Open()
    {
        base.Open();
        OnSelectTab(ShopType.Skin);
    }
    public void OnSelectTab(ShopType shopType)
    {
        if (CurShop == shopType && itemPool.Activating.Count > 0)
        {
            return;
        }

        CollectItems();
        CurShop = shopType;
        OnInit(shopType);
    }
    public void OnInit(ShopType shopType)
    {
        IEnumerator CreateItems<T>(List<ShopItemData<T>> itemDatas) where T : System.Enum
        {
            for (int i = 0; i < itemDatas.Count; i++)
            {
                UIShopItem item = SpawnItem();
                item.OnInit(this, itemDatas[i]);
            }

            foreach(var item in itemPool.Activating)
            {
                item.Show(true);
                yield return new WaitForSeconds(.1f);
            }
            initCoroutine = null;
        }

        if (initCoroutine != null)
        {
            StopCoroutine(initCoroutine);
        }

        switch (shopType)
        {
            case ShopType.Skin:
                initCoroutine = StartCoroutine(CreateItems(ShopItemSO.Ins.skins));
                break;
            case ShopType.Trail:
                initCoroutine = StartCoroutine(CreateItems(ShopItemSO.Ins.trails));
                break;
            case ShopType.Sticker:
                initCoroutine = StartCoroutine(CreateItems(ShopItemSO.Ins.stickers));
                break;
        }
    }

    public void OnSelectItem(UIShopItem item)
    {
        itemPool.Activating.ForEach(si => si.OnResetState());
        item.ItemState = UIShopItem.State.Selecting;
    }
}
