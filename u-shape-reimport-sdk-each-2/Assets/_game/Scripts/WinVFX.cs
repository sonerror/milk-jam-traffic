using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinVFX : Singleton<WinVFX>
{
    public ParticleSystem[] particleSystems;

    private void OnEnable()
    {
        EventManager.OnLoadNewScene += Hide;
    }

    private void OnDisable()
    {
        EventManager.OnLoadNewScene -= Hide;
    }

    private void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    public void Show()
    {
        Hide();
        AudioManager.Ins.PlaySoundFX(SoundType.Win);
        bool isLoop = !LevelManager.Ins.IsStage1();
        foreach (ParticleSystem p in particleSystems)
        {
            p.loop = isLoop;
            p.Play();
        }
        //AudioManager.Ins.PlaySoundFX(SoundType.Confetti);
    }

    public void Hide()
    {
        foreach (ParticleSystem p in particleSystems)
        {
            p.Stop();
        }
    }
}
