using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.Utilities;
using DG.Tweening;

public class UserInfoManager : Singleton<UserInfoManager>
{
    public int randomUserQuantity;
    public UserInfo youUi => DataManager.Ins.playerData.userInfoList.Find(x => x.username == "You");




    public void InitUserInfos()
    {
        if (!DataManager.Ins.playerData.userInfoList.IsNullOrEmpty()) return;
        //random userinfo
        for (int i = 0; i < randomUserQuantity; i++)
        {
            UserInfo ui = new UserInfo();
            ui.username = GetRandomName();
            ui.score = GetRandomScore();
            DataManager.Ins.playerData.userInfoList.Add(ui);
        }
        //you userinfo
        UserInfo youUi = new UserInfo();
        youUi.username = "You";
        youUi.score = 10000;
        DataManager.Ins.playerData.userInfoList.Add(youUi);
        Debug.Log("init user info");
    }

    public void SortUserInfos()
    {
        DataManager.Ins.playerData.userInfoList = DataManager.Ins.playerData.userInfoList.OrderByDescending(x => x.score).ToList();
        for (int i = 0; i < DataManager.Ins.playerData.userInfoList.Count; i++)
        {
            UserInfo ui = DataManager.Ins.playerData.userInfoList[i];
            ui.rank = i + 1;
        }
    }

    public string GetRandomName()
    {
        string[] commonNames = {
        "James", "Emma", "Liam", "Olivia", "Ava", "Isabella", "Sophia", "Jackson", "Lucas",
        "Mia", "Logan", "Ethan", "Aiden", "Caden", "Oliver", "Charlotte", "Amelia", "Harper",
        "Benjamin", "Henry", "Alexander", "Sebastian", "Evelyn", "Ella", "Michael", "Daniel", "Matthew",
        "Joseph", "David", "Jack", "Owen", "Wyatt", "William", "Emily", "Madison", "Lily", "Grace",
         "Sofia", "Chloe", "Ella", "Scarlett", "Victoria",  "Lily", "Hannah", "Aubrey",
        "Nathan", "Samuel", "Ryan",  "Anthony", "Dylan", "Nicholas", "Connor", "Eli",
        "Grace", "Zoe", "Sophie", "Emma", "Eva", "Liam", "Lucas", "Mason", "Logan", "Caleb", "Nguyen", "Lee", "Zhen", 
        "Abdul", "Mohamed", "Tran", "Wei", "Xiao", "Kim", "Kang"
        };
        int randomIndex = Random.Range(0, commonNames.Length);
        string res = commonNames[randomIndex];
        /*bool isLowerCase = Random.Range(0, 100)%2==0;
        bool isAddNumber = Random.Range(0, 100) % 2 == 0;
        if (isLowerCase)
        {
            res = res.ToLower();
            res += Random.Range(123122, 912331).ToString();
        }*/
        return res;
    }

    public int GetRandomScore()
    {
        return Random.Range(100, 100000);
    }

    public void BoostUsersScore()
    {
        foreach(UserInfo ui in DataManager.Ins.playerData.userInfoList)
        {
            if(ui != youUi)
            {
                ui.score += UnityEngine.Random.Range(800,1600);
            }
        }
    }
}
