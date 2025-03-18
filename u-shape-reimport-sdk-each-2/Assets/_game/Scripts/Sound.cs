using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sound
{
    public SoundType type;
    public AudioClip clip;
    public float volume;

    public Sound()
    {
        volume = 1.0f;
    }
}
