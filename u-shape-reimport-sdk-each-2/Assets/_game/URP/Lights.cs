using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    public Light[] lights;
    public float intensity1;
    public float intensity2;


    private void Awake()
    {
        lights = GetComponentsInChildren<Light>();
    }

    private void OnEnable()
    {
        EventManager.OnLoadNewScene += OnNewScene;
    }

    private void OnDisable()
    {
        EventManager.OnLoadNewScene -= OnNewScene;
    }

    public void OnNewScene()
    {
        foreach (Light light in lights)
        {
            if(DataManager.Ins.playerData.skinType == SkinType.None)
            {
                light.intensity = intensity1;
                continue;
            }
            if (LevelManager.Ins.IsStage1())
            {
                light.intensity = intensity1;
            }
            else
            {
                light.intensity = intensity2;
            }
        }
    }
}
