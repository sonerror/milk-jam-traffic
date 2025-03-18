using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailManager : Singleton<TrailManager>
{
    public Material[] mats;


    public Material GetMat(TrailType type)
    {
        return mats[(int)type];
    }

    public void ChangeTrail()
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
            foreach (Trail t in u.trails)
            {
                t.particleSystem.GetComponent<ParticleSystemRenderer>().material = GetMat(DataManager.Ins.playerData.trailType);
            }
            u.model.OnInit();
            u.OnFinishShopping();
        }
        //Debug.Log("CHANGE TRAIL TO " + DataManager.Ins.playerData.trailType.ToString().ToUpper());
    }
}
