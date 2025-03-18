using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ggmba_UMP : MonoBehaviour
{
    public static ggmba_UMP ins;
    public bool isLoad = false;
    private void Awake()
    {
        ins = this;
    }
    public void CheckUMP()
    {
        Time.timeScale = 0;
        // Create a ConsentRequestParameters object.
        ConsentRequestParameters request = new ConsentRequestParameters();

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);

        if (ConsentInformation.CanRequestAds())
        {
            Debug.LogError("Load 1");
            LoadAd();
        }
    }
    void OnConsentInfoUpdated(FormError consentError)
    {
        if (consentError != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(consentError);
            LoadAd();
            return;
        }

        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        {
            if (formError != null)
            {
                // Consent gathering failed.
                UnityEngine.Debug.LogError(consentError);
                LoadAd();
                return;
            }

            // Consent has been gathered.
            if (ConsentInformation.CanRequestAds())
            {
                Debug.LogError("Load 2");
                LoadAd();
            }
        });
    }
    private void LoadAd()
    {
        if (isLoad) return;
        MaxManager.ins.InitMAXSDK();
        AOAManager.ins.InitAOA();
        Time.timeScale = 1;
        isLoad = true;
    }
}
