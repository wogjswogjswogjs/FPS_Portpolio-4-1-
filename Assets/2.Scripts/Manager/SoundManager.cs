using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : SingletonMonobehaviour<SoundManager>
{
    public Transform audioRoot = null;
    public const string ContainerName = "SoundContainer";
    
    public const string MixerName = "AudioMixer";
    public const string MasterGroupName = "Master";
    public const string EffectGroupName = "Effect";
    public const string BGMGroupName = "BGM";
    public const string UIGroupName = "UI";
    public const string MasterVolumeParam = "Volume_Master";
    public const string EffectVolumeParam = "Volume_Effect";
    public const string BGMVolumeParam = "Volume_BGM";
    public const string UIVolumeParam = "Volume_UI";

    public enum MUSICPLAYINGTYPE
    {
        NONE = 0,
        SOUECEA = 1,
        SOURCEB = 2,
        ATOB = 3,
        BTOA = 4
    }
    
    // 오디오 채널
    public AudioMixer mixer = null;
    private float minVolume = -80.0f;
    private float maxVolume = 0.0f;
    public AudioSource BGM_fadeA_audio = null;
    public AudioSource BGM_fadeB_audio = null;
    public AudioSource[] effect_audios = null;
    public float[] effect_PlayStartTime = null;
    private int EffectChannelCount = 5;
    public AudioSource UI_audio = null;

    private MUSICPLAYINGTYPE currentPlayingType = MUSICPLAYINGTYPE.NONE;
    private bool isTiching = false;
    private SoundClip currentSound = null;
    private SoundClip lastSound = null;

    private void Start()
    {
        if (this.mixer == null)
        {
            this.mixer = Resources.Load(MixerName) as AudioMixer;
        }

        if (this.audioRoot == null)
        {
            audioRoot = new GameObject(ContainerName).transform;
            audioRoot.SetParent(this.transform);
            audioRoot.localPosition = Vector3.zero;
        }

        if (BGM_fadeA_audio == null)
        {
            GameObject fadeA = new GameObject("FadeA", typeof(AudioSource));
            fadeA.transform.SetParent(audioRoot);
            this.BGM_fadeA_audio = fadeA.GetComponent<AudioSource>();
            this.BGM_fadeA_audio.playOnAwake = false;
        }

        if (BGM_fadeB_audio == null)
        {
            GameObject fadeB = new GameObject("FadeB", typeof(AudioSource));
            fadeB.transform.SetParent(audioRoot);
            this.BGM_fadeB_audio = fadeB.GetComponent<AudioSource>();
            this.BGM_fadeB_audio.playOnAwake = false;
        }

        if (UI_audio == null)
        {
            GameObject ui = new GameObject("UI", typeof(AudioSource));
            ui.transform.SetParent(audioRoot);
            this.UI_audio = ui.GetComponent<AudioSource>();
            this.UI_audio.playOnAwake = false;
        }

        if (effect_audios == null || effect_audios.Length == 0)
        {
            this.effect_PlayStartTime = new float[EffectChannelCount];
            this.effect_audios = new AudioSource[EffectChannelCount];
            for (int i = 0; i < EffectChannelCount; i++)
            {
                effect_PlayStartTime[i] = 0.0f;
                GameObject effect = new GameObject("Effect" + i.ToString(), typeof(AudioSource));
                effect.transform.SetParent(audioRoot);
                this.effect_audios[i] = effect.GetComponent<AudioSource>();
                this.effect_audios[i].playOnAwake = false;
            }
        }

        if (this.mixer != null)
        {
            this.BGM_fadeA_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(BGMGroupName)[0];
            this.BGM_fadeB_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(BGMGroupName)[0];
            this.UI_audio.outputAudioMixerGroup = mixer.FindMatchingGroups(UIGroupName)[0];
            for (int i = 0; i < EffectChannelCount; i++)
            {
                this.effect_audios[i].outputAudioMixerGroup = mixer.FindMatchingGroups(EffectGroupName)[0];
            }
        }

        VolumeInit();
    }
    
    void VolumeInit()
    {
        if (this.mixer != null)
        {
            this.mixer.SetFloat(BGMVolumeParam, GetBGMVolume());
            this.mixer.SetFloat(EffectVolumeParam, GetEffectVolume());
            this.mixer.SetFloat(UIVolumeParam, GetUIVolume());
        }
    }
    
    public void SetBGMVolume(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        float volume = Mathf.Lerp(minVolume, maxVolume, ratio);
        this.mixer.SetFloat(BGMVolumeParam, volume);
        PlayerPrefs.SetFloat(BGMVolumeParam, volume);
    }

    public float GetBGMVolume()
    {
        if (PlayerPrefs.HasKey(BGMVolumeParam))
        {
            return Mathf.Lerp(minVolume, maxVolume, PlayerPrefs.GetFloat(BGMVolumeParam));
        }
        else
        {
            return maxVolume;
        }
    }

    public void SetEffectVolume(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        float volume = Mathf.Lerp(minVolume, maxVolume, ratio);
        this.mixer.SetFloat(EffectVolumeParam, volume);
        PlayerPrefs.SetFloat(EffectVolumeParam, volume);
    }

    public float GetEffectVolume()
    {
        if (PlayerPrefs.HasKey(EffectVolumeParam))
        {
            return Mathf.Lerp(minVolume, maxVolume, PlayerPrefs.GetFloat(EffectVolumeParam));
        }
        else
        {
            return maxVolume;
        }
    }

    public void SetUIVolume(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        float volume = Mathf.Lerp(minVolume, maxVolume, ratio);
        this.mixer.SetFloat(UIVolumeParam, volume);
        PlayerPrefs.SetFloat(UIVolumeParam, volume);
    }

    public float GetUIVolume()
    {
        if (PlayerPrefs.HasKey(UIVolumeParam))
        {
            return Mathf.Lerp(minVolume, maxVolume, PlayerPrefs.GetFloat(UIVolumeParam));
        }
        else
        {
            return maxVolume;
        }
    }

    public void PlayAudioSource(AudioSource source, int index, float volume)
    {
        SoundClip clip = DataManager.GetSoundData().GetSound(index);
        if (source == null || clip == null)
        {
            return;
        }
        source.Stop();
        source.clip = clip.soundPrefab;
        source.volume = volume;
        source.pitch = clip.pitch;
        source.dopplerLevel = clip.dopplerLevel;
        source.rolloffMode = clip.rolloffMode;
        source.minDistance = clip.minDistance;
        source.maxDistance = clip.maxDistance;
        source.spatialBlend = clip.spartialBlend;
        source.Play();
    }

    public void PlayAudioSourceAtPoint(int index, Vector3 position, float volume)
    {
        AudioSource.PlayClipAtPoint(DataManager.GetSoundData().GetSound(index).soundPrefab, position, volume);
    }

    public bool IsPlaying()
    {
        return (int)this.currentPlayingType > 0;
    }

    public bool IsDifferentSound(SoundClip clip)
    {
        if (clip == null)
        {
            return false;
        }

        if (currentSound != null && currentSound.soundID == clip.soundID &&
            IsPlaying() && currentSound.isFadeOut == false)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
