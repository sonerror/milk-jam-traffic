using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    public Transform[] path;
    public Vector3[] pathPoints;

    private void Awake()
    {
        SetUpPathPoints();
    }

    public void TutorialClick()
    {
        this.transform.DOKill();
        this.transform
            .DOScale(0.8f, 0.5f)
            .SetEase(Ease.InSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void TutorialRotate()
    {
        this.transform.DOKill();
        this.transform.position = pathPoints[0];
        this.transform
            .DOPath(pathPoints, 5f, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                TutorialRotate();
            });
    }

    public void SetUpPathPoints()
    {
        pathPoints = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            pathPoints[i] = path[i].transform.position;
        }
    }
}
