using Castle.Core.Internal;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ComboManager : Singleton<ComboManager> 
{
    public List<LevelCombo> levelComboList = new List<LevelCombo>();
    public int currentLevelComboIndex
    {
        get {
            return LevelManager.Ins.currentStageIndex / 2;
        }
    } 
    public LevelCombo currentLevelCombo
    {
        get
        {
            //return levelComboList[currentLevelComboIndex];
            return levelComboList[0];
        }
    }
    public bool isStopCalculateGreenCountAfterBomb = false;

    private void Awake()
    {
        foreach (LevelCombo levelCombo in levelComboList)
        {
            levelCombo.OnInit();
        }
    }

    private void OnEnable()
    {
        EventManager.OnLoadStage1 += OnNewLevelStage1;
    }

    public void OnNewLevelStage1()
    {
        /*foreach(ComboAward ca in currentLevelCombo.comboAwardList)
        {
            ca.isReceived = false;
        }*/
    }

    public void CheckAward()
    {
        if (LevelManager.Ins.currentLevelIndex < 2) return;
        if (LevelManager.Ins.currentLevel.IsAllUshapesDisappear() || LevelManager.Ins.currentLevel.IsAllUshapesGreen())
        {
            return;
        }
        LevelCombo lc = levelComboList.First();
        foreach(ComboAward ca in currentLevelCombo.comboAwardList)
        {
            if (ca.move == DataManager.Ins.playerData.greenCount && !ca.isReceived)
            {
                ca.OnReceive();
                int idx = currentLevelCombo.comboAwardList.IndexOf(ca);
                ComboLabel cl = ComboBar.Ins.comboLabelList[idx];
                cl.OnReceive();
                
            }
        }
    }

    public IEnumerator I_CalculateGreenCountAfterExplosion(int numberToAdd)
    {
        Debug.Log("I_CalculateGreenCountAfterExplosion");
        int target = DataManager.Ins.playerData.greenCount + numberToAdd;
        isStopCalculateGreenCountAfterBomb = false;
        while(DataManager.Ins.playerData.greenCount < target)
        {
            CheckAward();
            DataManager.Ins.playerData.greenCount += 1;
            DataManager.Ins.playerData.greenCount = Mathf.Clamp(DataManager.Ins.playerData.greenCount, 0, currentLevelCombo.moveSum);
            CheckAward();
            if(isStopCalculateGreenCountAfterBomb) yield break;
            yield return new WaitForEndOfFrame();
        }
    }
    
    public void ResetComboLabels()
    {
        foreach (ComboAward ca in currentLevelCombo.comboAwardList)
        {
            ca.isReceived = false;
        }
        foreach (ComboLabel cl in ComboBar.Ins.comboLabelList) {
            cl.ShowCollectedCover(false);
        }
    }

}

[Serializable]
public class LevelCombo
{
    [HideInInspector]
    public int moveSum;
    [HideInInspector]
    public int awardCount;
    public Dictionary<int, ComboAwardType> moveTypeDict;

    [SerializeReference] public List<ComboAward> comboAwardList = new List<ComboAward> ();


    internal void OnInit()
    {
        InitMoveSum();
        InitAwardCount();
        InitMoveTypeDict();
    }

    public void InitMoveSum()
    {
        int max = 0;
        if (comboAwardList.IsNullOrEmpty()) return;
        foreach (ComboAward ca in comboAwardList)
        {
            max = Mathf.Max(max, ca.move);
        }
        moveSum = max;
    }
    public void InitAwardCount()
    {
        int count = 0;
        foreach (ComboAward ca in comboAwardList)
        {
            count++;
        }
        awardCount = count;
    }
    public void InitMoveTypeDict()
    {
        moveTypeDict = new Dictionary<int, ComboAwardType>();
        foreach (ComboAward ca in comboAwardList)
        {
            moveTypeDict.Add(ca.move, ca.comboAwardType);
        }
        moveTypeDict = moveTypeDict.OrderBy(x => x.Key)
                              .ToDictionary(x => x.Key, x => x.Value);
    
    }
}

[Serializable]
public class ComboAward
{
    public int move;
    public ComboAwardType comboAwardType;
    public bool isReceived;

    public virtual void OnReceive() {
        isReceived = true;
    }
}

[Serializable]
public class ComboAwardGold : ComboAward
{
    [SerializeField]
    public int goldCount;
                                                                                                  
    public ComboAwardGold()
    {
        comboAwardType = ComboAwardType.Gold;
    }

    public override void OnReceive() {
        base.OnReceive();
        DataManager.Ins.playerData.gold += goldCount;
    }
}

[Serializable]
public class ComboAwardBomb : ComboAward
{
    public int bombCount;

    public ComboAwardBomb()
    {
        comboAwardType = ComboAwardType.Bomb;
    }

    public override void OnReceive()
    {
        base.OnReceive();
        DataManager.Ins.playerData.bombCount += bombCount;
        UIManager.Ins.GetUI<Gameplay>().ShowBombCountOrAds();
        UIManager.Ins.GetUI<Gameplay>().bombCountContainer.EffectBounce(1.4f);
        //need to bounce bomb count
        Debug.Log("ON RECEIVE BOMB");
    }
}

[Serializable]
public class ComboAwardX2 : ComboAward
{
    public ComboAwardX2()
    {
        comboAwardType = ComboAwardType.X2;
    }

    public override void OnReceive()
    {
        base.OnReceive();
        DoublePick.Ins.isActive = true;
        Debug.Log("ON RECEIVE X2");
    }
}

[Serializable]
public class ComboAwardGift : ComboAward
{
    public ItemType itemType;
    //public int itemId;

    public ComboAwardGift()
    {
        comboAwardType = ComboAwardType.Gift;
    }

    public override void OnReceive()
    {
        base.OnReceive();

        ComboBar.Ins.EffectGiftFly();
       
        
        Debug.Log("ON RECEIVE GIFT");
    }
}

