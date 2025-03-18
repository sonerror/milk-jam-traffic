using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShopTab : RectUnit
{
    public UIShop shop;
    public ShopType shopType;

    public void OnSelected()
    {
        shop.OnSelectTab(shopType);
    }
}
