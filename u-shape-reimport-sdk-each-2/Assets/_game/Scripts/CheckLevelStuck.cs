using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CheckLevelStuck : Singleton<CheckLevelStuck>
{
    public Level level;



    public void AutoPlayAllLevels()
    {
        StartCoroutine(I_AutoPlayAllLevels());    
    }

    public IEnumerator I_AutoPlayAllLevels()
    {
        yield return new WaitForSeconds(0.1f);
        level = LevelManager.Ins.currentLevel;
        while (!LevelManager.Ins.currentLevel.IsAllUshapesDisappear())
        {
            PickAnotherUShape();
            yield return new WaitForSeconds(0.01f);
            yield return null;
        }
        SceneController.Ins.ChangeScene("Main", () =>
        {
            ++DataManager.Ins.playerData.currentStageIndex;
            ++DataManager.Ins.playerData.currentStageIndex;
            LevelManager.Ins.LoadLevel(DataManager.Ins.playerData.currentStageIndex);
            CheckLevelStuck.Ins.AutoPlayAllLevels();
        }, true, true);
    }

    public void PickAnotherUShape()
    {
        foreach (UShape u in LevelManager.Ins.currentLevel.uShapes)
        {
            if (u != null && u.gameObject.activeInHierarchy && !u.isFlying && !u.IsCollideOtherUshapesOnWay())
            {
                Destroy(u.gameObject);
                return;
            }
        }
        foreach (UShape u in LevelManager.Ins.currentLevel.uShapes)
        {
            if (u != null && u.gameObject.activeInHierarchy && !u.isFlying && u.IsCollideOtherUshapesOnWay())
            {
                Debug.Log("STUCK level " + LevelManager.Ins.currentLevel.name + " parent " + u.transform.parent.name + " position " + u.TF.localPosition);
                Destroy(u.gameObject);
                return;
            }
        }
    }
}
