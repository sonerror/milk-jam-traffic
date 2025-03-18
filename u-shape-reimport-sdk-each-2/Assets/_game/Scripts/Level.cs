using DG.Tweening;
using Firebase.RemoteConfig;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public UShape[] uShapes;
    public int quantity;
    public bool isTutorialClick;
    public bool isTutorialRotate;
    public Transform container;
    [HideInInspector]
    public float initialCountDownTime = 100f;
    public float countDownTime = 100f;
    public bool isCountDown;

    [Header("Effect : ")]
    public float initialX;
    public float effectX;
    public static float effectDuration = 0.6f;
    public bool isDoingEffect;





    public void Init()
    {
        uShapes = GetComponentsInChildren<UShape>();
        quantity = uShapes.Length;
        InitCountDownTime();
        initialX = -3.12f;
        effectX = 11f;
        isCountDown = true;
        StartCoroutine(I_EffectNewLevel());
    }

    private void Update()
    {
        if (!isCountDown || countDownTime < 0) return;
        else if(!isDoingEffect)
            {
                countDownTime -= Time.deltaTime;
            }

        if (countDownTime < 0)
        {
            LevelManager.Ins.CheckWinLose();
            isCountDown=false;
        }
    }

    public void InitCountDownTime()
    {
        if(LevelManager.Ins.currentLevelIndex >= 3 && !LevelManager.Ins.IsStage1())
        {
            countDownTime = (float)FirebaseManager.Ins.GetValue_Double("time_stage_2");
        }
        initialCountDownTime = countDownTime;
    }

    public bool IsAllUshapesDisappear()
    {
        foreach (var shape in uShapes)
        {
            if (shape != null && shape.gameObject.activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsAllUshapesGreen()
    {
        foreach (var u in uShapes)
        {
            if (u == null) continue;
            if(u.gameObject.activeInHierarchy && !u.isFlySuccess) return false;
        }
        return true;
    }

    public IEnumerator I_EffectNewLevel()
    {
        if (!LevelManager.Ins.IsStage1()) // nếu là stage 2
        {
            yield return new WaitUntil(() => !SceneController.Ins.isLoadingNewScene);
            yield return new WaitForSeconds(0.1f);
            isDoingEffect = true;
            this.transform.position = new Vector3(effectX, this.transform.position.y, this.transform.position.z);
            this.transform
                .DOLocalMoveX(initialX, effectDuration)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    isDoingEffect = false;
                    UIManager.Ins.CloseUI<PopUpWarningHardLevel>();
                });
        }
        else
        {
            this.transform.position = new Vector3(initialX, this.transform.position.y, this.transform.position.z);
        }
        yield return null;
    }

    public void StopCountDown()
    {
        isCountDown = false;
    }

    public void OnContinue(float plusTime)
    {
        countDownTime += plusTime;
        isCountDown = true;
    }

    public float GetTimePlayed()
    {
        return initialCountDownTime - countDownTime; 
    }
}
