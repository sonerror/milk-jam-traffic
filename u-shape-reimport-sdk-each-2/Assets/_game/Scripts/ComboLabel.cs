using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboLabel : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI moveText;
    public RectTransform rect;
    public Transform effectContainer;
    public Image bg;
    public GameObject line;
    public GameObject collected;



    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetValue(Sprite sprite, string txt, int moveCount)
    {
        icon.sprite = sprite;
        countText.text = txt;
        moveText.text = moveCount.ToString();
    }

    public void HideWhiteLine()
    {
        line.SetActive(false);
    }

    public void ShowCollectedCover(bool isShow=true)
    {
        collected.SetActive(isShow);
    }

    public void OnReceive()
    {
        ShowCollectedCover(true);
        effectContainer.EffectBounce(1.4f);
    }

    public void SetIconSizeY(float y)
    {
        icon.rectTransform.sizeDelta = new Vector2(icon.rectTransform.sizeDelta.x, y);   
    }
    
}
