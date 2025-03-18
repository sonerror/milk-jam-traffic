using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public int id;
    public GameObject label;
    public GameObject priceContainer;
    public GameObject adsContainer;
    public BuyType buyType;
    public int cost;
    public Image itemBg;
    public Image itemIcon;
    public TextMeshProUGUI costText;





    private void Awake()
    {
        HideLabel();
        costText.text = cost.ToString();
    }

    public void ShowLabel(BuyType buyType)
    {
        label.SetActive(true);
        switch (buyType)
        {
            case BuyType.Gold:
                priceContainer.SetActive(true);
                adsContainer.SetActive(false);
                break;
            case BuyType.Ads:
                priceContainer.SetActive(false);
                adsContainer.SetActive(true); 
                break;
            default: break;
        }
    }

    public void HideLabel()
    {
        label.SetActive(false);
    }

    public void SetItemBg(Sprite bg)
    {
        itemBg.sprite = bg;
    }
}
