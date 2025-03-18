using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopItemSO")]
public class ShopItemSO : ScriptableObject
{
    static ShopItemSO instance;
    public static ShopItemSO Ins
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.LoadAll<ShopItemSO>("SO")[0];
            }
            return instance;
        }
    }
    public List<ShopItemData<SkinType>> skins;
    public List<ShopItemData<TrailType>> trails;
    public List<ShopItemData<StickerType>> stickers;
}


public enum PurchaseMethod
{
    Gold,
    Ads
}

public abstract class ShopItemData
{
    public Sprite sprite;
    public PurchaseMethod purchaseMethod;
    public int cost;

    public abstract UIShopItem.State State { get; set; }
    public abstract System.Enum ItemType { get; }
    public static void SetState(System.Enum type, UIShopItem.State state) => PlayerPrefs.SetInt("ShopItem_" + type.ToString(), (int)state);
}

[System.Serializable]
public class ShopItemData<T> : ShopItemData where T : System.Enum
{
    public T type;

    public override UIShopItem.State State
    {
        get => (UIShopItem.State)PlayerPrefs.GetInt("ShopItem_" + type.ToString(), (int)UIShopItem.State.Lock);
        set => SetState(type, value);
    }
    public override System.Enum ItemType => type;
}