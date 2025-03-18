using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.VFX;
using AppsFlyerSDK;

public class GameManager : Singleton<GameManager>
{
    //[SerializeField] UserData userData;
    //[SerializeField] CSVData csv;
    //private static GameState gameState = GameState.MainMenu;
    // Start is called before the first frame update

    public bool isCheat = false;
    private bool isInitGame;

    protected void Awake()
    {
        //base.Awake();
        Input.multiTouchEnabled = true;
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //int maxScreenHeight = 1280;
        //float ratio = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
        //if (Screen.currentResolution.height > maxScreenHeight)
        //{
        //    Screen.SetResolution(Mathf.RoundToInt(ratio * (float)maxScreenHeight), maxScreenHeight, true);
        //}

        //csv.OnInit();
        //userData?.OnInitData();

        //ChangeState(GameState.MainMenu);
    }

    private void Start()
    {
        StartCoroutine(I_InitGame());
    }


    IEnumerator I_InitGame()
    {
        yield return new WaitUntil(
            () => (
            Ins != null
            && MaxManager.ins != null
            && FirebaseManager.Ins != null
            && AOAManager.ins != null
            && DataManager.Ins != null
            && UIManager.Ins != null
            && LevelManager.Ins != null
            && MaterialCollection.Ins != null
            && SceneController.Ins != null
            && SkinManager.Ins != null
            && StickerManager.Ins != null
            && TrailManager.Ins != null
            && BombManager.Ins != null
            && ComboManager.Ins != null
            && ProgressManager.Ins != null
            && UserInfoManager.Ins != null
            && AudioManager.Ins != null
            && DoublePick.Ins != null
            )
        );
        UIManager.Ins.OpenUI<Loading>();
        UIManager.Ins.bg.SetActive(false);

        ggmba_UMP.ins.CheckUMP();

        DataManager.Ins.LoadData();
        UserInfoManager.Ins.InitUserInfos();
        FirebaseManager.Ins.ConfirmGoogleServices();
        yield return new WaitForEndOfFrame();
        Debug.Log("before fetch remote config");
        yield return new WaitUntil(() => FirebaseManager.Ins.isFetchComplete);// fetch remote config
        Debug.Log("after fetch remote config");


        /*if (FirebaseManager.ins.GetValue_Boolean("AOA_FirstOpenApp") && DataManager.ins.playerData.isOpenFirst) AOAManager.ins.ShowAppOpenAd();
        else if (FirebaseManager.ins.GetValue_Boolean("AOA_OpenApp") && !DataManager.ins.playerData.isOpenFirst)
        {
            AOAManager.ins.ShowAppOpenAd();
        }*/
        AppsFlyerAdRevenue.start(AppsFlyerAdRevenueType.Generic);

        Debug.Log("before isCheckDependency");
        yield return new WaitUntil(() => FirebaseManager.Ins.isCheckDependency);
        Debug.Log("after isCheckDependency");

        /*if (AOAManager.ins.dummyAds != null)
        {
            AOAManager.ins.dummyAds.gameObject.transform.SetParent(DontDestroyOnLoadContainer.ins.canvas.transform);
            AOAManager.ins.dummyAds.gameObject.transform.SetAsLastSibling();
        }*/

        if (DataManager.Ins.playerData.isOpenFirst)
        {
            FirebaseManager.Ins.SendEvent("first_open");
            DataManager.Ins.playerData.isOpenFirst = false;
        }

        StartCoroutine(DataManager.Ins.CheckSession());

        NoInternet.SubscribeEvent();

        DoublePick.Ins.OnInit();

        isInitGame = true;
        yield return null;


        //AOAManager.ins.ShowAppOpenAd();
    }

}
