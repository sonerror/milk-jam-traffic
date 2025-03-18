using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class UShape : GameUnit
{
    [Header("Positions: ")]
    public Transform initialPos;
    public Transform targetLaunch;
    public Transform targetStuck;
    public Transform topPoint;

    [Header("Mesh: ")]
    public UShape_Model model;

    [Header("Conditions: ")]
    public bool isInited;
    public bool isFlying;
    public bool isReturning;
    public bool isShaking;
    public bool isCancelFly;

    [Header("Fly stats : ")]
    public float flySpeed;
    public Vector3 currentModelLocalPos;
    public bool isFlySuccess;

    [Header("Raycasts : ")]
    public UShape_Raycasts uShape_Raycasts;

    [Header("Type : ")]
    public SkinType skinType;
    public SizeType sizeType;
    public StickerType stickerType;

    [Header("Sticker : ")]
    public MeshFilter sticker;

    [Header("Trails : ")]
    public TrailRenderer[] trailRenderers;
    public Trail[] trails;

    public Coroutine moveCoroutine;
    public Dictionary<SkinType, UShape_Model> skinModelDict = new Dictionary<SkinType, UShape_Model>();
    public Dictionary<StickerType, GameObject> stickerObjDict = new Dictionary<StickerType, GameObject>();


    private void Start()
    {
        if (LevelManager.Ins.IsStage1())
        {
            OnInit();
            model.OnInit();
        }
    }

    public void OnInit()
    {
        //if (isInited) return;
        isFlying = false;
        isShaking = false;
        isReturning = false;
        isCancelFly = false;
        isFlySuccess = false;
        targetLaunch = this.TF.GetChildWithTag("target-launch");
        targetStuck.position = new Vector3(0, -9999f, 0);
        SetUpModelAndRefs();
        SetUpTypes();
        initialPos.localPosition = model.TF.localPosition;
        
        isInited = true;
    }

    public UShape_Model SpawnModel(int first, int last)
    {
#if UNITY_EDITOR
        model = ModelCollection.Ins.GetRandomModel(first, last);
#endif
        return model;
    }

    public void OnFinishShopping()
    {
        SetUpModelAndRefs();
        SetUpTypes();
        if (isFlySuccess)
        {
            model.vfx.ShowGreenVFX();
        }
    }

    public void SetUpModelAndRefs()
    {
        if (model == null) model = GetComponentInChildren<UShape_Model>();
        model.OnInit();
        Vector3 modelScale = model.transform.localScale;
        model.transform.SetParent(this.TF);
        if (isFlying)
        {
            model.transform.localPosition = currentModelLocalPos;
        }
        else
        {
            model.transform.localPosition = new Vector3(0, -1.5f, 0);
        }
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = modelScale;

        if (model != null && !skinModelDict.ContainsKey(model.skinType))
        {
            skinModelDict.Add(model.skinType, model);
        }

        topPoint = model.topPoint;
        uShape_Raycasts = model.uShape_Raycasts;
        uShape_Raycasts.OnInit();

        //trailRenderers = GetComponentsInChildren<TrailRenderer>(true);
        trails = model.GetComponentsInChildren<Trail>(true);
        ShowTrails(false);

        if(Application.isPlaying)
        sticker.transform.SetParent(model.transform);
    }

    public void SetUpTypes()
    {
        foreach(SkinModelsStruct sm in ModelCollection.Ins.skinModels)
        {
            for (int i = 0; i < sm.models.Count; i++)
            {
                UShape_Model m = sm.models[i];
                if (m.skinType == model.skinType && m.sizeType == model.sizeType)
                {
                    this.skinType = sm.skinType;
                    this.sizeType = (SizeType)i;
                    //this.stickerType = DataManager.Ins.playerData.stickerType;
                    return;
                }
            }
        }
    }

    public Vector3 GetDir()
    {
        return (targetLaunch.position - initialPos.position).normalized;
    }

    public void Fly()
    {
        if (isFlying) return;
        isFlying = true;
        Transform targetTF = null;

        // green vfx
        if (uShape_Raycasts.GetVectorRaycast() == Vector3.zero
            || (uShape_Raycasts.GetVectorRaycast() != Vector3.zero && uShape_Raycasts.IsAllHitUShapesFlying()))
        {
            model.vfx.ShowGreenVFX();
            isFlySuccess = true;
            if (LevelManager.Ins.currentLevelIndex >= 2)
            {
                DataManager.Ins.playerData.greenCount += 1;
                DataManager.Ins.playerData.greenCount = Mathf.Clamp(DataManager.Ins.playerData.greenCount, 0, ComboManager.Ins.currentLevelCombo.moveSum);
                ComboManager.Ins.CheckAward();
            }
        }

        LevelManager.Ins.CheckWinLose();


        Action OnComplete = () =>
        {
            DOVirtual.DelayedCall(0.3f, () =>
            {
                LevelManager.Ins.CheckWinLose();
            });
            if (LevelManager.Ins.IsStage1())
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                //PoolUshape.Ins.ReturnObject(this);
                SimplePool.Despawn(this);
            }
        };
        Action OnUpdate = () =>
        {
            if (isFlySuccess) return;

            uShape_Raycasts.GetVectorRaycast();
            Vector3 dirFly = this.model.TF.up;
            Vector3 currentTopPointPos = topPoint.position;
            Vector3 nextFrameTopPointPos = Vector3.MoveTowards(currentTopPointPos, currentTopPointPos + dirFly*100f, flySpeed * Time.deltaTime);

            float currentTargetStuckDistance = Vector3.Distance(currentTopPointPos, targetStuck.position);
            float currentNextFrameDistance = Vector3.Distance(currentTopPointPos, nextFrameTopPointPos);


            if (uShape_Raycasts.isRaycastingUshape
            && currentNextFrameDistance > currentTargetStuckDistance
            && !isReturning
            && !this.isFlySuccess) //đang bay thì đụng
            {
                //Debug.Log("đang bay thì đụng nên phải quay lại");
                foreach (UShape u in uShape_Raycasts.hitUShapeList)
                {
                    if (u.isFlying) continue;
                    u.Punch(dirFly);
                }
                isFlySuccess = false;
                isCancelFly = true;
                Return();
            }
        };
        targetTF = targetLaunch;
        Move(false, targetTF, flySpeed, OnComplete, OnUpdate);

  

    }

    public void Return()
    {
        isFlying = true;
        isReturning = true;
        model.vfx.ShowRedVFX();
        DataManager.Ins.playerData.greenCount = 0;
        ComboManager.Ins.ResetComboLabels();
        Move(true, initialPos, flySpeed * 0.9f, () =>
        {
            isFlying = false;
            isReturning = false;
            LevelManager.Ins.CheckWinLose();
        }, null);
        AudioManager.Ins.PlaySoundFX(SoundType.ClickFail);
        Vibrator.Vibrate(50);
    }

    public void Move(bool isReturn, Transform target, float speed, Action OnComplete, Action OnUpdate)
    {
        moveCoroutine = StartCoroutine(I_Move(isReturn, target, speed, OnComplete, OnUpdate));
    }

    IEnumerator I_Move(bool isReturn, Transform target, float speed, Action OnComplete, Action OnUpdate)
    {
        if (isReturn)
        {
            ShowTrails(false);
            while (model.TF.localPosition.y > target.localPosition.y)
            {
                model.TF.localPosition = Vector3.MoveTowards(model.TF.localPosition, target.localPosition, speed * Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            ShowTrails(true);
            while (Vector3.Distance(topPoint.position, target.position) > 1f)
            {
                OnUpdate?.Invoke();
                if (isCancelFly)
                {
                    isCancelFly = false;
                    yield break;
                }
                Vector3 targetModel = model.TF.localPosition + Vector3.up * 100f;
                speed += Time.deltaTime * 3;
                model.TF.localPosition = Vector3.MoveTowards(model.TF.localPosition, targetModel, speed * Time.deltaTime);
                yield return null;
            }
        }
        OnComplete?.Invoke();
        moveCoroutine = null;
        yield return null;
    }

    public void Punch(Vector3 dir)
    {
        isShaking = true;
        this.model.DOKill();
        Vector3 localDir = this.model.TF.GetLocalDir(dir);
        this.model.TF
            .DOPunchPosition(localDir/3, 0.1f, 2, 0.5f)
            .OnComplete(() =>
            {
                isShaking = false;
            });
    }

    public void SetUpTargetStuck(Vector3 pos)
    {
        targetStuck.position = pos;
    }

    public Vector3 GetDirFly()
    {
        return topPoint.position - initialPos.position;
    }

    public bool IsCollideOtherUshapesOnWay() // check xem có va nhau trên đường bay không
    {
        LayerMask mask = LayerMask.GetMask("bar");
        List<RaycastHit> hitList = new List<RaycastHit>();
        foreach (Transform org in uShape_Raycasts.origins)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(org.position, GetDirFly(), Mathf.Infinity, mask, QueryTriggerInteraction.Collide);
            hitList.AddRange(hits);
        }
        foreach(RaycastHit hit in hitList)
        {
            UShape u = hit.collider.gameObject.GetComponentInParent<UShape>();
            if (u != null && !u.isFlying)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsOppositeOtherUshapes() // check xem có đối đầu nhau không, nếu có thì không spawn nữa
    {
        bool isOppositeOtherUshapes = false;

        LayerMask mask = LayerMask.GetMask("bar");
        List<RaycastHit> hitList = new List<RaycastHit>();
        foreach (Transform org in uShape_Raycasts.origins)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(org.position, GetDirFly(), Mathf.Infinity, mask, QueryTriggerInteraction.Collide);
            hitList.AddRange(hits);
        }
        foreach (RaycastHit hit in hitList)
        {
            Vector3 hitDir = hit.collider.gameObject.GetComponentInParent<UShape>().GetDirFly();
            if (Vector3.Angle(hitDir, GetDirFly()) > 170 && Vector3.Angle(hitDir, GetDirFly()) < 190) // nguoc chieu
            {
                isOppositeOtherUshapes = true;
                break;
            }
        }
        return isOppositeOtherUshapes;
    }

    public void ShowTrails(bool isShow = true)
    {
        foreach(Trail t in trails)
        {
            if (DataManager.Ins!=null && DataManager.Ins.playerData.trailType == TrailType.None) {
                t.gameObject.SetActive(false);
                continue;
            } 

            t.gameObject.SetActive(isShow);
            if (isShow) {
                t.Play();
            } 
        }
    }

    public void OnExplode(float force, Vector3 center, float radius)
    {
        StopAllCoroutines();
        ShowTrails(false);
        model.vfx.HideVFX();
        model.SetAllBoxCollidersIsTrigger(false);
        model.rb.isKinematic = false;
        model.rb.AddExplosionForce(force, center, radius);
        isFlying = true;
        isFlySuccess = true;
    }

}
