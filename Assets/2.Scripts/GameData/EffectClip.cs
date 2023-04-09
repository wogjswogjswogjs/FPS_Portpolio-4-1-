using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

/// <summary>
/// 이펙트 프리펩과 경로, 타입등의 속성 데이터를 가지고 있다,
/// 프리팹 로딩 기능을 갖고 있다 - 풀링을 위한 기능
/// 이펙트 인스턴스 기능을 갖고 있다
/// </summary>
public class EffectClip
{
    public int effectID = 0;
    public EFFECTTYPE EFFECTTYPE = EFFECTTYPE.NORMAL;
    public string effectPath = string.Empty;
    public string effectName = string.Empty;
    public GameObject effectPrefab = null;

    public void LoadEffect()
    {
        if (this.effectPrefab == null)
        {
            this.effectPrefab = ResourceManager.Load(effectPath + effectName) as GameObject;
        }
    }

    public void ReleaseEffect()
    {
        if (this.effectPrefab != null)
        {
            this.effectPrefab = null;
        }
    }

    public GameObject Instantiate(Vector3 pos)
    {
        if (this.effectPrefab == null)
        {
            this.LoadEffect();
        }

        GameObject effect = GameObject.Instantiate(effectPrefab, pos, Quaternion.identity);
        return effect;
    }
}
