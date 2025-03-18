using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : RectUnit
{
    //public bool IsAvoidBackKey = false;
    public bool IsDestroyOnClose = false;
    public bool isAllButtonEffect = true;

    private void Start()
    {
        if (isAllButtonEffect)
        {
            AddEffectToButtons();
            AddSoundToButtons();
        }
    }

    public void AddEffectToButtons()
    {
        Button[] btns = GetComponentsInChildren<Button>(true);
        foreach (Button btn in btns)
        {
            btn.gameObject.AddComponent<ButtonEffect>();
        }
    }

    public void AddSoundToButtons()
    {
        Button[] btns = GetComponentsInChildren<Button>(true);
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() =>
            {
                AudioManager.Ins.PlaySoundFX(SoundType.ClickButton);
            });
        }
    }

    //Setup canvas to avoid flash UI
    //set up mac dinh cho UI de tranh truong hop bi nhay' hinh
    public virtual void Setup()
    {
        UIManager.Ins.AddBackUI(this);
        UIManager.Ins.PushBackAction(this, BackKey);
    }


    //back key in android device
    //back key danh cho android
    public virtual void BackKey()
    {

    }

    //Open canvas
    //mo canvas
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    //close canvas directly
    //dong truc tiep, ngay lap tuc
    public virtual void CloseDirectly()
    {
        UIManager.Ins.RemoveBackUI(this);
        gameObject.SetActive(false);
        if (IsDestroyOnClose)
        {
            Destroy(gameObject);
        }
        
    }

    //close canvas with delay time, used to anim UI action
    //dong canvas sau mot khoang thoi gian delay
    public virtual void Close(float delayTime)
    {
        Invoke(nameof(CloseDirectly), delayTime);
    }

}
