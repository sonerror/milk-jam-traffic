
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine.SceneManagement;
using System;

public class SwitchScene 
{
#if UNITY_EDITOR
    [MenuItem("ChangeScene/Loading #1")]
    public static void ChangeSceneLoading()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/_game/Scenes/Loading.unity");
        }
    }
    [MenuItem("ChangeScene/Main #2")]
    public static void ChangeSceneCanYouMatch()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/_game/Scenes/Main.unity");
        }
    }
    [MenuItem("ChangeScene/HungTest #3")]
    public static void ChangeSceneHungTest()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/_HungFolder/Scenes/Test.unity");
        }
    }
#endif

}