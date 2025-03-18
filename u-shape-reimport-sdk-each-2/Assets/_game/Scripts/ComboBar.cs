using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ComboBar : Singleton<ComboBar>
{
    public Slider slider;
    public float sliderSpeed;
    public RectTransform rect;
    public float width;
    public Sprite goldSprite;
    public Sprite giftSprite;
    public ComboLabel comboLabelPrefab;
    public List<ComboLabel> comboLabelList = new List<ComboLabel>();
    public GameObject container;
    public RectTransform iconGiftRect;
    public Image iconGift;
    public Sprite iconGiftClose;
    public Sprite iconGiftOpen;



    public void Awake()
    {
        slider = GetComponent<Slider>();
        rect = GetComponent<RectTransform>();
        width = rect.sizeDelta.x;
    }

    private void OnEnable()
    {
        EventManager.OnLoadStage1 += OnStartStage1;
    }

    private void OnDisable()
    {
        EventManager.OnLoadStage1 -= OnStartStage1;
    }

    public void OnStartStage1()
    {
        if(LevelManager.Ins.currentLevelIndex < 2)
        {
            HideContainer();
            return;
        }
        if (!LevelManager.Ins.IsStage1()) return; // stage2
        if (ComboManager.Ins.currentLevelCombo.awardCount == 0)
        {
            HideContainer();
            return;
        }
        ShowContainer();
        StartCoroutine(I_RunSliderValue());
        //set vi tri cho cac combo-label
        if(comboLabelList.Count == 0)
        {
            for (int i = 0; i < ComboManager.Ins.currentLevelCombo.awardCount; i++)
            {
                int index = i;
                ComboLabel cl = Instantiate(comboLabelPrefab);
                cl.transform.SetParent(rect);
                cl.rect.anchoredPosition = Vector3.zero;
                cl.rect.localPosition -= new Vector3(width / 2, 0, 0);
                cl.rect.localScale = Vector3.one;
                cl.ShowCollectedCover(ComboManager.Ins.currentLevelCombo.comboAwardList[index].isReceived);
                comboLabelList.Add(cl);
            }
            comboLabelList.Last().rect.localPosition = new Vector3(9999f, 0, 0);
            int x = -1;
            foreach (var kvp in ComboManager.Ins.currentLevelCombo.moveTypeDict)
            {
                ++x;
                ComboLabel cl = comboLabelList[x];
                float percentage = (float)kvp.Key / (float)ComboManager.Ins.currentLevelCombo.moveSum;
                cl.rect.localPosition += new Vector3(width * percentage, 0, 0);
                switch (kvp.Value)
                {
                    case ComboAwardType.Gold:
                        ComboAwardGold cag = (ComboAwardGold)ComboManager.Ins.currentLevelCombo.comboAwardList.First(x => x.move == kvp.Key);
                        cl.SetValue(goldSprite, cag.goldCount.ToString(), kvp.Key);
                        cl.SetIconSizeY(70f);
                        break;
                    case ComboAwardType.Bomb:
                        //cl.SetValue(bombSprite, "", kvp.Key);
                        cl.SetIconSizeY(91f);
                        break;
                    case ComboAwardType.X2:
                        //cl.SetValue(x2Sprite, "x2", kvp.Key);
                        cl.SetIconSizeY(70f);
                        break;
                    case ComboAwardType.Gift:
                        cl.SetValue(giftSprite, "", kvp.Key);
                        cl.SetIconSizeY(80f);
                        break;
                    default:
                        break;
                }
            }
            comboLabelList.Last().HideWhiteLine();
        }
        iconGift.gameObject.SetActive(true);
    }

    public void InitIconGift()
    {
        //if (LevelManager.Ins.currentLevelIndex < 3) return;
        iconGiftRect.transform.SetParent(UIManager.Ins.GetUI<Gameplay>().transform);
        DataManager.Ins.playerData.iconGiftPos = iconGiftRect.position;
        iconGiftRect.transform.SetParent(this.transform);
        iconGift.gameObject.SetActive(false);
    }

    public void EffectGiftFly()
    {
        LevelManager.Ins.currentLevel.StopCountDown();
        UIManager.Ins.GetUI<Gameplay>().blackCover.SetActive(true);

        iconGiftRect.transform.SetParent(UIManager.Ins.GetUI<Gameplay>().transform);
        iconGiftRect.transform.SetSiblingIndex(UIManager.Ins.GetUI<Gameplay>().blackCover.transform.GetSiblingIndex() + 1);
        DataManager.Ins.playerData.iconGiftPos = iconGiftRect.position;

        iconGiftRect
            .DOAnchorPos(Vector2.zero, 1.5f)
            .SetEase(Ease.OutSine)
            ;

        iconGiftRect
            .DOScale(3f, 1.5f)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                //thay icon
                iconGift.sprite = iconGiftOpen;
                iconGiftRect.localScale = Vector3.one * 4.8f;
                DOVirtual.DelayedCall(0.15f, () =>
                {
                    UIManager.Ins.GetUI<Gameplay>().blackCover.SetActive(false);
                    int x2Count = UnityEngine.Random.Range(0, 3);
                    int bombCount = 2 - x2Count;
                    UIManager.Ins.OpenUI<PopUpComboAwardGift>();
                    UIManager.Ins.GetUI<PopUpComboAwardGift>().SetGridValue(x2Count, bombCount);
                    UIManager.Ins.GetUI<PopUpComboAwardGift>().SetTitle("COMBO AWARD");
                });
            });

    }

    public void ResetIconGift()
    {
        iconGiftRect.position = DataManager.Ins.playerData.iconGiftPos;
        iconGiftRect.localScale = Vector3.one;
        iconGift.sprite = iconGiftClose;
    }

    public IEnumerator I_RunSliderValue()
    {
        slider.value = (float)DataManager.Ins.playerData.greenCount / (float)ComboManager.Ins.currentLevelCombo.moveSum;
        while (true)
        {
            float newPercent = (float)DataManager.Ins.playerData.greenCount / (float)ComboManager.Ins.currentLevelCombo.moveSum;
            if (slider.value < newPercent)
            {
                float speedModifier = (newPercent - slider.value)*7f;
                slider.value += Time.deltaTime * sliderSpeed * (1+speedModifier);
                if(slider.value > newPercent) {
                    slider.value = newPercent;
                }
            }
            else if (slider.value > newPercent)
            {
                float speedModifier = /*((float)ComboManager.Ins.greenCount / (float)ComboManager.Ins.currentLevelCombo.moveSum - slider.value) **/ 7f;
                slider.value -= Time.deltaTime * sliderSpeed * (1 + speedModifier);
                if (slider.value < newPercent)
                {
                    slider.value = newPercent;
                }
            }
            yield return null;
        }
    }

    public void ShowContainer()
    {
        container.SetActive(true);
    }

    public void HideContainer()
    {
        container.SetActive(false);
    }
}
