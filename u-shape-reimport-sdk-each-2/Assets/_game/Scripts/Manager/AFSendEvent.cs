using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;

public class AFSendEvent : MonoBehaviour
{
    //public void InitSDK()
    //{
    //    AppsFlyer.initSDK("G3MBmMRHTuEpXbqyqSWGeK",null);
    //    AppsFlyer.startSDK();
    //    AppsFlyerAdRevenue.start(AppsFlyerAdRevenueType.Generic);
    //}

    public static void SendEvent(string nameEvent, Dictionary<string, string> dict = null)
    {
        if (dict == null)
        {
            dict = new Dictionary<string, string>();
        }
        AppsFlyer.sendEvent(nameEvent, dict);
        /*
            #region debug.log
                Debug.Log("===================================================");
                Debug.Log(nameEvent);
                foreach (KeyValuePair<string, string> items in dict)
                {
                    Debug.Log(items.Key.ToString() + ": " + items.Value.ToString());
                }
                Debug.Log("===================================================");
            }
            #endregion
        */
    }
}
