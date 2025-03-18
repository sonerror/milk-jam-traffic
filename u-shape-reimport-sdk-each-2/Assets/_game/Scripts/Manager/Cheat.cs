using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheat : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            LevelManager.Ins.LoadNextLevel();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            SceneController.Ins.LoadCurrentScene();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SceneController.Ins.ChangeScene("Main", () =>
            {
                UIManager.Ins.CloseUI<Loading>();
                UIManager.Ins.bg.gameObject.SetActive(false);
                LevelManager.Ins.LoadLevel(0);
            }, true,true);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Time.timeScale = 1.0f;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Time.timeScale = 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            ++DataManager.Ins.playerData.currentStageIndex; 
            SceneController.Ins.ChangeScene("Main", () =>
            {
                UIManager.Ins.CloseUI<Loading>();
                UIManager.Ins.bg.gameObject.SetActive(false);
            }, true, true);
        }
        /*if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DataManager.Ins.playerData.skinType = SkinType.CakeBrown;
            SkinManager.Ins.ChangeSkin();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DataManager.Ins.playerData.skinType = SkinType.Purple;
            SkinManager.Ins.ChangeSkin();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DataManager.Ins.playerData.skinType = SkinType.Pink;
            SkinManager.Ins.ChangeSkin();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DataManager.Ins.playerData.stickerType = StickerType.Cycle;
            StickerManager.Ins.ChangeSticker();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            DataManager.Ins.playerData.stickerType = StickerType.Cross;
            StickerManager.Ins.ChangeSticker();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            DataManager.Ins.playerData.stickerType = StickerType.Cycle;
            StickerManager.Ins.ChangeSticker();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            DataManager.Ins.playerData.trailType = TrailType.None;
            TrailManager.Ins.ChangeTrail();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            DataManager.Ins.playerData.trailType = TrailType.Heart;
            TrailManager.Ins.ChangeTrail();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            DataManager.Ins.playerData.trailType = TrailType.Star;
            TrailManager.Ins.ChangeTrail();
        }*/
        if (Input.GetKeyDown(KeyCode.H))
        {
            SceneController.Ins.ChangeScene("Main", () =>
            {
                UIManager.Ins.CloseUI<Loading>();
                UIManager.Ins.bg.gameObject.SetActive(false);
                DataManager.Ins.playerData.currentStageIndex += 2;
                LevelManager.Ins.LoadLevel(DataManager.Ins.playerData.currentStageIndex);
            }, true, true);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (DoublePick.Ins.isPickingAll) {
                DoublePick.Ins.StopPickingAll();
            }
            else StartCoroutine(DoublePick.Ins.I_PickAll());
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(CheckLevelStuck.Ins.I_AutoPlayAllLevels());
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            CheckLevelStuck.Ins.AutoPlayAllLevels();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Time.timeScale = 2f;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            DataManager.Ins.playerData.currentStageIndex++;
            LevelManager.Ins.LoadLevel(DataManager.Ins.playerData.currentStageIndex);
        }
    }
}
