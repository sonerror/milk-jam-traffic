using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vibrator 
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
#endif

    public static bool IsAndroid()
    {
#if UNITY_ANDROID
        return true;
#else
        return false;
#endif
    }

    public static void Vibrate(long milisec = 250)
    {
        if (!DataManager.Ins.playerData.isVibrateOn) return;

        if(IsAndroid())
        {
            vibrator?.Call("vibrate", milisec);
        }
        else
        {
            Handheld.Vibrate();
        }
    }

    public static void Cancel()
    {
        if(IsAndroid())
        {
            vibrator.Call("cancel");
        }
    }
}
