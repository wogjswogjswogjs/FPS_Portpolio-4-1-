using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class EffectData : BaseData
{
   public List<EffectClip> effectClips = new List<EffectClip>();

   private string clipPath = "Prefabs/Effects/";
   private string jsonFileName = "effectData.json";

   /// <summary>
   /// json파일로 저장되어 있는 EffectData정보를 불러오는 함수
   /// </summary>
   public void LoadData()
   {
      string jsonData;
      EffectData effectData = ScriptableObject.CreateInstance<EffectData>();

      try
      {
         jsonData = System.IO.File.ReadAllText(Application.dataPath + dataDirectory + jsonFileName);
         effectData = JsonConvert.DeserializeObject<EffectData>(jsonData);
      }
      catch (Exception e)
      {
         return;
      }

      this.effectClips = effectData.effectClips;
      this.dataNameList = effectData.dataNameList;

      foreach (var clip in this.effectClips)
      {
         clip.LoadEffect();
      }
   }

   /// <summary>
   /// EffectData정보를 json파일로 저장하는 함수
   /// </summary>
   public void SaveData()
   {
      foreach (var clip in this.effectClips)
      {
         clip.ReleaseEffect();
      }

      string jsonData = JsonConvert.SerializeObject(this);
      System.IO.File.WriteAllText(Application.dataPath + dataDirectory + jsonFileName, jsonData);
   }

   public override int AddData()
   {
      EffectClip effectClip = new EffectClip();
      effectClip.effectName = "New Effect";
      effectClip.effectPath = clipPath;
      this.effectClips.Add(effectClip);
      this.dataNameList.Add(effectClip.effectName);

      return GetDataCount();
   }

   public override void RemoveData(int index)
   {
      this.effectClips.RemoveAt(index);
      this.dataNameList.RemoveAt(index);
   }
   
   public void ClearData()
   {
      foreach (var clip in this.effectClips)
      {
         clip.ReleaseEffect();
      }

      this.effectClips = null;
      this.dataNameList = null;
   }

   public EffectClip GetCopyEffect(int index)
   {
      if (effectClips[index] == null)
      {
         return null;
      }

      EffectClip original = this.effectClips[index];
      EffectClip retClip = new EffectClip();
      retClip.effectPath = original.effectPath;
      retClip.effectName = original.effectName;
      retClip.EFFECTTYPE = original.EFFECTTYPE;
      retClip.effectID = original.effectID;
      return retClip;
   }

   public EffectClip GetEffect(int index)
   {
      if (effectClips[index] == null)
      {
         return null;
      }
      effectClips[index].LoadEffect();
      return effectClips[index];
   }
}
