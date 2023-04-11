using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class SoundData : BaseData
{
    public List<SoundClip> soundClips = new List<SoundClip>();
    
    private string clipPath = "Prefabs/Sounds/";
    private string jsonFileName = "soundData.json";

    public void LoadData()
    {
        string jdata;
        SoundData soundData = ScriptableObject.CreateInstance<SoundData>();

        try
        {
            jdata = System.IO.File.ReadAllText(Application.dataPath + dataDirectory + jsonFileName);
            soundData = JsonConvert.DeserializeObject<SoundData>(jdata);
        }
        catch (Exception e)
        {
            return;
        }
        
        this.soundClips = soundData.soundClips;
        this.dataNameList = soundData.dataNameList;
        
        foreach (var clip in soundClips)
        {
            clip.LoadSound();
        }
    }
    
    public void SaveData()
    {
        foreach (var clip in soundClips)
        {
            clip.ReleaseSound();
        }
        string jdata = JsonConvert.SerializeObject(this);
        System.IO.File.WriteAllText(Application.dataPath + dataDirectory + jsonFileName, jdata);
    }

    public override int AddData()
    {
        SoundClip soundClip = new SoundClip();
        soundClip.soundName = "New Sound";
        soundClip.soundPath = clipPath;
        this.soundClips.Add(soundClip);
        dataNameList.Add(soundClip.soundName);

        return GetDataCount();
    }

    public override void RemoveData(int index)
    {
        soundClips.RemoveAt(index);
        dataNameList.RemoveAt(index);
    }

    public SoundClip GetSound(int index)
    {
        if (soundClips[index] == null)
        {
            return null;
        }
        
        soundClips[index].LoadSound();
        return soundClips[index];
    }
}
