using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    public static  Action OnLoadNewScene;
    public static  Action OnLoadStage1;
    public static  Action OnLoadStage2;

    public static void TriggerLoadNewScene()
    {
        OnLoadNewScene?.Invoke();
    }

    public static void TriggerLoadStage1()
    {
        OnLoadStage1?.Invoke();
    }

    public static void TriggerLoadStage2()
    {
        OnLoadStage2?.Invoke();
    }
}
