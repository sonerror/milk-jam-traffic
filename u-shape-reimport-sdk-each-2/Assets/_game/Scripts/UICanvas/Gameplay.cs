using Castle.Core.Internal;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : UICanvas
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI countDownText;
    
    public HandUI hand;
    public GameObject tutRotate;
    public GameObject[] stages;

    [Header("X2 : ")]
    public Transform btnDoublePick;
    public CanvasGroup btnDoublePickCanvasGroup;
    public GameObject x2AdsIcon;
    public TextMeshProUGUI x2CountText;
    public Transform x2CountContainer;
    public TextMeshProUGUI x2CountDownText;
    public GameObject x2CountObj;
    public GameObject x2CutOut;

    [Header("Bomb : ")]
    public Transform btnBomb;
    public CanvasGroup btnBombCanvasGroup;
    public TextMeshProUGUI bombCountText;
    public GameObject bombAdsIcon;
    public Transform bombCountContainer;
    public GameObject bombCutOut;


    [Header("=======================================================================")]
    public TextMeshProUGUI goldText;
    public GameObject blackCover;



    private void OnEnable()
    {
        EventManager.OnLoadNewScene += OnLoadNewScene;
    }

    private void OnDisable()
    {
        EventManager.OnLoadNewScene -= OnLoadNewScene;
    }

    private void Start()
    {
        if (GameManager.Ins.isCheat)
        {
            Button b = levelText.gameObject.AddComponent<Button>();
            b.onClick.AddListener(() =>
            {
                UIManager.Ins.OpenUI<CheatPanel>();
            });
        }
    }

    public override void Open()
    {
        base.Open();
        ShowX2CountOrAds();
        ShowBombCountOrAds();
        ComboBar.Ins.InitIconGift();
        blackCover.SetActive(false);
        UpdateBtnDoublePick();
        UpdateBtnBomb();
        tutRotate.SetActive(false);
    }

    public override void CloseDirectly()
    {
        base.CloseDirectly();
    }

    public void OnLoadNewScene()
    {
        UpdateLevelText();
        InitHand();
        ChangeStageImg();
        ShowOrHideBoosters();                   
    }

    public void ShowOrHideBoosters()
    {
        if(DataManager.Ins.playerData.isShowedX2) ShowOrHideBtnDoublePick();
        if(DataManager.Ins.playerData.isShowedBomb) ShowOrHideBtnBomb();
    }

    private void Update()
    {
        ShowOrHideX2CountObj();
        UpdateCountDownText();
        UpdateDoublePickCountDownText();
        goldText.text = DataManager.Ins.playerData.gold.ToString();
    }

    public void ShowOrHideX2CountObj()
    {
        if (DataManager.Ins.playerData.x2Count <= 0 && DoublePick.Ins.isActive)
        {
            x2CountObj.SetActive(false);
            return;
        }
        x2CountObj.SetActive(true);
    }

    public void UpdateBtnDoublePick()
    {
        if (btnDoublePick != null)
        {
            btnDoublePick.GetChild(0).localScale = DoublePick.Ins.isActive ? Vector3.one * 0.8f : Vector3.one;
        }

        if (btnDoublePickCanvasGroup != null)
        {
            btnDoublePickCanvasGroup.alpha = DoublePick.Ins.isActive ? 0.5f : 1f;
        }
    }

    public void UpdateBtnBomb()
    {
        if (btnBomb != null)
        {
            btnBomb.GetChild(0).localScale = MouseClicker.Ins.isUsingBomb ? Vector3.one * 0.8f : Vector3.one;
        }
        if (btnBombCanvasGroup != null)
        {
            btnBombCanvasGroup.alpha = MouseClicker.Ins.isUsingBomb ? 0.5f : 1f;
        }
    }

    public void ChangeStageImg()
    {
        if (LevelManager.Ins.IsStage1())
        {
            stages[0].gameObject.SetActive(true);
            stages[1].gameObject.SetActive(false); ;
        }
        else
        {
            stages[0].gameObject.SetActive(false);
            stages[1].gameObject.SetActive(true);
        }
    }

    private void UpdateLevelText()
    {
        levelText.text = "Level " + (DataManager.Ins.playerData.currentStageIndex/2 + 1).ToString() ;
    }

    private void UpdateCountDownText()
    {
        if (LevelManager.Ins.currentLevel == null) return;
        float currentTime = LevelManager.Ins.currentLevel.countDownTime;
        int minute = (int)currentTime / 60;
        int second = (int)currentTime % 60;
        string minuteString = minute.ToString();
        string secondString = second.ToString();
        if(minuteString.Length < 2)
        {
            minuteString = "0" + minuteString;
        }
        if (secondString.Length < 2)
        {
            secondString = "0" + secondString;
        }
        countDownText.text = minuteString + ":" + secondString;
    }

    public void InitHand()
    {
        if (hand == null) return;

        if (LevelManager.Ins.currentLevel.isTutorialClick && !DataManager.Ins.playerData.isPassedTutorialClick)
        {
            hand.gameObject.SetActive(true);
            hand.TutorialClick();
        }
        else if (LevelManager.Ins.currentLevel.isTutorialRotate && !DataManager.Ins.playerData.isPassedTutorialRotate)
        {
            DOVirtual.DelayedCall(Level.effectDuration + 0.1f, () =>
            {
                tutRotate.SetActive(true);
                hand.gameObject.SetActive(true);
                hand.SetUpPathPoints();
                hand.TutorialRotate();
            });
        }
        else
        {
            hand.gameObject.SetActive(false);
        }

        /*if(!DataManager.Ins.playerData.isPassedTutorialZoom && DataManager.Ins.playerData.currentLevelIndex == 3)
        {
            TutorialZoom.Ins.Show();
        }*/
    }

    public void Btn_Bomb()
    {
        if(DataManager.Ins.playerData.bombCount <= 0) //ads
        {
            //show reward
            MaxManager.ins.ShowReward("btn_bomb", "gameplay", () =>
            {
                DataManager.Ins.playerData.bombCount += 1;
                ShowBombCountOrAds();
                bombCountContainer.EffectBounce(1.4f);
            });
        }
        else
        {
            DataManager.Ins.playerData.isUsedBomb = true;
            if (HandBomb.Ins.hand.activeInHierarchy) // nếu đang tutorial bomb thì di chuyển vị trí hand qua ushape
            {
                UShape u = LevelManager.Ins.currentLevel.uShapes.Find(x => !x.isFlying);
                bombCutOut.gameObject.transform.SetParent(HandBomb.Ins.transform);
                HandBomb.Ins.MoveToUshape();
            }
            MouseClicker.Ins.isUsingBomb = true;
            UIManager.Ins.GetUI<Gameplay>().UpdateBtnBomb();
            //firebase
            string level = (LevelManager.Ins.currentLevelIndex + 1).ToString();
            string stage = (LevelManager.Ins.currentStageIndex + 1).ToString();
            FirebaseManager.Ins.SendEvent("booster_spend",
                new Firebase.Analytics.Parameter("booster","bomb"),
                new Firebase.Analytics.Parameter("level", level),
                new Firebase.Analytics.Parameter("stage", stage)
                );
        }
    }

    public void Btn_DoublePick()
    {
        if (DoublePick.Ins.isActive) return;
        if (DataManager.Ins.playerData.x2Count <= 0) //ads
        {
            //show reward
            MaxManager.ins.ShowReward("btn_x2", "gameplay", () =>
            {
                DataManager.Ins.playerData.x2Count += 1;
                ShowX2CountOrAds();
                x2CountContainer.EffectBounce(1.4f);
            });
        }
        else                        
        {
            DataManager.Ins.playerData.x2Count -= 1;
            DataManager.Ins.playerData.isUsedX2 = true;
            HandX2.Ins.Hide();
            ShowX2CountOrAds();
            DoublePick.Ins.isActive = true;
            DoublePick.Ins.elapsedTime = DoublePick.Ins.initialTime;
            UIManager.Ins.GetUI<Gameplay>().UpdateBtnDoublePick();
            DoublePick.Ins.elapsedTime = DoublePick.Ins.initialTime;
            //firebase
            string level = (LevelManager.Ins.currentLevelIndex + 1).ToString();
            string stage = (LevelManager.Ins.currentStageIndex + 1).ToString();
            FirebaseManager.Ins.SendEvent("booster_spend",
                new Firebase.Analytics.Parameter("booster", "double_pick"),
                new Firebase.Analytics.Parameter("level", level),
                new Firebase.Analytics.Parameter("stage", stage)
                );
        }
        ShowCutOutX2(false);
        LevelManager.Ins.currentLevel.OnContinue(0f);
    }

    public void BtnHome()
    {
        if (LevelManager.Ins.currentLevel.IsAllUshapesDisappear() || LevelManager.Ins.currentLevel.IsAllUshapesGreen()) return;
        if (BombManager.Ins.isThrowingBomb) return;
        LevelManager.Ins.currentLevel.StopCountDown();
        SceneController.Ins.ChangeScene("Main", () => {
            UIManager.Ins.CloseAll();
            UIManager.Ins.OpenUI<Home>();
            if (!LevelManager.Ins.IsStage1()) DataManager.Ins.playerData.currentStageIndex -= 1;
        }, true, true);
        AudioManager.Ins.StopMusic();
    }

    public void ShowX2CountOrAds()
    {
        if (DataManager.Ins.playerData.x2Count <= 0)
        {
            x2AdsIcon.SetActive(true);
        }
        else
        {
            x2AdsIcon.SetActive(false);
            x2CountText.text = DataManager.Ins.playerData.x2Count.ToString();
        }
    }

    public void ShowBombCountOrAds()
    {
        if(DataManager.Ins.playerData.bombCount <= 0)
        {
            bombAdsIcon.SetActive(true);   
        }
        else
        {
            bombAdsIcon.SetActive(false);
            bombCountText.text = DataManager.Ins.playerData.bombCount.ToString();
        }
    }

    public void ShowOrHideBtnDoublePick()
    {
        if (LevelManager.Ins.currentLevelIndex < 2)
        {
            btnDoublePick.gameObject.SetActive(false);
        }
        else
        {
            btnDoublePick.gameObject.SetActive(true);
            HandX2.Ins.Show();
            DataManager.Ins.playerData.isShowedX2 = true;
        }
    }

    public void ShowOrHideBtnBomb()
    {
        if (LevelManager.Ins.currentLevelIndex < 3)
        {
            btnBomb.gameObject.SetActive(false);
        }
        else
        {
            btnBomb.gameObject.SetActive(true);
            HandBomb.Ins.Show();
            DataManager.Ins.playerData.isShowedBomb = true;
        }
    }

    public void ShowCutOutX2(bool isShow)
    {
        x2CutOut.SetActive(isShow);
    }

    public void ShowCutOutBomb(bool isShow)
    {
        bombCutOut.SetActive(isShow);
    }

    private void UpdateDoublePickCountDownText()
    {
        float currentTime = DoublePick.Ins.elapsedTime;

        if(currentTime < 0)
        {
            x2CountDownText.text = "";
            return;
        }

        int minute = (int)currentTime / 60;
        int second = (int)currentTime % 60;
        string minuteString = minute.ToString();
        string secondString = second.ToString();
        if (minuteString.Length < 2)
        {
            minuteString = "0" + minuteString;
        }
        if (secondString.Length < 2)
        {
            secondString = "0" + secondString;
        }
        x2CountDownText.text = minuteString + ":" + secondString;
    }
}
