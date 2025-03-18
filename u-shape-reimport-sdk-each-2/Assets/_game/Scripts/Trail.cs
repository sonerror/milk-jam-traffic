using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    public new ParticleSystem particleSystem;
    public Material mat;



    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        if (particleSystem != null)
        {
            //particleSystem.startSize = LevelManager.Ins.currentLevel.container.localScale.x * 7f;
            particleSystem.startSize = LevelManager.Ins.currentLevel.uShapes[0].TF.lossyScale.x * 12f;
            particleSystem.startSpeed = LevelManager.Ins.currentLevel.uShapes[0].TF.lossyScale.x * 100;
            particleSystem.startLifetime = 0.2f;
            if (mat != null)
            {
                particleSystem.GetComponent<ParticleSystemRenderer>().material = mat;
            }
            particleSystem.Play(true);
        }
    }
}
