using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Common;
using System;

public class AOAManager : MonoBehaviour
{
    public static AOAManager ins;
    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    //private string _adUnitId = "ca-app-pub-3940256099942544/3419835294";
    private string _adUnitId = "ca-app-pub-3940256099942544/9257395921";
#elif UNITY_IPHONE
        private string _adUnitId = "ca-app-pub-3940256099942544/5662855259";
#else
        private string _adUnitId = "unused";
#endif
    public bool isDropAOA = false;
    private AppOpenAd _appOpenAd;
    private int countAppChange = 0;

    public bool IsAdAvailable
    {
        get
        {
            return _appOpenAd != null;
        }
    }
    private void OnDestroy()
    {
        // Always unlisten to events when complete.
        AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
    }
    private void Awake()
    {
        // Use the AppStateEventNotifier to listen to application open/close events.
        // This is used to launch the loaded ad when we open the app.

        ins = this;
    }

    private void OnAppStateChanged(AppState state)
    {
        Debug.Log("App State changed to : " + state);

        // if the app is Foregrounded and the ad is available, show it.
        if (state == AppState.Foreground)
        {
            if (countAppChange > 0 && !MaxManager.ins.isShowingAds /*&& FirebaseManager.Ins.GetValue_Boolean("AOA_SwitchApp")*/) ShowAppOpenAd();
            countAppChange += 1;
        }
    }
    public void InitAOA()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {

            // This callback is called once the MobileAds SDK is initialized.
            LoadAppOpenAd();
            ShowAppOpenAd();
            Debug.Log("init AOA SDK");
        });
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }

    /// <summary>
    /// Loads the app open ad.
    /// </summary>
    public void LoadAppOpenAd()
  {
      // Clean up the old ad before loading a new one.
      if (_appOpenAd != null)
      {
            _appOpenAd.Destroy();
            _appOpenAd = null;
      }

      Debug.Log("Loading the app open ad.");

      // Create our request used to load the ad.
      var adRequest = new AdRequest();

      // send the request to load the ad.
      AppOpenAd.Load(_adUnitId, adRequest,
          (AppOpenAd ad, LoadAdError error) =>
          {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
              {
                  Debug.LogError("app open ad failed to load an ad " +
                                 "with error : " + error);
                  return;
              }

              Debug.Log("App open ad loaded with response : "
                        + ad.GetResponseInfo());

              _appOpenAd = ad;
              RegisterEventHandlers(ad);
          });
    }
    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("App open ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("App open ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("App open ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("App open ad failed to open full screen content " +
                           "with error : " + error);
        };
    }
    /// <summary>
    /// Shows the app open ad.
    /// </summary>
    public void ShowAppOpenAd()
    {
        if (MaxManager.ins.isRemoveAllAds) return;
        if (MaxManager.ins.isShowingAds) return;
        if (DataManager.Ins.playerData.isOpenFirst) return;
        isDropAOA = false;
        StartCoroutine(I_ShowAOA());
    }
    IEnumerator I_ShowAOA()
    {
        yield return new WaitUntil(() => _appOpenAd != null);
        if (!isDropAOA) _appOpenAd.Show();
    }
}
