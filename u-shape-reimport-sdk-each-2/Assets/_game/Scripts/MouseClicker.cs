using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MouseClicker : Singleton<MouseClicker>
{
    public UShape uShape;
    public Vector3 mousePos;
    public bool isMouseDownOnMesh;
    public bool isUsingBomb;

    public void OnInit()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !UI_Hover.IsPointerOverUIElement()) // nhấn chuột
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("bar");
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 2f);
            if (Physics.Raycast(ray, out hit, 1000f, mask))// bắn raycast trúng bar
            {
                mousePos = hit.collider.transform.position;
                uShape = hit.collider.GetComponentInParent<UShape>();
                isMouseDownOnMesh = true;
            }
            Debug.LogError(LevelManager.Ins.currentLevel.isTutorialRotate + "leeeeeeeeeeeeeeeeeeeeeeeeeeê ");
            if (LevelManager.Ins.currentLevel.isTutorialRotate && !DataManager.Ins.playerData.isPassedTutorialRotate)
            {
                UIManager.Ins.GetUI<Gameplay>().tutRotate.gameObject.SetActive(false);
                UIManager.Ins.GetUI<Gameplay>().hand.gameObject.SetActive(false);
                DataManager.Ins.playerData.isPassedTutorialRotate = true;
                AFSendEvent.SendEvent("af_tutorial_completion", new Dictionary<string, string>()
                {
                    {"af_success", "true"},
                    {"af_tutorial_id", "1" }
                });
            }
        }
        if (Input.GetMouseButtonUp(0) && !UI_Hover.IsPointerOverUIElement()) // thả chuột
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            LayerMask barMask = LayerMask.GetMask("bar");
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);
            if (Physics.Raycast(ray, out hit, 1000f, barMask)) // bắn raycast trúng bar
            {
                if (isMouseDownOnMesh && Vector3.Distance(mousePos, hit.collider.transform.position) < 0.1f)  // mouseUpPos trùng mouseDownPos
                {
                    if (isUsingBomb) // nếu đang sử dụng destroy thì setactive(false)
                    {
                        /*uShape.gameObject.SetActive(false);
                        DOVirtual.DelayedCall(0.3f, () =>
                        {
                            LevelManager.Ins.CheckWin();
                        });*/
                        Vector3 targetThrow = hit.collider.transform.position + new Vector3(0,0,1.5f);
                        DataManager.Ins.playerData.bombCount -= 1;
                        UIManager.Ins.GetUI<Gameplay>().ShowBombCountOrAds();
                        BombManager.Ins.ThrowBomb(targetThrow, () =>
                        {
                           
                            Collider[] colliders = Physics.OverlapSphere(targetThrow, BombManager.Ins.explodeRadius, barMask);
                            List<UShape> hitUshapeList = new List<UShape>();
                            foreach (Collider collider in colliders)
                            {
                                UShape u = collider.GetComponentInParent<UShape>();
                                if (u != null)
                                {
                                    hitUshapeList.Add(u);
                                }
                            }
                            foreach (UShape u in hitUshapeList)
                            {
                                if (u != null)
                                    u.OnExplode(BombManager.Ins.explodeForce, targetThrow, BombManager.Ins.explodeRadius);
                            }
                            LevelManager.Ins.CheckWinLose();

                            //GameObject obj = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_game/Prefabs/explosions/explosion1.prefab")) as GameObject;
                            Explosion explosion = SimplePool.Spawn<Explosion>(PoolType.Explosion);
                            explosion.TF.position = targetThrow;
                            DOVirtual.DelayedCall(1.4f, () =>
                            {
                                SimplePool.Despawn(explosion);
                            });
                            StartCoroutine(ComboManager.Ins.I_CalculateGreenCountAfterExplosion(hitUshapeList.Count/3));

                            DOVirtual.DelayedCall(2f, () =>
                            {
                                foreach (UShape u in hitUshapeList)
                                {
                                    if (u == null) continue;
                                    if (LevelManager.Ins.IsStage1())
                                    {
                                        u.gameObject.SetActive(false);
                                    }
                                    else
                                    {
                                        //PoolUshape.Ins.ReturnObject(u);
                                        SimplePool.Despawn(u);
                                    }
                                    //DestroyImmediate(u.gameObject);
                                }
                            });
                            AudioManager.Ins.PlaySoundFX(SoundType.BombExplode);
                            Vibrator.Vibrate(100);
                        });
                        if(HandBomb.isActive)
                        {
                            UIManager.Ins.GetUI<Gameplay>().ShowCutOutBomb(false);
                            LevelManager.Ins.currentLevel.OnContinue(0f);
                            HandBomb.Ins.Hide();
                        }
                    }
                    else if(!HandX2.isActive && !HandBomb.isActive)  // nếu không sử dụng bomb và ko tutorial thì fly
                    {
                        uShape?.Fly();
                        if (uShape.isFlySuccess)
                        {
                            AudioManager.Ins.PlaySoundFX(SoundType.ClickSuccess);
                            DoublePick.Ins.PickAnotherUShape();
                        }
                        if (LevelManager.Ins.currentLevel.isTutorialClick && !DataManager.Ins.playerData.isPassedTutorialClick) //nếu là level tutorial thì tắt tutorial đi
                        {
                            UIManager.Ins.GetUI<Gameplay>().hand.gameObject.SetActive(false);
                            DataManager.Ins.playerData.isPassedTutorialClick = true;
                            AFSendEvent.SendEvent("af_tutorial_completion", new Dictionary<string, string>()
                            {
                                {"af_success", "true"},
                                {"af_tutorial_id", "0" }
                            });
                        }
                    }
                }
                else
                {
                    //Debug.LogError("mousePos != hit.collider.position");
                }
                isMouseDownOnMesh = false;
            }
            else
            {
                //Debug.LogError("raycast does not hit layer ");
            }

            if (!HandBomb.isActive)
            {
                isUsingBomb = false;
                UIManager.Ins.GetUI<Gameplay>().UpdateBtnBomb();
            }
        }
    }


}
