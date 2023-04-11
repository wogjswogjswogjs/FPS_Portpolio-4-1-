using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EffectManager : SingletonMonobehaviour<EffectManager>
{
    private Transform effectRoot = null;
    public const string ContainerName = "EffectContainer";

    void Start()
    {
        if (effectRoot == null)
        {
            effectRoot = new GameObject(ContainerName).transform;
            effectRoot.SetParent(this.transform);
        }
    }

    public GameObject EffectOneShot(int index, Vector3 position)
    {
        EffectClip clip = DataManager.GetEffectData().GetEffect(index);
        GameObject effect = clip.Instantiate(position);
        effect.SetActive(true);
        return effect;
    }
}
