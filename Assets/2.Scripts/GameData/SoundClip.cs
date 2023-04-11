using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundClip
{
    public int soundID = 0;
    public SOUNDPLAYTYPE SOUNDTYPE = SOUNDPLAYTYPE.NONE;
    public string soundPath = String.Empty;
    public string soundName = String.Empty;
    public AudioClip soundPrefab = null;
    
    // AudioClip 속성들
    public bool hasLoop = false;
    public float maxVolume = 1.0f;
    public float pitch = 1.0f;
    
    // 3D Sound
    public float dopplerLevel = 1.0f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;
    public float minDistance = 10000.0f;
    public float maxDistance = 50000.0f;
    public float spartialBlend = 1.0f;

    // Fade
    public float fadeTime1 = 0.0f;
    public float fadeTime2 = 0.0f;
    public bool isFadeIn = false;
    public bool isFadeOut = false;

    public void LoadSound()
    {
        if (this.soundPrefab == null)
        {
            this.soundPrefab = ResourceManager.Load(soundPath + soundName) as AudioClip;
        }
    }

    public void ReleaseSound()
    {
        if (this.soundPrefab != null)
        {
            this.soundPrefab = null;
        }
    }

    public void FadeIn(float time)
    {
        this.isFadeOut = false;
        this.fadeTime1 = 0.0f;
        this.fadeTime2 = time;
        this.isFadeIn = true;
    }

    public void FadeOut(float time)
    {
        this.isFadeIn = false;
        this.fadeTime1 = 0.0f;
        this.fadeTime2 = time;
        this.isFadeOut = true;
    }

    public void DoFade()
    {
        if (this.isFadeIn == true)
        {
           // this.fadeTime1 +=  
        }
    }
    
}
