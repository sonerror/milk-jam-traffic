using AppsFlyerSDK;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MaxManager : MonoBehaviour
{
    #region singleton
    public static MaxManager ins;
    private void Awake()
    {
        if (ins != null)
        {
            Destroy(this);
            return;
        }
        ins = this;
    }
    #endregion

#if UNITY_IOS
    string bannerAdUnitId = "YOUR_IOS_BANNER_AD_UNIT_ID"; // Retrieve the ID from your account
    string inter_adUnitId = "YOUR_IOS_AD_UNIT_ID";
    string reward_adUnitId = "YOUR_IOS_AD_UNIT_ID";
    string mrecAdUnitId = "YOUR_IOS_AD_UNIT_ID"; // Retrieve the ID from your account
#else // UNITY_ANDROID
    string bannerAdUnitId = "e8fd27e34708a28f";
    string inter_adUnitId = "7f1d8a1d179f482c";
    string reward_adUnitId = "7c71f7fd37501da5";
    string mrecAdUnitId = "317423ce8fd2deb2";
    /*string bannerAdUnitId = "111";
    string inter_adUnitId = "222";
    string reward_adUnitId = "333";
    string mrecAdUnitId = "444";*/
#endif
    int inter_retryAttempt;
    int reward_retryAttempt;

    public bool isRemoveAllAds = false;

    public bool isShowingAds = false;

    public float reloadAdsDelay;

    public float initialCappingInter;
    public float initialCappingInterAfterRwAds;
    public float currentCappingInter;





    public void InitMAXSDK()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            // AppLovin SDK is initialized, start loading ads
            InitializeBannerAds();
            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitializeMRecAds();
        };

        MaxSdk.SetSdkKey("ZoNyqu_piUmpl33-qkoIfRp6MTZGW9M5xk1mb1ZIWK6FN9EBu0TXSHeprC3LMPQI7S3kTc1-x7DJGSV8S-gvFJ");
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();

    }
    private void Update()
    {
        if (isRemoveAllAds) return;

        //capping inter
        if(currentCappingInter > 0f)
        {
            currentCappingInter -= Time.deltaTime; 
        }

    }

    #region Banner
    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerExtraParameter(bannerAdUnitId, "adaptive_banner", "true");

        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.white);

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;

        MaxSdk.LoadBanner(bannerAdUnitId);
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        ShowBanner(!isRemoveAllAds);
    }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) {
        MaxSdk.LoadBanner(bannerAdUnitId);
    }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("AdFormat", "Banner");
        dic.Add("AdUnitIdentifier", adInfo.AdUnitIdentifier);
        dic.Add("CountryCode", MaxSdk.GetSdkConfiguration().CountryCode);
        dic.Add("NetworkName", adInfo.NetworkName);
        dic.Add("Placement", adInfo.Placement);
        dic.Add("Revenue", adInfo.Revenue.ToString());
        AppsFlyerAdRevenue.logAdRevenue("ApplovinMax", AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax, adInfo.Revenue, "USD", dic);

        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "Banner";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = placement;
        data.Revenue = revenue;

        FirebaseManager.Ins.ADS_RevenuePain(data);

    }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    public void ShowBanner(bool isShow = true)
    {
        try
        {
            if (isRemoveAllAds) return;
            if (isShow) MaxSdk.ShowBanner(bannerAdUnitId);
            else MaxSdk.HideBanner(bannerAdUnitId);
        }
        catch (Exception ex)
        {
            Debug.LogError("lỗi banner : " + ex.ToString());
        }

    }
 
    #endregion

    #region Inter

    private Action OnCompleteInter = null;
    private string currentPlacementInter = "";
    private int interDisplayedTimes = 0;

    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialAdRevenuePaidEvent;

        currentCappingInter = -1f;

        // Load the first interstitial
        LoadInterstitial();

    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(inter_adUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        inter_retryAttempt = 0;
        FirebaseManager.Ins.SendEvent("ad_inter_load",
            new Firebase.Analytics.Parameter("scene", currentPlacementInter));
        AFSendEvent.SendEvent("af_inters_api_called");

    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        inter_retryAttempt++;
        double retryDelay = System.Math.Pow(2, System.Math.Min(6, inter_retryAttempt));

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Ins.SendEvent("ads_inter_displayed",
            new Firebase.Analytics.Parameter("scene", currentPlacementInter));
        AFSendEvent.SendEvent("af_inters_displayed");
        /*interDisplayedTimes += 1;
        if ((interDisplayedTimes >= 5 && interDisplayedTimes <= 10) || interDisplayedTimes == 15)
        {
            FirebaseManager.Ins.SendEvent("inter_ad_displayed_" + interDisplayedTimes.ToString(),
                new Firebase.Analytics.Parameter("scene", currentPlacementInter));
            //AFSendEvent.SendEvent("inter_ad_displayed_" + interDisplayedTimes.ToString());
        }*/
        isShowingAds = false;
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
        OnCompleteInter?.Invoke();
        FirebaseManager.Ins.SendEvent("ad_inter_fail",
            new Firebase.Analytics.Parameter("scene", currentPlacementInter));
        isShowingAds = false;
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Ins.SendEvent("ad_inter_click",
            new Firebase.Analytics.Parameter("scene", currentPlacementInter));
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
        currentCappingInter = initialCappingInter;
        OnCompleteInter?.Invoke();
        isShowingAds = false;
    }
    private void OnInterstitialAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("AdFormat", "Interstitial");
        dic.Add("AdUnitIdentifier", adInfo.AdUnitIdentifier);
        dic.Add("CountryCode", MaxSdk.GetSdkConfiguration().CountryCode);
        dic.Add("NetworkName", adInfo.NetworkName);
        dic.Add("Placement", adInfo.Placement);
        dic.Add("Revenue", adInfo.Revenue.ToString());
        AppsFlyerAdRevenue.logAdRevenue("ApplovinMax", AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax, adInfo.Revenue, "USD", dic);

        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "Interestial";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = placement;
        data.Revenue = revenue;

        FirebaseManager.Ins.ADS_RevenuePain(data);
    }

    public bool IsInterReady()
    {
        bool res = true;
        res &= !isRemoveAllAds;
        res &= currentCappingInter <= 0f;
        res &= LevelManager.Ins.currentLevelIndex >= 3;
        res &= MaxSdk.IsInterstitialReady(inter_adUnitId);
        return res;
    }

    public void ShowInter(string placement, Action OnComplete = null)
    {
        if (GameManager.Ins.isCheat)
        {
            OnComplete?.Invoke();
            return;
        }

        try
        {
            AFSendEvent.SendEvent("af_inters_ad_eligible");
            currentPlacementInter = placement;

            if (!IsInterReady())
            {
                OnComplete?.Invoke();
                LoadInterstitial();
                return;
            }
            else
            {
                FirebaseManager.Ins.SendEvent("ads_inter_start_show",
                    new Firebase.Analytics.Parameter("scene", currentPlacementInter));
                MaxSdk.ShowInterstitial(inter_adUnitId);
                isShowingAds = true;
                Debug.LogWarning("SHOW INTER");
                OnCompleteInter = OnComplete;
            }
        }
        catch (Exception ex)
        {
            OnComplete?.Invoke();
            Debug.LogError("lỗi inter : " + ex.ToString());
        }
    }
    #endregion

    #region Reward
    private System.Action OnRewardDone;
    private string currentNameEventRw = "";
    private string currentPlacementRw = "";
    private int rwDisplayedTimes = 0;

    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(reward_adUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        reward_retryAttempt = 0;
        AFSendEvent.SendEvent("af_rewarded_api_called");

    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        reward_retryAttempt++;
        double retryDelay = System.Math.Pow(2, System.Math.Min(6, reward_retryAttempt));


        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Ins.SendEvent("ads_reward_show",
            new Firebase.Analytics.Parameter("scene", currentPlacementRw),
            new Firebase.Analytics.Parameter("button_name", currentNameEventRw));
        AFSendEvent.SendEvent("af_rewarded_displayed");
        /*rwDisplayedTimes += 1;
        if ((rwDisplayedTimes >= 2 && rwDisplayedTimes <= 10) || rwDisplayedTimes == 15)
        {
            FirebaseManager.Ins.SendEvent("rw_ad_displayed_" + rwDisplayedTimes.ToString(),
                new Firebase.Analytics.Parameter("scene", currentPlacementRw));
            //AFSendEvent.SendEvent("rw_ad_displayed_" + rwDisplayedTimes.ToString());
        }*/
        isShowingAds = false;
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        FirebaseManager.Ins.SendEvent("ads_reward_fail",
            new Firebase.Analytics.Parameter("scene", currentPlacementRw),
            new Firebase.Analytics.Parameter("button_name", currentNameEventRw));
        LoadRewardedAd();
        isShowingAds = false;
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Ins.SendEvent("ads_reward_click",
            new Firebase.Analytics.Parameter("scene", currentPlacementRw),
            new Firebase.Analytics.Parameter("button_name", currentNameEventRw));
    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
        if(currentCappingInter < initialCappingInterAfterRwAds)
        {
            currentCappingInter = initialCappingInterAfterRwAds;
        }
        isShowingAds = false;
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        Debug.Log("Complete Reward");
        OnRewardDone?.Invoke();
        FirebaseManager.Ins.SendEvent("ads_reward_complete",
            new Firebase.Analytics.Parameter("scene", currentPlacementRw),
            new Firebase.Analytics.Parameter("button_name", currentNameEventRw));
        AFSendEvent.SendEvent("af_rewarded_ad_completed");
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("AdFormat", "Reward");
        dic.Add("AdUnitIdentifier", adInfo.AdUnitIdentifier);
        dic.Add("CountryCode", MaxSdk.GetSdkConfiguration().CountryCode);
        dic.Add("NetworkName", adInfo.NetworkName);
        dic.Add("Placement", adInfo.Placement);
        dic.Add("Revenue", adInfo.Revenue.ToString());
        AppsFlyerAdRevenue.logAdRevenue("ApplovinMax", AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax, adInfo.Revenue, "USD", dic);

        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "Reward";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = placement;
        data.Revenue = revenue;

        FirebaseManager.Ins.ADS_RevenuePain(data);
    }

    public bool IsRewardReady()
    {
        bool res = true;
        res &= !isRemoveAllAds;
        res &= MaxSdk.IsRewardedAdReady(reward_adUnitId);
        return res;
    }

    public void ShowReward(string nameEvent, string placement, System.Action OnComplete)
    {
        if(GameManager.Ins.isCheat)
        {
            OnComplete?.Invoke();
            return;
        }

        try
        {
            AFSendEvent.SendEvent("af_rewarded_ad_eligible");
         
            currentNameEventRw = nameEvent;
            currentPlacementRw = placement;

            if (IsRewardReady())
            {
                OnRewardDone = OnComplete;
            }

            if (GameManager.Ins.isCheat)
            {
                OnRewardDone?.Invoke();
                return;
            }
                
            MaxSdk.ShowRewardedAd(reward_adUnitId);
            isShowingAds = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("lỗi reward : " + ex.ToString());
        }
    }
    #endregion

    #region MREC
    public void InitializeMRecAds()
    {
        // MRECs are sized to 300x250 on phones and tablets
        MaxSdk.CreateMRec(mrecAdUnitId, MaxSdkBase.AdViewPosition.Centered);

        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnMRecAdExpandedEvent;
        MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnMRecAdCollapsedEvent;
    }

    public void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error) { }

    public void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("AdFormat", "MrecAds");
        dic.Add("AdUnitIdentifier", adInfo.AdUnitIdentifier);
        dic.Add("CountryCode", MaxSdk.GetSdkConfiguration().CountryCode);
        dic.Add("NetworkName", adInfo.NetworkName);
        dic.Add("Placement", adInfo.Placement);
        dic.Add("Revenue", adInfo.Revenue.ToString());
        AppsFlyerAdRevenue.logAdRevenue("ApplovinMax", AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax, adInfo.Revenue, "USD", dic);

        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "Mrec";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = placement;
        data.Revenue = revenue;

        FirebaseManager.Ins.ADS_RevenuePain(data);
    }

    public void OnMRecAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    public void ShowMREC(bool isShow = true)
    {
        try
        {
            if (isRemoveAllAds) return;
            if (isShow) MaxSdk.ShowMRec(mrecAdUnitId);
            else MaxSdk.HideMRec(mrecAdUnitId);
        }
        catch (Exception ex)
        {
            Debug.LogError("lỗi MREC : " + ex.ToString());
        }
    }
    #endregion
}
[System.Serializable]
public class ImpressionData
{
    public string CountryCode;
    public string NetworkName;
    public string AdUnitIdentifier;
    public string Placement;
    public double Revenue;
    public string AdFormat;
}