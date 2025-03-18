using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutorialZoom2 : MonoBehaviour
{
    public Transform[] path;
    public Vector3[] pathPoints;



    public void OnInit()
    {
        SetUpPathPoints();
        TutorialRotate();
    }

    public void TutorialRotate()
    {
        this.transform.DOKill();
        this.transform.position = pathPoints[0];
        this.transform
            .DOPath(pathPoints, 2.5f, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
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
