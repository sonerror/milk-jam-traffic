using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePick : Singleton<DoublePick>
{
    public bool isActive;
    public bool isPicked;
    public float waitTime;
    public bool isPickingAll;
    public float initialTime;
    public float elapsedTime;

   
    private void OnEnable()
    {
        EventManager.OnLoadStage1 += OnLoadStage1;
    }

    private void OnDisable()
    {
        EventManager.OnLoadStage1 -= OnLoadStage1;
    }

    public void OnLoadStage1()
    {
        isActive = false;
        elapsedTime = -1;
        UIManager.Ins.GetUI<Gameplay>().UpdateBtnDoublePick();
    }

    public void OnInit()
    {
        initialTime = (float)FirebaseManager.Ins.GetValue_Double("time_double_pick");
    }

    private void Update()
    {
       
        if (elapsedTime < 0)
        {
            isActive = false;
            UIManager.Ins.GetUI<Gameplay>().UpdateBtnDoublePick();
            return;
        }
        else
        {
            elapsedTime -= Time.deltaTime;
        }
    }

    public IEnumerator I_PickAll()
    {
        isPickingAll = true;
        yield return new WaitForSeconds(0.1f);
        while (!LevelManager.Ins.currentLevel.IsAllUshapesDisappear())
        {
            if(!isPickingAll) yield break;
            PickAnotherUShape();
            yield return new WaitForSeconds(0.001f);
        }
    }

    public void StopPickingAll()
    {
        isPickingAll = false;
        StopAllCoroutines();
    }

    public void PickAnotherUShape()
    {
        if (!isActive) return;

        isPicked = false;

        foreach (UShape u in LevelManager.Ins.currentLevel.uShapes)
        {
            if(u != null && u.gameObject.activeInHierarchy && !u.isFlying && !u.IsCollideOtherUshapesOnWay())
            {
                isPicked = true;
                UIManager.Ins.GetUI<Gameplay>().UpdateBtnDoublePick();

                DOVirtual.DelayedCall(waitTime/2, () =>
                {
                    u.Fly();
                });
                return;
            }
        }

        if (!isPicked) // trường hợp bị stuck thì đợi bay qua rồi pick
        {
            DOVirtual.DelayedCall(waitTime, () =>
            {
                foreach (UShape u in LevelManager.Ins.currentLevel.uShapes)
                {
                    if (u != null && u.gameObject.activeInHierarchy && !u.isFlying && !u.IsCollideOtherUshapesOnWay())
                    {
                        u.Fly();
                        isPicked = true;
                        UIManager.Ins.GetUI<Gameplay>().UpdateBtnDoublePick();
                        return;
                    }
                }
            });
        }
    }
}
