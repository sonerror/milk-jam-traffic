using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UShape_Model : GameUnit
{
    public UShape_Raycasts uShape_Raycasts;
    public Transform topPoint;
    public Transform[] bars;
    public WinLoseVFX vfx;
    public SkinType skinType;
    public SizeType sizeType;
    public Rigidbody rb;
    public List<BoxCollider> boxColliderList;
    public MeshRenderer meshRenderer;
    public bool isFinishRandomMaterial;



    private void Awake()
    {
        isFinishRandomMaterial = false;
    }

    public void OnInit()
    {
        uShape_Raycasts = GetComponentInChildren<UShape_Raycasts>();
        topPoint = this.TF.GetChildWithTag("top-point");
        bars = this.TF.GetChildsWithTag("bar");
        vfx = GetComponentInChildren<WinLoseVFX>();
        vfx.transform.localPosition = Vector3.zero; 
        //vfx.HideVFX();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        boxColliderList = new List<BoxCollider>();
        foreach(Transform b in bars)
        {
            BoxCollider collider = b.GetComponent<BoxCollider>();
            boxColliderList.Add(collider); 
        }
        SetAllBoxCollidersIsTrigger(true);
        meshRenderer = GetComponent<MeshRenderer>();
        RandomMaterial();
    }

    public void SetAllBoxCollidersIsTrigger(bool isTrigger = true)
    {
        foreach(BoxCollider collider in boxColliderList)
        {
            collider.isTrigger = isTrigger;
        }
    }

    public void RandomMaterial()
    {
        if (isFinishRandomMaterial || DataManager.Ins==null || !LevelManager.Ins.IsStage1()) return;
        meshRenderer.material = MaterialCollection.Ins.mats[UnityEngine.Random.Range(1, MaterialCollection.Ins.mats.Length - 2)];
        isFinishRandomMaterial = true;
    }
}
