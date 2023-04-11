using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static SoundData soundData = null;
    private static EffectData effectData = null;

    void Start()
    {
        if (effectData == null)
        {
            effectData = ScriptableObject.CreateInstance<EffectData>();
            effectData.LoadData();
        }

        if (soundData == null)
        {
            soundData = ScriptableObject.CreateInstance<SoundData>();
            soundData.LoadData();
        }
    }

    public static EffectData GetEffectData()
    {
        if (effectData == null)
        {
            effectData = ScriptableObject.CreateInstance<EffectData>();
            effectData.LoadData();
        }

        return effectData;
    }

    public static SoundData GetSoundData()
    {
        if (soundData == null)
        {
            soundData = ScriptableObject.CreateInstance<SoundData>();
            soundData.LoadData();
        }

        return soundData;
    }
}
