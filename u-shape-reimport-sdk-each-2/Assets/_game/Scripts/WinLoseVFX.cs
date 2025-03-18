using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseVFX : MonoBehaviour
{
    public MeshRenderer[] meshRenderers;
    public Tween[] fadeTweens;


    private void Awake()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        fadeTweens = new Tween[meshRenderers.Length];
        foreach (MeshRenderer mr in meshRenderers)
        {
            mr.material = new Material(MaterialCollection.Ins.GetMat(MatType.RedVFX));
            Color c = mr.material.color;
            c.a = 0f;
            mr.material.color = c;
        }
    }

    public void HideVFX()
    {
        foreach (Tween t in fadeTweens) t?.Kill();
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            MeshRenderer mr = meshRenderers[i];
            Color c = mr.material.color;
            c.a = 0f;
            mr.material.color = c;
        }
    }

    public void ShowRedVFX()
    {
        if (LevelManager.Ins.IsStage1()) return;
        foreach (Tween t in fadeTweens) t?.Kill();
        Color c = MaterialCollection.Ins.GetMat(MatType.RedVFX).color;
        c.a = 0.8f;
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            MeshRenderer mr = meshRenderers[i];
            mr.material.color = c;
            fadeTweens[i] = mr.material.DOFade(0f, 1f);
        }
    }

    public void ShowGreenVFX()
    {
        return;
        if (LevelManager.Ins.IsStage1()) return;
        foreach (Tween t in fadeTweens) t?.Kill();
        Color c = MaterialCollection.Ins.GetMat(MatType.GreenVFX).color;
        c.a = 0.7f;
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            MeshRenderer mr = meshRenderers[i];
            mr.material.color = c;
        }
    }
}
