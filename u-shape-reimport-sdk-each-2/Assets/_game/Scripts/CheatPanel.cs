using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheatPanel : UICanvas
{
    public RectTransform rectTransform;

    public TMP_InputField txt_LevelCheat;
    public TMP_InputField txt_StageCheat;

    public FlexibleColorPicker fcp;

    public CanvasGroup canvasGroup;

    public Image bg;

    public static Color bgColor;

    public int currentLevel;
    public int currentStage;



    private void Awake()
    {
        canvasGroup = UIManager.Ins.GetUI<Gameplay>().gameObject.AddComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        EventManager.OnLoadNewScene += ChangeBgColor;
    }

    private void OnDisable()
    {
        EventManager.OnLoadNewScene -= ChangeBgColor;
    }

    public override void Open()
    {
        base.Open();
        rectTransform.anchoredPosition = Vector3.zero;
        LevelManager.Ins.currentLevel.StopCountDown();
    }

    private void Update()
    {
        ChangeBgColor();
    }

    public void ChangeBgColor()
    {
        if (bg == null)
        {
            bg = GameObject.FindGameObjectWithTag("gameplay-bg").GetComponent<Image>();
        }
        if (bg != null)
        {
            bg.sprite = null;
            bg.color = fcp.color;
            bgColor = bg.color;
        }
    }

    public void BtnShowHideUI()
    {
        if(canvasGroup.alpha > 0.5f)
        {
            canvasGroup.alpha = 0f;
        }
        else
        {
            canvasGroup.alpha = 1f;
        }
    }

    public void OnEndEditCheatLevel()
    {
        if (string.IsNullOrEmpty(txt_LevelCheat.text) || string.IsNullOrEmpty(txt_StageCheat.text))
        {
            return;
        }

        int newStageIndex = (int.Parse(txt_LevelCheat.text)-1) * 2;
        if((int.Parse(txt_StageCheat.text)) == 2)
        {
            newStageIndex += 1;
        }

        int newLevel = newStageIndex / 2;
        int newStage = int.Parse(txt_StageCheat.text);

        if (currentLevel == newLevel && currentStage == newStage)
        {
            return;
        }

        currentLevel = newLevel;
        currentStage = newStage;

        DataManager.Ins.playerData.currentStageIndex = newStageIndex;
        SceneController.Ins.LoadCurrentScene();
        MoveFarAway();
    }
    public void BtnClose()
    {
        OnEndEditCheatLevel();
        MoveFarAway();
    }

    public void MoveFarAway()
    {
        rectTransform.anchoredPosition = new Vector3(99999f, 0, 0);
    }
}
