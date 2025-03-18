using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ColorStickersStruct
{
    public SkinType colorType;
    public Material[] stickers;
}


public class StickerManager : Singleton<StickerManager>
{
    public Mesh[] stickers;

    public Mesh GetSticker(StickerType type)
    {
        return stickers[(int)type];
    }

    /*private void OnEnable()
    {
        EventManager.OnLoadNewScene += () =>
        {
            DOVirtual.DelayedCall(0.15f, () =>
            {
                ChangeSticker();
            });
        };
    }*/

    public void ChangeSticker()
    {
        if (UIManager.Ins.IsOpened<Home>()) return; // neu mua trong scene Home
        //if (LevelManager.Ins.IsStage1()) return;

        UShape[] uShapes = LevelManager.Ins.currentLevel.uShapes;
        foreach (var u in uShapes)
        {
            if (u == null) continue;
            if (u.gameObject.activeInHierarchy && u.isFlying)
            {
                u.currentModelLocalPos = u.model.transform.localPosition;
            }
            /*if(DataManager.Ins.playerData.stickerType == StickerType.None)
            {
                u.sticker.transform.localPosition = new Vector3(0,-0.005f,0);
            }
            else
            {
                u.sticker.transform.localPosition = new Vector3(0, 0.0001f, 0);
            }*/
            u.sticker.transform.localPosition = new Vector3(0, 0.0001f, 0);
            u.sticker.mesh = GetSticker(DataManager.Ins.playerData.stickerType);
            u.OnFinishShopping();
        }
        //Debug.Log("CHANGE STICKER TO " + DataManager.Ins.playerData.stickerType.ToString().ToUpper());
    }
}
