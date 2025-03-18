﻿using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseManager : Singleton<FirebaseManager>
{
    public bool isFetchComplete = false; //không cần biết fetch thành công hay ko, cứ fetch xong là biến này thành true
    public bool isCheckDependency = false;
    public void ConfirmGoogleServices()
    {
        isCheckDependency = false;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                SetInAppDefaultValue();
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
                SetInAppDefaultValue();

            }
            isCheckDependency = true;
        });
    }

    public void SendEvent(string nameEvent = "event_null", params Firebase.Analytics.Parameter[] parammeters)
    {
        //SendEvent("Test1",new Firebase.Analytics.Parameter[] { 
        //    new Firebase.Analytics.Parameter("level","1")
        //});

#if !UNITY_EDITOR

        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(nameEvent, parammeters);
        }
        catch (Exception ex)
        {
            Debug.LogError("Firebase event : " + ex.ToString());
        }
#endif
    }
    public void SendEvent(string nameEvent = "event_null")
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(nameEvent);
        }
        catch (Exception ex)
        {
            Debug.LogError("lỗi firebase : " + ex.ToString());
        }
    }
    public void ADS_RevenuePain(ImpressionData data)
    {
        try
        {
            if (!Debug.isDebugBuild && !Application.isEditor)
            {
                Parameter[] AdParameters = {
                 new Parameter("ad_platform", "applovin"),
                 new Parameter("ad_source", data.NetworkName),
                 new Parameter("ad_unit_name", data.AdUnitIdentifier),
                 new Parameter("currency", "USD"),
                 new Parameter("value", data.Revenue),
                 new Parameter("placement", data.Placement),
                 new Parameter("country_code", data.CountryCode),
                 new Parameter("ad_format", data.AdFormat),
                };
                FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Lỗi ADS_RevenuePain:" + e.ToString());
        }
        try
        {
            if (!Debug.isDebugBuild && !Application.isEditor)
            {
                Parameter[] AdParameters = {
                 new Parameter("ad_platform", "applovin"),
                 new Parameter("ad_source", data.NetworkName),
                 new Parameter("ad_unit_name", data.AdUnitIdentifier),
                 new Parameter("currency", "USD"),
                 new Parameter("value", data.Revenue),
                 new Parameter("placement", data.Placement),
                 new Parameter("country_code", data.CountryCode),
                 new Parameter("ad_format", data.AdFormat),
                };
                FirebaseAnalytics.LogEvent("ad_impression_abi", AdParameters);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Lỗi ad_impression_abi:" + e.ToString());
        }
    }

    #region remote config
    private void SetInAppDefaultValue()
    {
        Dictionary<string, object> defaults =
         new Dictionary<string, object>();

        // These are the values that are used if we haven't fetched data from the
        // server
        // yet, or if we ask for values that the server doesn't have:
        defaults.Add("AOA_FirstOpenApp", false);
        defaults.Add("AOA_OpenApp", false);
        defaults.Add("AOA_SwitchApp", false);
        defaults.Add("AOA_SwitchAppTime", 0);

        FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
          .ContinueWithOnMainThread(task => {
              FetchDataAsync();
          });

    }
    // Start a fetch request.
    // FetchAsync only fetches new data if the current data is older than the provided
    // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
    // By default the timespan is 12 hours, and for production apps, this is a good
    // number. For this example though, it's set to a timespan of zero, so that
    // changes in the console will always show up immediately.
    public Task FetchDataAsync()
    {
        isFetchComplete = false;
        Debug.Log("Fetching data...");
        Task fetchTask =
        FirebaseRemoteConfig.DefaultInstance.FetchAsync(
            TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
        {
            Debug.LogError("Retrieval hasn't finished.");
            isFetchComplete = true;
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            isFetchComplete = true;
            return;
        }

        // Fetch successful. Parameter values must be activated to use.
        remoteConfig.ActivateAsync()
          .ContinueWithOnMainThread(
            task => {
                isFetchComplete = true;
                Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
            });
    }
    public bool GetValue_Boolean(string key)
    {
        return FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue;
    }
    public double GetValue_Double(string key)
    {
        return FirebaseRemoteConfig.DefaultInstance.GetValue(key).DoubleValue;
    }
    public long GetValue_Long(string key)
    {
        return FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue;
    }
    #endregion
}
