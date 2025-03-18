/*using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class StickerManager2 : Singleton<StickerManager2>
{
    public GameObject[] stickerPrefabs;
    public GameObject currentSticker;



    private void OnEnable()
    {
        EventManager.OnLoadNewScene += () =>
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                ChangeSticker();
            });
        };
    }


    public void ChangeSticker()
    {
        if (UIManager.Ins.IsOpened<Home>()) return; // neu mua trong scene Home
        UShape[] uShapes = LevelManager.Ins.currentLevel.uShapes;
        foreach (var u in uShapes)
        {
            if (u == null) continue;
            if (u.gameObject.activeInHierarchy && u.isFlying)
            {
                u.currentModelLocalPos = u.model.transform.localPosition;
            }
            if(u.sticker != null) u.sticker.gameObject.SetActive(false);
            // ushape đã từng spawn sticker này
            if (u.stickerObjDict.ContainsKey(DataManager.Ins.playerData.stickerType)) 
            {
                u.sticker = u.stickerObjDict[DataManager.Ins.playerData.stickerType];
                u.sticker.gameObject.SetActive(true);
            }
            else // ushape chưa từng spawn sticker này
            {
                currentSticker = stickerPrefabs[(int)DataManager.Ins.playerData.stickerType];
                u.sticker = Instantiate(currentSticker) as GameObject;
            }
            u.sticker.transform.SetParentAndReset(u.model.topPoint);
            u.sticker.transform.localScale = Vector3.one * 0.04f;
            u.model.OnInit();
            u.OnFinishShopping();
        }
        TrailManager.Ins.ChangeTrail();
        //Debug.Log("CHANGE SKIN TO COLOR " + DataManager.Ins.playerData.skinType.ToString().ToUpper());
    }
}


*/