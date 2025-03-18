using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoUI : MonoBehaviour
{
    public Image medal23img;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    public GameObject medalRecent;
    public GameObject medal1;
    public GameObject medal23;

    public void SetValue(UserInfo ui)
    {
        rankText.text = ui.rank.ToString();
        nameText.text = ui.username;
        scoreText.text = ui.score.ToString();
    }

    public void ShowMedal(UserInfo ui)
    {
        if (ui.rank == 1)
        {
            medal1.SetActive(true);
            medalRecent.SetActive(false);
            medal23.SetActive(false);
        }
        else if (ui.rank <= 3) { 
            medal1.SetActive(false);
            medal23.SetActive(true);
            medal23img.sprite = UIManager.Ins.GetUI<Leaderboard>().medals[ui.rank-1];
            medalRecent.SetActive(false);
        }
        else
        {
            medal1.SetActive(false);
            medal23.SetActive(false);
            medalRecent.SetActive(true);
        }
    }
}
