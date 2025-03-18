using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    public Camera cam;




    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void ResetZoom(Action onComplete)
    {
            cam.orthographicSize = 11.6f;
            onComplete?.Invoke();
    }

}
