using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>   
{
    public Animator animator;
    public string currentSceneName = "Loading";
    public bool isLoadingNewScene = false;

    public void LoadCurrentScene(Action OnComplete = null, bool isFadeOut = true, bool isFadeIn = true)
    {
        ChangeScene(SceneManager.GetActiveScene().name, OnComplete, isFadeOut, isFadeIn);
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void ChangeScene(string sceneName, Action OnComplete, bool isFadeOut, bool isFadeIn)
    {
        if (isLoadingNewScene) return;
        StartCoroutine(I_ChangeScene(sceneName,OnComplete, isFadeOut,isFadeIn));
    }

    IEnumerator I_ChangeScene(string sceneName, Action OnComplete, bool isFadeOut, bool isFadeIn)
    {
        isLoadingNewScene = true;
        // Fade Out
        if (isFadeOut)
        {
            animator.gameObject.SetActive(true);
            animator.transform.SetAsLastSibling();
            animator.SetTrigger("out");
            yield return new WaitForSeconds(0.67f);
        }
        else
        {
            animator.gameObject.SetActive(false);
        }

        //PoolUshape.Ins.ReturnAllObjects();
        SimplePool.CollectAll();

        // Load Scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Scene scene = SceneManager.GetSceneByName(sceneName);
        yield return new WaitUntil(() => scene.IsValid() && scene.isLoaded);
        currentSceneName = sceneName;


        OnComplete?.Invoke();
        EventManager.TriggerLoadNewScene();
        if (LevelManager.Ins.IsStage1())
        {
            EventManager.TriggerLoadStage1();
        }
        else
        {
            EventManager.TriggerLoadStage2();
        }

        // Fade In
        if (isFadeIn)
        {
            animator.gameObject.SetActive(true);
            animator.transform.SetAsLastSibling();
            animator.SetTrigger("in");
        }
        else
        {
            animator.gameObject.SetActive(false);
        }


        isLoadingNewScene = false;

        yield return null;
    }

}
