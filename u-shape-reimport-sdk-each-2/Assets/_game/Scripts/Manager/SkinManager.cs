using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public struct SkinModelsStruct
{
    public SkinType skinType;
    public List<UShape_Model> models;   
}

public class SkinManager : Singleton<SkinManager>
{
    private UShape_Model newModel;
    public MatType[] matTypes;

    private void OnEnable()
    {
        EventManager.OnLoadNewScene += () =>
        {
            StartCoroutine(I_OnLoadNewScene());
        };
    }

    public IEnumerator I_OnLoadNewScene()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if(!LevelManager.Ins.IsStage1()) yield return new WaitUntil(()=>LevelManager.Ins.isFinishInstantiateLevel);
        ChangeSkin();
        TrailManager.Ins.ChangeTrail();
    }


    public void ChangeSkin()
    {
        if (UIManager.Ins.IsOpened<Home>()) return; // neu mua trong scene Home
        UShape[] uShapes = LevelManager.Ins.currentLevel.uShapes;
        foreach (var u in uShapes)
        {
            if(u == null) continue; 
            if (u.gameObject.activeInHierarchy && u.isFlying)
            {
                u.currentModelLocalPos = u.model.transform.localPosition;
            }

            u.model.gameObject.SetActive(false);

            if (LevelManager.Ins.IsStage1())
            {
                newModel = ModelCollection.Ins.skinModels[(int)SkinType.None].models[(int)u.sizeType];
                u.model = Instantiate(newModel) as UShape_Model;
            }
            else
            {
                if (u.skinModelDict.ContainsKey(DataManager.Ins.playerData.skinType)) // ushape đã từng spawn model này
                {
                    u.model = u.skinModelDict[DataManager.Ins.playerData.skinType];
                    u.model.gameObject.SetActive(true);
                }
                else // ushape chưa từng spawn model này
                {
                    newModel = ModelCollection.Ins.skinModels[(int)DataManager.Ins.playerData.skinType].models[(int)u.sizeType];
                    u.model = Instantiate(newModel) as UShape_Model;
                }
            }

            u.model.OnInit();

            if (!LevelManager.Ins.IsStage1() && DataManager.Ins.playerData.skinType == SkinType.None)
            {
                u.model.meshRenderer.material = MaterialCollection.Ins.GetRandomMat(matTypes);
            }
            
            u.model.OnInit();
            u.OnFinishShopping();
        }
        StickerManager.Ins.ChangeSticker();
        TrailManager.Ins.ChangeTrail();
        //Debug.Log("CHANGE SKIN TO COLOR " + DataManager.Ins.playerData.skinType.ToString().ToUpper());
    }
}


