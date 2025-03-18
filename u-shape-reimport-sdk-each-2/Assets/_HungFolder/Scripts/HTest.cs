using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTest : MonoBehaviour
{
    [Button]
    void OpenShop()
    {
        UIManager.Ins.OpenUI<UIShop>();
    }

    [Button]
    void Test(int x)
    {
        UIManager.Ins.OpenUI<UIWin>().OnInit(x);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            UIManager.Ins.OpenUI<UIShop>();
        }
    }
}
