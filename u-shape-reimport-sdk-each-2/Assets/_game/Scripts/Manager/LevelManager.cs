using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public Level[] levels;
    public Level currentLevel;
    public bool isLoadStage1;
    public int levelIndexToReturn = 20;
    public float durationStage1;
    public GameObject ushapePrefab;
    public bool isFinishInstantiateLevel;

    [SerializeField]
    public int currentStageIndex
    {
        get
        {
            if (DataManager.Ins.playerData.currentStageIndex >= LevelManager.Ins.levels.Length)
                return ((DataManager.Ins.playerData.currentStageIndex - LevelManager.Ins.levelIndexToReturn) % (LevelManager.Ins.levels.Length - LevelManager.Ins.levelIndexToReturn) + LevelManager.Ins.levelIndexToReturn);
            else return DataManager.Ins.playerData.currentStageIndex;
        }
    }
    [SerializeField]
    public int currentLevelIndex
    {
        get
        {
            return currentStageIndex / 2;
        }
    }


    //34
    /*for (int i = 0; i < 100; i++)
    {
        if(i< LevelManager.Ins.levels.Length)
        {
            Debug.Log("levelIndex at index " + i + " is " + (i).ToString());
            continue;
        }
        Debug.Log("levelIndex at index " + i + " is " + ((i-34) % (LevelManager.Ins.levels.Length - 34) + 34).ToString());
    }*/

    private void OnEnable()
    {
        EventManager.OnLoadNewScene += OnLoadNewScene;
    }

    private void OnDestroy()
    {
        EventManager.OnLoadNewScene -= OnLoadNewScene;
    }

    public void OnLoadNewScene()
    {
        if (SceneController.Ins.currentSceneName.Equals("Main") && !UIManager.Ins.IsOpened<Home>())
        {
            if (isLoadStage1)
            {
                isLoadStage1 = false;
                if (!LevelManager.Ins.IsStage1())
                {
                    --DataManager.Ins.playerData.currentStageIndex;
                }
            }
            LoadLevel(currentStageIndex);
        }
    }

    public void InstantiateLevel(int levelIndex)
    {
        isFinishInstantiateLevel = false;
        int realLevelIndex = levelIndex % levels.Length;
        currentLevel = Instantiate(levels[realLevelIndex]);
        if (realLevelIndex % 2 != 0) //level hard
        {
            LevelData levelData = LevelDataCollection.Ins.levelDatas[realLevelIndex / 2]; // chia 2 de lay level hard
            foreach (TransformData td in levelData.transformDataList)
            {
                //Assets/_game/Prefabs/ushape-Instance/UShape1.prefab
                //GameObject obj = Instantiate(Resources.Load<GameObject>("Assets/_game/Prefabs/ushape-Instance/UShape1.prefab")) as GameObject;

                //GameObject obj = Instantiate(ushapePrefab) as GameObject;
                //UShape u = PoolUshape.Ins.GetObject();
                UShape u = SimplePool.Spawn<UShape>(PoolType.UShape);
                u.TF.SetParent(null);
                u.model.OnInit();
                u.OnInit();
                u.TF.position = td.position;
                u.TF.rotation = Quaternion.Euler(td.eulerAngle);
                u.TF.localScale = td.scale;
                u.TF.SetParent(currentLevel.container);
            }
        }
        isFinishInstantiateLevel = true;
    }

    public void LoadLevel(int levelIndex)
    {
        DestroyCurrentLevel();
        InstantiateLevel(levelIndex);
        currentLevel.gameObject.SetActive(true);
        currentLevel.Init();

        //fire base
        #region old
        /*if (currentLevelIndex + 1 > DataManager.Ins.playerData.maxCheckPointStartLevel)
        {
            FirebaseManager.Ins.SendEvent("checkpoint_start_" + (currentLevelIndex + 1).ToString(),
                new Firebase.Analytics.Parameter("level", (currentLevelIndex + 1).ToString()));
            DataManager.Ins.playerData.maxCheckPointStartLevel = currentLevelIndex + 1;
            //Debug.Log("checkpoint_start_" + (currentLevelIndex + 1).ToString());
        }*/

        /*DataManager.Ins.playerData.levelPlayTimes[currentLevelIndex + 1] += 1;
        FirebaseManager.Ins.SendEvent("level_start",
            new Firebase.Analytics.Parameter("level_id", (currentLevelIndex + 1).ToString()),
            new Firebase.Analytics.Parameter("level_retry", (DataManager.Ins.playerData.levelPlayTimes[currentLevelIndex + 1]).ToString()));*/
        #endregion
        string level = (DataManager.Ins.playerData.currentStageIndex/2 + 1).ToString(); 
        string stage = (IsStage1() ? 1:2).ToString();
        FirebaseManager.Ins.SendEvent("level_start",
                new Firebase.Analytics.Parameter[] {
                    new Firebase.Analytics.Parameter("level", level),
                    new Firebase.Analytics.Parameter("stage", stage)
                });
        Debug.Log("level_start " + "level " + level + " " + "stage " + stage);
        if (DataManager.Ins.playerData.currentStageIndex/2 > DataManager.Ins.playerData.maxCheckPointStartIndex)
        {
            FirebaseManager.Ins.SendEvent("checkpoint_start",
                new Firebase.Analytics.Parameter[] {
                    new Firebase.Analytics.Parameter("level", level),
                    new Firebase.Analytics.Parameter("stage", stage)
                });
            Debug.Log("checkpoint_start " + "level " + level + " " + "stage " + stage);
            DataManager.Ins.playerData.maxCheckPointStartIndex = DataManager.Ins.playerData.currentStageIndex/2;
        }
        DataManager.Ins.playerData.stagePlayTimes[currentStageIndex] += 1;

    }

    public void LoadNextLevel()
    {
        LoadLevel(++DataManager.Ins.playerData.currentStageIndex % levels.Length);
    }

    public void DestroyCurrentLevel()
    {
        if (currentLevel)
        {
            Debug.LogError("desstroy");
            Destroy(currentLevel.gameObject);
            
        }
    }

    public void CheckWinLose()
    {
        if (SceneController.Ins.isLoadingNewScene) return;
        if (!currentLevel.isCountDown) return;

        if (currentLevel.IsAllUshapesDisappear() || currentLevel.IsAllUshapesGreen())
        {
            currentLevel.StopCountDown();
            CameraController.Ins.ResetZoom(() => WinVFX.Ins.Show());
            AudioManager.Ins.StopMusic();
            bool isSwitchStage2to1 = !IsStage1();
            if (isSwitchStage2to1) {
                DOVirtual.DelayedCall(2f, () =>
                {
                    MaxManager.ins.ShowInter("win", () =>
                    {
                        UIManager.Ins.OpenUI<Win>();
                    });
                    //firebase
                    if(DataManager.Ins.playerData.currentStageIndex > DataManager.Ins.playerData.maxStageWinIndex)
                    {
                        string level = (DataManager.Ins.playerData.currentStageIndex / 2 + 1).ToString();
                        string stage = (IsStage1() ? 1 : 2).ToString();
                        string levelRetry = (DataManager.Ins.playerData.stagePlayTimes[currentStageIndex] - 1).ToString();
                        string duration = ((int)currentLevel.GetTimePlayed()).ToString();
                        FirebaseManager.Ins.SendEvent("level_win",
                            new Firebase.Analytics.Parameter[] {
                                new Firebase.Analytics.Parameter("level", level),
                                new Firebase.Analytics.Parameter("stage", stage),
                                new Firebase.Analytics.Parameter("level_retry", levelRetry),
                                new Firebase.Analytics.Parameter("duration", duration)
                            });
                        Debug.Log("level_win level "+level+" stage "+stage+ " levelRetry " + levelRetry+ " duration " + duration);
                        DataManager.Ins.playerData.maxStageWinIndex = DataManager.Ins.playerData.currentStageIndex;
                    }

                    DataManager.Ins.playerData.currentStageIndex++;
                });
            }
            else
            {
                DOVirtual.DelayedCall(1.8f, () =>
                {
                    UIManager.Ins.OpenUI<PopUpWarningHardLevel>();
                    durationStage1 = currentLevel.GetTimePlayed();
                    //firebase
                    if (DataManager.Ins.playerData.currentStageIndex > DataManager.Ins.playerData.maxStageWinIndex)
                    {
                        string level = (DataManager.Ins.playerData.currentStageIndex / 2 + 1).ToString();
                        string stage = (IsStage1() ? 1 : 2).ToString();
                        string levelRetry = (DataManager.Ins.playerData.stagePlayTimes[currentStageIndex]-1).ToString();
                        string duration = ((int)currentLevel.GetTimePlayed()).ToString();
                        FirebaseManager.Ins.SendEvent("level_win",
                            new Firebase.Analytics.Parameter[] {
                                new Firebase.Analytics.Parameter("level", level),
                                new Firebase.Analytics.Parameter("stage", stage),
                                new Firebase.Analytics.Parameter("level_retry", levelRetry),
                                new Firebase.Analytics.Parameter("duration", duration)
                            }
                        );

                        Debug.Log("level_win level " + level + " stage " + stage + " levelRetry " + levelRetry + " duration " + duration);
                        DataManager.Ins.playerData.maxStageWinIndex = DataManager.Ins.playerData.currentStageIndex;
                    }
                });
                DOVirtual.DelayedCall(3.1f, () =>
                {
                    DataManager.Ins.playerData.currentStageIndex++;
                    SceneController.Ins.LoadCurrentScene(() =>
                    {
                    }, false, false);
                });
            }
            return;
        }

        if(LevelManager.Ins.currentLevel.countDownTime <= 0 && !LevelManager.Ins.currentLevel.IsAllUshapesGreen())
        {
            LevelManager.Ins.currentLevel.StopCountDown();
            /*MaxManager.ins.ShowInter("lose", () =>
            {
                UIManager.Ins.OpenUI<Lose>();
            });*/ 
            UIManager.Ins.OpenUI<Lose>();
            //firebase
            if (DataManager.Ins.playerData.currentStageIndex > DataManager.Ins.playerData.maxStageWinIndex)
            {
                string level = (DataManager.Ins.playerData.currentStageIndex / 2 + 1).ToString();
                string stage = (IsStage1() ? 1 : 2).ToString();
                string duration = ((int)currentLevel.GetTimePlayed()).ToString();
                FirebaseManager.Ins.SendEvent("level_fail",
                    new Firebase.Analytics.Parameter("level", level),
                    new Firebase.Analytics.Parameter("stage", stage),
                    new Firebase.Analytics.Parameter("duration", duration)
                );
                Debug.Log("level_fail level" + level + " stage " + stage + " duration " + duration);
            }
            Debug.Log("time out and lose");
            return;
        }

    }

    public bool IsStage1()
    {
        return DataManager.Ins.playerData.currentStageIndex % 2 == 0;
    }
}
