using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : Singleton<BombManager>
{
    public AnimationCurve curveY;
    public float throwSpeed = 5f;
    private float throwDuration;
    public float offsetY;
    public float explodeForce = 555f;
    public float explodeRadius = 2f;
    public bool isThrowingBomb;


    public void ThrowBomb(Vector3 target, Action OnComplete)
    {
        isThrowingBomb = true;
        StartCoroutine(I_ThrowBomb(target, OnComplete));
    }

    IEnumerator I_ThrowBomb(Vector3 target, Action OnComplete)
    {
        Bomb bomb = SimplePool.Spawn<Bomb>(PoolType.Bomb);
        bomb.TF.position = new Vector3(-3.25f, -13f, -8.5f);
        bomb.TF.localScale = Vector3.one;
        //bomb.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(-360f, 360f)));
        Vector3 initialPos = bomb.TF.position;
        float distance = Vector3.Distance(bomb.TF.position, target);
        throwDuration = distance / throwSpeed;
        bomb.TF
            .DOMove(target, throwDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                OnComplete?.Invoke();
                SimplePool.Despawn(bomb);
                isThrowingBomb = false;
            });
        //bomb.transform
            //.DORotate(this.transform.eulerAngles, 1f, RotateMode.FastBeyond360);
        yield return null;
    }

    /*IEnumerator I_ThrowBomb(Vector3 target, Action OnComplete)
    {
        Transform bomb = Instantiate(bombPrefab);
        bomb.position = new Vector3(-3.25999999f, -3.1500001f, -27.8199997f);
        bomb.localScale = Vector3.one;
        Vector3 initialPos = bomb.position;
        float distance = Vector3.Distance(bomb.position, target);
        throwDuration = distance / throwSpeed;
        float elapsedTime = 0f;
        while(elapsedTime < throwDuration)
        {
            float t = (float)elapsedTime / (float)throwDuration;
            bomb.position = Vector3.Lerp(initialPos, target, t);
            bomb.position += Vector3.up * curveY.Evaluate(t) * offsetY;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        OnComplete?.Invoke();
        Destroy(bomb.gameObject);
        yield return null;
    }*/

}
