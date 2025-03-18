using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public Sound[] sounds;
    public AudioSource musicSource;
    public AudioSource soundFXSource;



    public Sound GetSound(SoundType type)
    {
        return sounds[(int)type];
    }

    public void PlayMusic(SoundType soundType)
    {
        if (!DataManager.Ins.playerData.isSoundOn) return;
        Sound s = GetSound(soundType);
        musicSource.clip = s.clip;
        musicSource.volume = s.volume; 
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySoundFX(SoundType soundType)
    {
        if (!DataManager.Ins.playerData.isSoundOn) return;
        Sound s = GetSound(soundType);
        soundFXSource.clip = s.clip;
        soundFXSource.volume = s.volume;
        soundFXSource.PlayOneShot(soundFXSource.clip);
    }
}
