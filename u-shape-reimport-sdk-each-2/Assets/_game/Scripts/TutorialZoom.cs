using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialZoom : Singleton<TutorialZoom>
{
    public GameObject container;
    /*public RectTransform handLeft;
    public RectTransform handRight;*/
    public TutorialZoom2[] tutorialZoom2s;
    public bool isShowing => container.activeInHierarchy;
    public float existTime;


    private void OnEnable()
    {
        Hide();
        EventManager.OnLoadStage2 += Show;
        EventManager.OnLoadStage1 += HideIfUserIgnore;
    }

    private void OnDisable()
    {
        EventManager.OnLoadStage2 -= Show;
        EventManager.OnLoadStage1 -= HideIfUserIgnore;
    }

    private void Start()
    {
        existTime = -1f;
    }

    private void Update()
    {
        if(existTime > 0f)
        {
            existTime -= Time.deltaTime;
            if(existTime < 0f)
            {
                HideIfUserIgnore();
            }
        }
    }

    public void Show()
    {
        if (DataManager.Ins.playerData.isPassedTutorialZoom) return;
        if (LevelManager.Ins.currentLevelIndex < 1) return;
        container.SetActive(true);
        /*handLeft
            .DOAnchorPos(new Vector2(handLeft.anchoredPosition.x - 80f, handLeft.anchoredPosition.y), 0.7f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
        handRight
            .DOAnchorPos(new Vector2(handRight.anchoredPosition.x + 80f, handRight.anchoredPosition.y), 0.7f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);*/
        foreach (TutorialZoom2 tz2 in tutorialZoom2s) {
            tz2.OnInit();
        }
        existTime = 5f;
    }

    public void Hide()
    {
        container.SetActive(false);
    }

    public void HideIfUserIgnore()
    {
        if (isShowing)
        {
            Hide();
            DataManager.Ins.playerData.isPassedTutorialZoom = true;
        }
    }

}
