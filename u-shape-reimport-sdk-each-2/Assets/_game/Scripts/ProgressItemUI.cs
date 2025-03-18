using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressItemUI : MonoBehaviour
{
    public Slider slider;
    public ItemType type;
    public TextMeshProUGUI progressText;
    public static bool isRunningProgress;
    public GameObject chestClose;
    public GameObject chestOpen;




    public void OnOpen()
    {
        chestClose.SetActive(true);
        chestOpen.SetActive(false);
    }

    public void ChangeProgressText(bool isItem = true)
    {
        if (isItem)
        {
            progressText.text = DataManager.Ins.playerData.currentProgressItem.ToString() + "/" + DataManager.Ins.playerData.totalProgressItemList.First().ToString(); ;
        }
        else
        {
            progressText.text = DataManager.Ins.playerData.currentProgressBooster.ToString() + "/" + DataManager.Ins.playerData.totalProgressBoosterList.First().ToString(); ;
        }
    }

    public void RunProgress(float newValue, Action OnComplete)
    {
        isRunningProgress = true;
        float duration = 0.8f;
        DOVirtual.Float(slider.value, newValue, duration, value =>
        {
            slider.value = value;
        }).OnComplete(() =>
        {
            OnComplete?.Invoke();
            isRunningProgress = false;
            if(Mathf.Abs(newValue - 1f) < 0.05f)
            {
                chestClose.SetActive(false);
                chestOpen.SetActive(true);
            }
        });
    }

}
