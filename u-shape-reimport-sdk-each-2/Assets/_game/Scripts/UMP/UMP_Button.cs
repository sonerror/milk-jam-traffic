using GoogleMobileAds.Ump.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UMP_Button : MonoBehaviour
{
    [SerializeField, Tooltip("Button to show the privacy options form.")]
    private Button _privacyButton;
    public void CheckButton()
    {
        _privacyButton.gameObject.SetActive(ConsentInformation.PrivacyOptionsRequirementStatus ==
                    PrivacyOptionsRequirementStatus.Required);
    }
    /// <summary>
    /// Shows the privacy options form to the user.
    /// </summary>
    public void ShowPrivacyOptionsForm()
    {
        Debug.Log("Showing privacy options form.");

        ConsentForm.ShowPrivacyOptionsForm((FormError showError) =>
        {
            if (showError != null)
            {
                Debug.LogError("Error showing privacy options form with error: " + showError.Message);
            }
            // Enable the privacy settings button.
            if (_privacyButton != null)
            {
                _privacyButton.gameObject.SetActive(ConsentInformation.PrivacyOptionsRequirementStatus ==
                    PrivacyOptionsRequirementStatus.Required);
            }
        });
    }

}
